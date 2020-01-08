using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreFunctions;
using Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreHttp;
using Beffyman.AspNetCore.Client.Generator.CSharp.SignalR;
using Beffyman.AspNetCore.Client.Generator.Framework;
using Beffyman.AspNetCore.Client.Generator.Json;
using Beffyman.AspNetCore.Client.Generator.Output;
using Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Functions;

namespace Beffyman.AspNetCore.Client.Generator
{
	public class GeneratorTask : ContextIsolatedTask
	{
		public string CurrentDirectory { get; set; }
		public string RouteToServiceProjectFolder { get; set; }
		public string ClientInterfaceName { get; set; }
		public string RegisterName { get; set; }
		public string UseValueTask { get; set; }
		public string UseInternalClients { get; set; }
		public string ClientRouteConstraints { get; set; }
		public string ErrorOnUnhandledCallback { get; set; }
		public string MultipleFiles { get; set; }
		public string GenerateStaticRoutes { get; set; }
		public string RoutesNamespace { get; set; }
		public string ClientNamespace { get; set; }
		public string HubNamespace { get; set; }
		public string AllowedNamespaces { get; set; }
		public string ExcludedNamespaces { get; set; }

		public void Fill(IDictionary<string, string> properties)
		{
			RouteToServiceProjectFolder = properties.GetValueOrDefault(nameof(RouteToServiceProjectFolder));
			ClientInterfaceName = properties.GetValueOrDefault(nameof(ClientInterfaceName));
			RegisterName = properties.GetValueOrDefault(nameof(RegisterName));
			UseValueTask = properties.GetValueOrDefault(nameof(UseValueTask));
			UseInternalClients = properties.GetValueOrDefault(nameof(UseInternalClients));
			ClientRouteConstraints = properties.GetValueOrDefault(nameof(ClientRouteConstraints));
			ErrorOnUnhandledCallback = properties.GetValueOrDefault(nameof(ErrorOnUnhandledCallback));
			MultipleFiles = properties.GetValueOrDefault(nameof(MultipleFiles));
			GenerateStaticRoutes = properties.GetValueOrDefault(nameof(GenerateStaticRoutes));
			RoutesNamespace = properties.GetValueOrDefault(nameof(RoutesNamespace));
			ClientNamespace = properties.GetValueOrDefault(nameof(ClientNamespace));
			HubNamespace = properties.GetValueOrDefault(nameof(HubNamespace));
			AllowedNamespaces = properties.GetValueOrDefault(nameof(AllowedNamespaces));
			ExcludedNamespaces = properties.GetValueOrDefault(nameof(ExcludedNamespaces));
		}

		public bool ByPassExecute()
		{

			return ExecuteIsolated();
		}

		protected override bool ExecuteIsolated()
		{
			Log.LogCommandLine($">> [{typeof(GeneratorTask).Namespace}] START");

#if !DEBUG
			try
			{
			`
#endif
			#region Settings Map

			Settings.RouteToServiceProjectFolder = RouteToServiceProjectFolder;
			Settings.ClientInterfaceName = ClientInterfaceName;
			Settings.RegisterName = RegisterName;
			Settings.UseValueTask = bool.Parse(UseValueTask ?? "false");
			Settings.UseInternalClients = bool.Parse(UseInternalClients ?? "false");
			Settings.ClientRouteConstraints = bool.Parse(ClientRouteConstraints ?? "false");
			Settings.ErrorOnUnhandledCallback = bool.Parse(ErrorOnUnhandledCallback ?? "false");
			Settings.MultipleFiles = bool.Parse(MultipleFiles ?? "false");
			Settings.GenerateStaticRoutes = bool.Parse(GenerateStaticRoutes ?? "false");
			Settings.RoutesNamespace = RoutesNamespace;
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

			var parsedControllers = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.RouteToServiceProjectFolder}", "*.cs", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(cs => new HttpControllerCSharpFile(cs))
									.ToList();


			var parsedHubs = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.RouteToServiceProjectFolder}", "*.cs", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(cs => new HubCSharpFile(cs))
									.ToList();

			var hostFile = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.RouteToServiceProjectFolder}", "host.json", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(jn => new HostJsonFile(jn))
									.SingleOrDefault();

			var parsedFunctions = Directory.EnumerateFiles($"{Environment.CurrentDirectory}/{Settings.RouteToServiceProjectFolder}", "*.cs", SearchOption.AllDirectories)
									.Where(x => !x.Contains("/obj/") && !x.Contains("\\obj\\")
											&& !x.Contains("/bin/") && !x.Contains("\\bin\\"))
									.Select(cs => new FunctionsCSharpFile(cs, hostFile?.Data))
									.ToList();

			var context = new GenerationContext();
			foreach (var file in parsedControllers)
			{
				context = context.Merge(file.Context);
			}
			foreach (var file in parsedHubs)
			{
				context = context.Merge(file.Context);
			}
			foreach (var file in parsedFunctions)
			{
				context = context.Merge(file.Context);
			}
			context.MapRelatedInfo();
			context.Validate(Settings.AllowedNamespaces, Settings.ExcludedNamespaces);

			ClassWriter.WriteClientsFile(context);

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
