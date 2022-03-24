using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreFunctions;
using Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreHttp;
using Beffyman.AspNetCore.Client.Generator.CSharp.SignalR;
using Beffyman.AspNetCore.Client.Generator.Framework;
using Beffyman.AspNetCore.Client.Generator.Json;
using Beffyman.AspNetCore.Client.Generator.Output;
using Microsoft.Build.Utilities;

namespace Beffyman.AspNetCore.Client.Generator
{
	public static class ClientGenerator
	{
		public static bool Generate(GeneratorArgs args, TaskLoggingHelper Log = null)
		{
			Log?.LogCommandLine($">> [{typeof(ClientGenerator).Namespace}] START");

			try
			{
				#region Settings Map

				Settings.RouteToServiceProjectFolder = args.RouteToServiceProjectFolder;
				Settings.ClientInterfaceName = args.ClientInterfaceName;
				Settings.RegisterName = args.RegisterName;

				Settings.UseValueTask = bool.Parse(args.UseValueTask ?? "false");
				Settings.UseInternalClients = bool.Parse(args.UseInternalClients ?? "false");
				Settings.ClientRouteConstraints = bool.Parse(args.ClientRouteConstraints ?? "false");
				Settings.ErrorOnUnhandledCallback = bool.Parse(args.ErrorOnUnhandledCallback ?? "false");
				Settings.MultipleFiles = bool.Parse(args.MultipleFiles ?? "false");
				Settings.GenerateStaticRoutes = bool.Parse(args.GenerateStaticRoutes ?? "false");

				Settings.GenerateClientRepository = bool.Parse(args.GenerateClientRepository ?? "true");
				Settings.GenerateLazyClientRepository = bool.Parse(args.GenerateLazyClientRepository ?? "false");

				Settings.RoutesNamespace = args.RoutesNamespace;
				Settings.ClientNamespace = args.ClientNamespace;
				Settings.HubNamespace = args.HubNamespace;

				Settings.AllowedNamespaces = args.AllowedNamespaces?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				Settings.ExcludedNamespaces = args.ExcludedNamespaces?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

				#endregion


				Directory.SetCurrentDirectory(args.CurrentDirectory);

				Log?.LogCommandLine($"Generating W/ CurrentPath : {args.CurrentDirectory}");

				if (string.IsNullOrWhiteSpace(args.CurrentDirectory))
				{
					Log?.LogError("One of the settings is not filled out.");
					return false;
				}

				//Start out by loading all relevent DLLS
				if (string.IsNullOrEmpty(Settings.RouteToServiceProjectFolder))
				{
					Log?.LogWarning("Service project folder is not provided");
					return false;
				}
				Log?.LogCommandLine(Settings.RouteToServiceProjectFolder);

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

				Log?.LogCommandLine("Client Generation Successful!");
				Log?.LogCommandLine($">> [{typeof(ClientGenerator).Namespace}] END");
				return true;
			}
			catch (Exception ex)
			{
				Log?.LogError(ex.ToString());
				Debugger.Break();
				return false;
			}
		}

	}
}
