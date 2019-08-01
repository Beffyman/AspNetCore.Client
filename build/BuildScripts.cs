using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations(TimeoutInMilliseconds = 5000)]
[UnsetVisualStudioEnvironmentVariables]
public class BuildScripts : NukeBuild
{
	//#error need to update blazor tests to use netcoreapp3.0
	//#error need to update README

	public static int Main() => Execute<BuildScripts>(x => x.Build);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution] readonly Solution Solution;
	[GitRepository] readonly GitRepository GitRepository;
	[GitVersion] readonly GitVersion GitVersion;

	const string SourceFolder = "src";
	const string TestsFolder = "tests";

	AbsolutePath SourceDirectory => RootDirectory / SourceFolder;
	AbsolutePath TestsDirectory => RootDirectory / TestsFolder;
	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
	AbsolutePath TestArtifactsDirectory => ArtifactsDirectory / "tests";
	AbsolutePath NugetDirectory => ArtifactsDirectory / "nuget";
	AbsolutePath CodeCoverageReportOutput => TestArtifactsDirectory / "Reports";
	AbsolutePath CodeCoverageFile => TestArtifactsDirectory / "coverage.cobertura.xml";


	RelativePath TestGeneratorProject => (RelativePath)TestsFolder / "Beffyman.AspNetCore.Client.Test.Generator" / "Beffyman.AspNetCore.Client.Test.Generator.csproj";
	RelativePath GeneratorProject => (RelativePath)SourceFolder / "Beffyman.AspNetCore.Client.Generator" / "Beffyman.AspNetCore.Client.Generator.csproj";

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
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
				.SetFileVersion(GitVersion.GetNormalizedFileVersion())
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.SetVerbosity(DotNetVerbosity.Detailed)
				.EnableNoRestore());
		});

	private void RunTests()
	{
		DotNetTest(s => s.EnableNoBuild()
				.SetConfiguration(Configuration)
				.EnableNoBuild()
				.EnableNoRestore()
				.SetLogger("trx")
				.SetResultsDirectory(TestArtifactsDirectory)
				.SetLogOutput(true)
				.SetArgumentConfigurator(arguments => arguments.Add("/p:CollectCoverage={0}", "true")
					.Add("/p:CoverletOutput={0}/", TestArtifactsDirectory)
					//.Add("/p:Threshold={0}", 90)
					.Add("/p:Exclude=\"[xunit*]*%2c[*.Tests]*\"")
					.Add("/p:UseSourceLink={0}", "true")
					.Add("/p:CoverletOutputFormat={0}", "cobertura"))
				.SetProjectFile(Solution));

		FileExists(CodeCoverageFile);
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
			DotNetPack(s => s.SetProject(Solution)
					.SetVersion(GitVersion.NuGetVersionV2)
					//.EnableNoBuild()
					.EnableIncludeSource()
					.EnableIncludeSymbols()
					.SetConfiguration(Configuration)
					.SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
					.SetFileVersion(GitVersion.GetNormalizedFileVersion())
					.SetInformationalVersion(GitVersion.InformationalVersion)
					.SetOutputDirectory(NugetDirectory));
		});

	Target GenerateTestProjectClients => _ => _
		.DependsOn(Build)
		.Before(Test)
		.Executes(() =>
		{
			DotNetRun(s => s.SetProjectFile(TestGeneratorProject)
							.SetConfiguration(Configuration)
							.SetFramework("netcoreapp2.2")
							.EnableNoBuild());
		});

	Target BuildWithGenerator => _ => _
		.After(Pack)
		.Executes(() =>
		{
			CleanArtifacts(false);

			DotNet($"sln remove {TestGeneratorProject}");
			DotNet($"sln remove {GeneratorProject}");

			DotNetBuild(s => s
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
				.SetFileVersion(GitVersion.GetNormalizedFileVersion())
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.AddProperty("GenerateWithNuget", "true")
				.AddProperty("GeneratorVersion", GitVersion.NuGetVersionV2)
				.AddSources(NugetDirectory));

			RunTests();

			DotNet($"sln add {GeneratorProject}");
			DotNet($"sln add {TestGeneratorProject}");

		});

	Target CI => _ => _
		.DependsOn(Clean)
		.DependsOn(Build)
		.DependsOn(GenerateTestProjectClients)
		.DependsOn(Test)
		.DependsOn(Pack)
		.DependsOn(BuildWithGenerator)
		.Executes(() => { });


}
