using AspNetCore.Client.Generator;
using Microsoft.Build.Framework;
using Moq;
using System;
using System.Reflection;
using System.IO;
using System.Linq;

namespace AspNetCore.Client.Test.Generator
{
	public static class Program
	{
		const string WEBAPP = "TestWebApp.Clients";
		const string BLAZOR = "TestBlazorApp.Clients";
		const string FAILURE_DIR = "AspNetCore.Client";

		static void Main()
		{
			var webApp = GoUpUntilDirectory(WEBAPP, FAILURE_DIR);
			var blazor = GoUpUntilDirectory(BLAZOR, FAILURE_DIR);

			if (!(Generate(webApp) && Generate(blazor)))
			{
				Console.ReadKey();
			}
		}

		private static string GoUpUntilDirectory(string targetDirectoryName, string failDirectory)
		{
			string currentPath = System.Environment.CurrentDirectory;

			while (Path.GetFileName(currentPath) != failDirectory)
			{
				var childDirectories = Directory.GetDirectories(currentPath).ToList();
				var dirs = childDirectories.Select(Path.GetFileName).ToList();
				if (!dirs.Contains(targetDirectoryName))
				{
					currentPath = Path.GetFullPath("..", currentPath);
				}
				else
				{
					return childDirectories.SingleOrDefault(x => Path.GetFileName(x) == targetDirectoryName);
				}
			}

			throw new DirectoryNotFoundException($"Directory {targetDirectoryName} was not found.");
		}


		private static bool Generate(string path)
		{
			var previousWorkDir = Environment.CurrentDirectory;
			var task = new GeneratorTask();

			task.ProjectPath = path;
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

			var success = task.Execute();
			Environment.CurrentDirectory = previousWorkDir;
			return success;
		}

	}
}
