using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Nuke.Components;
using System.IO;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildSettingsExtensions;
using NuGet.Configuration;

[CheckBuildProjectConfigurations(TimeoutInMilliseconds = 5000)]
[UnsetVisualStudioEnvironmentVariables]
public class BuildScripts : NukeBuild
{
	public static int Main() => Execute<BuildScripts>(x => x.Build);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution] readonly Solution Solution;
	[GitRepository] readonly GitRepository GitRepository;
	[GitVersion] readonly GitVersion GitVersion;
	[CI] readonly AzurePipelines AzurePipelines;

	const string SourceFolder = "src";
	const string TestsFolder = "tests";

	AbsolutePath SourceDirectory => RootDirectory / SourceFolder;
	AbsolutePath TestsDirectory => RootDirectory / TestsFolder;
	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
	AbsolutePath TestArtifactsDirectory => ArtifactsDirectory / "tests";
	AbsolutePath NugetDirectory => ArtifactsDirectory / "nuget";
	AbsolutePath CodeCoverageReportOutput => TestArtifactsDirectory / "Reports";
	AbsolutePath CodeCoverageFile => TestArtifactsDirectory / "coverage.cobertura.xml";

	AbsolutePath NugetConfigFile => RootDirectory / "nuget.config";

	string GeneratorPackageName => "Beffyman.AspNetCore.Client.Generator";

	RelativePath TestGeneratorProject => (RelativePath)TestsFolder / "Beffyman.AspNetCore.Client.Test.Generator" / "Beffyman.AspNetCore.Client.Test.Generator.csproj";
	RelativePath GeneratorProject => (RelativePath)SourceFolder / GeneratorPackageName / $"{GeneratorPackageName}.csproj";

	const string TestGeneratorFramework = "net6.0";

	private void CleanArtifacts(bool packages = true)
	{
		SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
		TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);

		if (packages)
		{
			EnsureCleanDirectory(ArtifactsDirectory);
		}
	}

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			if (IsLocalBuild)
			{
				var settings = Settings.LoadDefaultSettings(null);
				var globalNugetPath = SettingsUtility.GetGlobalPackagesFolder(settings);
				var packageDir = Path.Combine(globalNugetPath, GeneratorPackageName, GitVersion.NuGetVersionV2);

				if (Directory.Exists(packageDir))
				{
					try
					{
						DeleteDirectory(packageDir);
						Serilog.Log.Information("Stale package version has been removed");
					}
					catch (Exception)
					{
						Serilog.Log.Error("Could not delete stale package directory, this build may be stale, so erroring.");
						throw;
					}
				}
			}

			CleanArtifacts();
		});

	Target Restore => _ => _
		.Executes(() =>
		{
			DotNetRestore(s => s
			.SetProjectFile(Solution));
		});

	Target Build => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(s => s
				.SetProjectFile(Solution)
				.SetVerbosity(DotNetVerbosity.Normal)
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.EnableNoRestore());
		});

	private void RunTests()
	{
		DotNetTest(s => s
				.SetConfiguration(Configuration)
				.SetVerbosity(DotNetVerbosity.Normal)
				.EnableNoBuild()
				.EnableNoRestore()
				.SetLoggers("trx")
				.SetResultsDirectory(TestArtifactsDirectory)
				.EnableProcessLogOutput()
				.SetProcessArgumentConfigurator(arguments => arguments.Add("/p:CollectCoverage={0}", "true")
					.Add("/p:CoverletOutput={0}/", TestArtifactsDirectory)
					//.Add("/p:Threshold={0}", 90)
					.Add("/p:Exclude=\"[xunit*]*%2c[*.Tests]*\"")
					.Add("/p:UseSourceLink={0}", "true")
					.Add("/p:CoverletOutputFormat={0}", "cobertura"))
				.SetProjectFile(Solution));

		Nuke.Common.IO.FileSystemTasks.FileExists(CodeCoverageFile);
	}

	Target Test => _ => _
		.DependsOn(Build)
		.Executes(() =>
		{
			RunTests();
		});


	Target Pack => _ => _
		.DependsOn(Build)
		.After(Test)
		.Executes(() =>
		{
			EnsureExistingDirectory(NugetDirectory);

			DotNetPack(s => s
					.SetProject(Solution)
					.SetVerbosity(DotNetVerbosity.Normal)
					.SetVersion(GitVersion.NuGetVersionV2)
					.SetConfiguration(Configuration)
					.SetAssemblyVersion(GitVersion.AssemblySemVer)
					.SetFileVersion(GitVersion.AssemblySemFileVer)
					.SetInformationalVersion(GitVersion.InformationalVersion)
					.SetOutputDirectory(NugetDirectory));
		});

	Target GenerateTestProjectClients => _ => _
		.DependsOn(Build)
		.Before(Test)
		.Executes(() =>
		{
			DotNetRun(s => s
						.SetProjectFile(TestGeneratorProject)
						.SetConfiguration(Configuration)
						.SetFramework(TestGeneratorFramework)
						.EnableNoBuild());
		});

	Target BuildWithGenerator => _ => _
		.After(Pack)
		.Executes(() =>
		{
			EnsureExistingDirectory(NugetDirectory);

			//First run through the dotnet msbuild workflow for the projects using the packaged version of it
			CleanArtifacts(false);

			DotNetBuild(s => s
				.SetVerbosity(DotNetVerbosity.Normal)
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.EnableNoCache()
				.AddProperty("GenerateWithNuget", "true")
				.AddProperty("GeneratorVersion", GitVersion.NuGetVersionV2)
				.AddSources(NugetDirectory));

			RunTests();

			//Then run through the Visual studio MsBuild workflow for the projects using the packaged version of it

			CleanArtifacts(false);

			MSBuild(s => s
					.SetVerbosity(Nuke.Common.Tools.MSBuild.MSBuildVerbosity.Normal)
					.SetSolutionFile(Solution)
					.SetConfiguration(Configuration)
					.AddProperty("GenerateWithNuget", "true")
					.AddProperty("GeneratorVersion", GitVersion.NuGetVersionV2)
					.AddRestoreSources(NugetDirectory)
					.SetTargets("Restore", "Rebuild")
					.SetNodeReuse(IsLocalBuild)
					.SetMaxCpuCount(Environment.ProcessorCount));

			RunTests();

		});

	Target CI => _ => _
		.DependsOn(Clean)
		.DependsOn(Build)
		.DependsOn(GenerateTestProjectClients)
		.DependsOn(Test)
		.DependsOn(Pack)
		.DependsOn(NugetConfigGenerate)
		.DependsOn(BuildWithGenerator)
		.Executes(() =>
		{
			AzurePipelines?.UpdateBuildNumber(GitVersion.NuGetVersionV2);
		});

	Target NugetConfigGenerate => _ => _
		.Before(BuildWithGenerator)
		.Executes(async () =>
		{
			string nugetConfig =
$@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" protocolVersion=""3"" />
    <add key=""Test Sources"" value=""{NugetDirectory}"" />
</packageSources>
</configuration>
";

			await File.WriteAllTextAsync(NugetConfigFile, nugetConfig);
			EnsureExistingDirectory(NugetDirectory);
		});
}
