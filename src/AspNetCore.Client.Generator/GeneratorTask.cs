using AspNetCore.Client.Generator.Data;
using System;
using System.IO;
using System.Linq;

namespace AspNetCore.Client.Generator
{
	public class GeneratorTask : Microsoft.Build.Utilities.Task
	{
		public string ProjectPath { get; set; }

		public override bool Execute()
		{
			Log.LogCommandLine($">> [{typeof(GeneratorTask).Namespace}] START");

#if !DEBUG
			try
			{

#endif
			Log.LogCommandLine($">> NETSTANDARD1_5");
#if !DEBUG

#endif

			Directory.SetCurrentDirectory(ProjectPath);

			Log.LogCommandLine($"Generating W/ CurrentPath : {ProjectPath}");

			if (string.IsNullOrWhiteSpace(ProjectPath))
			{
				Log.LogError("One of the settings is not filled out.");
				return false;
			}

			Settings.Load();
			Settings.Save();

			if (Settings.Instance.Locked)
			{
				Log.LogWarning("Client Generation Locked!");
				return true;
			}

			//Start out by loading all relevent DLLS
			if (string.IsNullOrEmpty(Settings.Instance.RelativeRouteToServiceProjectFolder))
			{
				Settings.Save();
				Log.LogWarning("Service project folder is not provided");
				return false;
			}
			Log.LogCommandLine(Settings.Instance.RelativeRouteToServiceProjectFolder);

			//Start out by loading all cs files into memory

			var parsedFiles = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.Instance.RelativeRouteToServiceProjectFolder}", "*Controller.cs", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(cs => new ParsedFile(cs))
									.ToList();

			ClientWriter.WriteClientsFile(parsedFiles);

			Settings.Save();

			Log.LogCommandLine("Client Generation Successful!");
			Log.LogCommandLine($">> [{typeof(GeneratorTask).Namespace}] END");
			return true;
#if !DEBUG
			}
			catch (Exception ex)
			{
				Log.LogError(ex.ToString());
				return false;
			}
#endif
		}
	}
}
