using AspNetCore.Client.Generator;
using Microsoft.Build.Framework;
using Moq;
using System;
using System.Reflection;

namespace AspNetCore.Client.Test.Generator
{
	public static class Program
	{
		const string TESTWEBAPPCLIENT_PATH = "../../../../TestWebApp.Clients";
		const string TESTBLAZOR_PATH = "../../../../TestBlazorApp.Clients";

		static void Main()
		{
			if (!(GenerateAspNetCore() && GenerateBlazor()))
			{
				Console.ReadKey();
			}
		}


		private static bool GenerateAspNetCore()
		{
			var previousWorkDir = Environment.CurrentDirectory;
			var task = new GeneratorTask();

			task.ProjectPath = TESTWEBAPPCLIENT_PATH;
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

		private static bool GenerateBlazor()
		{
			var previousWorkDir = Environment.CurrentDirectory;
			var task = new GeneratorTask();

			task.ProjectPath = TESTBLAZOR_PATH;
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
