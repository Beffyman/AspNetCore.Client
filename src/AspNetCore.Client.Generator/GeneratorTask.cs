using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.CSharp.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetCore.Client.Generator.Output;
using AspNetCore.Client.Generator.CSharp.SignalR;

namespace AspNetCore.Client.Generator
{
	public class GeneratorTask :
#if NET462
		Microsoft.Build.Utilities.Task
#endif

#if NETSTANDARD2_0
		ContextIsolatedTask
#endif
	{
		public string CurrentDirectory { get; set; }
		public string RouteToServiceProjectFolder { get; set; }
		public string ClientInterfaceName { get; set; }
		public string RegisterName { get; set; }
		public string UseValueTask { get; set; }
		public string UseInternalClients { get; set; }
		public string ClientNamespace { get; set; }
		public string HubNamespace { get; set; }
		public string AllowedNamespaces { get; set; }
		public string ExcludedNamespaces { get; set; }

		public void Fill(IDictionary<string, string> properties)
		{
			RouteToServiceProjectFolder = properties[nameof(RouteToServiceProjectFolder)];
			ClientInterfaceName = properties[nameof(ClientInterfaceName)];
			RegisterName = properties[nameof(RegisterName)];
			UseValueTask = properties[nameof(UseValueTask)];
			UseInternalClients = properties[nameof(UseInternalClients)];
			ClientNamespace = properties[nameof(ClientNamespace)];
			HubNamespace = properties[nameof(HubNamespace)];
			AllowedNamespaces = properties[nameof(AllowedNamespaces)];
			ExcludedNamespaces = properties[nameof(ExcludedNamespaces)];
		}

		public bool ByPassExecute()
		{
#if NET462
			return Execute();
#endif

#if NETSTANDARD2_0
			return ExecuteIsolated();
#endif

		}

#if NET462
		public override bool Execute()
#endif

#if NETSTANDARD2_0
		protected override bool ExecuteIsolated()
#endif

		{
			Log.LogCommandLine($">> [{typeof(GeneratorTask).Namespace}] START");

#if !DEBUG
			try
			{

#endif
			#region Settings Map

			Settings.RouteToServiceProjectFolder = RouteToServiceProjectFolder;
			Settings.ClientInterfaceName = ClientInterfaceName;
			Settings.RegisterName = RegisterName;
			Settings.UseValueTask = bool.Parse(UseValueTask ?? "false");
			Settings.UseInternalClients = bool.Parse(UseInternalClients ?? "false");
			Settings.ClientNamespace = ClientNamespace;
			Settings.HubNamespace = HubNamespace;
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

			var parsedControllers = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.RouteToServiceProjectFolder}", "*Controller.cs", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(cs => new ClientCSharpFile(cs))
									.ToList();


			var parsedHubs = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.RouteToServiceProjectFolder}", "*Hub.cs", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(cs => new HubCSharpFile(cs))
									.ToList();

			ClassWriter.WriteClientsFile(parsedControllers, parsedHubs);

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
