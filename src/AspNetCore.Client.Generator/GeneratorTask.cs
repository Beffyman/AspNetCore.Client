using AspNetCore.Client.Generator.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspNetCore.Client.Generator
{
	public class GeneratorTask : Microsoft.Build.Utilities.Task
	{
		public string CurrentDirectory { get; set; }
		public string RouteToServiceProjectFolder { get; set; }
		public string ClientInterfaceName { get; set; }
		public string UseValueTask { get; set; }
		public string ClientNamespace { get; set; }
		public string AllowedNamespaces { get; set; }
		public string ExcludedNamespaces { get; set; }

		public void Fill(IDictionary<string,string> properties)
		{
			RouteToServiceProjectFolder = properties[nameof(RouteToServiceProjectFolder)];
			ClientInterfaceName = properties[nameof(ClientInterfaceName)];
			ClientInterfaceName = properties[nameof(ClientInterfaceName)];
			UseValueTask = properties[nameof(UseValueTask)];
			ClientNamespace = properties[nameof(ClientNamespace)];
			AllowedNamespaces = properties[nameof(AllowedNamespaces)];
			ExcludedNamespaces = properties[nameof(ExcludedNamespaces)];
		}


		public override bool Execute()
		{
			Log.LogCommandLine($">> [{typeof(GeneratorTask).Namespace}] START");

#if !DEBUG
			try
			{

#endif
			#region Settings Map

			Settings.RouteToServiceProjectFolder = RouteToServiceProjectFolder;
			Settings.ClientInterfaceName = ClientInterfaceName;
			Settings.UseValueTask = bool.Parse(UseValueTask ?? "false");
			Settings.ClientNamespace = ClientNamespace;
			Settings.AllowedNamespaces = AllowedNamespaces?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			Settings.ExcludedNamespaces = ExcludedNamespaces?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			#endregion


			Directory.SetCurrentDirectory(CurrentDirectory);

			Log.LogCommandLine($"Generating W/ CurrentPath : {CurrentDirectory}");

			if (string.IsNullOrWhiteSpace(CurrentDirectory))
			{
				Log.LogError("One of the settings is not filled out.");
				return false;
			}

			//Start out by loading all relevent DLLS
			if (string.IsNullOrEmpty(Settings.RouteToServiceProjectFolder))
			{
				Log.LogWarning("Service project folder is not provided");
				return false;
			}
			Log.LogCommandLine(Settings.RouteToServiceProjectFolder);

			//Start out by loading all cs files into memory

			var parsedFiles = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.RouteToServiceProjectFolder}", "*Controller.cs", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(cs => new ParsedFile(cs))
									.ToList();

			ClientWriter.WriteClientsFile(parsedFiles);

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
