using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Beffyman.AspNetCore.Client.Generator;
using Microsoft.Build.Framework;
using Moq;

namespace Beffyman.AspNetCore.Client.Test.Generator
{
	/// <summary>
	/// This project is used to run the generator locally on all the test projects
	/// </summary>
	public static class Program
	{
		public static List<string> Projects = new List<string>
		{
			"tests/AspNetCore3.1/TestWebApp.Clients/TestWebApp.Clients.csproj",
			"tests/AspNetCore6.0/TestWebApp.Clients/TestWebApp.Clients.csproj",
			"tests/Blazor3.1/TestBlazorApp.Clients/TestBlazorApp.Clients.csproj",
			"tests/Functions/FunctionApp2.Clients/FunctionApp2.Clients.csproj",
			"tests/Functions/TestAzureFunction.Clients/TestAzureFunction.Clients.csproj",
		};

		const string FAILURE_DIR = "AspNetCore.Client";

		static void Main()
		{
			string rootDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
			Console.WriteLine($"Root Dir: {rootDir}");
			Directory.SetCurrentDirectory(rootDir);

			Console.WriteLine("Starting Generator");

			foreach (var path in Projects)
			{
				var fullPath = GoUpUntilFile(path, FAILURE_DIR);

				if (!Generate(fullPath))
				{
					if (Debugger.IsAttached)
					{
						Console.ReadKey();
					}
				}
			}
		}

		private static string GoUpUntilFile(string targetFile, string failDirectory)
		{
			string currentPath = Path.GetDirectoryName(Environment.ProcessPath);
			bool notAtRootDir = true;

			while (notAtRootDir)
			{
				if (Path.GetFileName(currentPath) == failDirectory)
				{
					notAtRootDir = false;
				}

				var targetPath = Path.Combine(currentPath, targetFile);

				if (File.Exists(targetPath))
				{
					return Path.GetFullPath(targetPath);
				}
				else
				{
					currentPath = Path.GetFullPath($"{currentPath}/..");
				}
			}

			throw new DirectoryNotFoundException($"Relative Project File [{targetFile}] was not found.");
		}


		private static bool Generate(string csprojPath)
		{
			Console.WriteLine($"Generating {csprojPath}");
			var projectName = Path.GetFileName(csprojPath);
			var data = XElement.Load(csprojPath);
			var parentDirectory = Directory.GetParent(csprojPath).FullName;

			IDictionary<string, string> properties = data.Elements("PropertyGroup")
				.Descendants()
				.ToDictionary(x => x.Name.ToString(), y => y.Value);

			var previousWorkDir = Environment.CurrentDirectory;
			var task = new GeneratorTask();
			task.Fill(properties);
			task.CurrentDirectory = parentDirectory;


			var mockedBuildEngine = new Mock<IBuildEngine>();
			mockedBuildEngine.Setup(x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>())).Callback((BuildErrorEventArgs args) =>
			{
				Console.Error.WriteLine(args.Message);
				throw new Exception(args.Message);
			});
			mockedBuildEngine.Setup(x => x.LogMessageEvent(It.IsAny<BuildMessageEventArgs>())).Callback((BuildMessageEventArgs args) =>
			{
				Console.WriteLine(args.Message);
			});
			mockedBuildEngine.Setup(x => x.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Callback((BuildWarningEventArgs args) =>
			{
				Console.WriteLine(args.Message);
			});

			task.BuildEngine = mockedBuildEngine.Object;

			var success = task.ByPassExecute();
			Environment.CurrentDirectory = previousWorkDir;
			return success;
		}

	}
}
