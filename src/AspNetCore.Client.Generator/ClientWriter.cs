﻿using AspNetCore.Client.Core;
using AspNetCore.Client.Generator.Data;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator
{
	public static class ClientWriter
	{
		internal static string Namespace
		{
			get
			{
				return $"{Path.GetFileName(Settings.RouteToServiceProjectFolder)}.Clients";
			}
		}

		private static string GetInstaller(IList<ParsedFile> parsedFiles)
		{
			var clients = string.Join(Environment.NewLine, parsedFiles.SelectMany(x => x.Classes.Where(y => y.NotEmpty).Select(y => $@"			services.AddScoped<I{y.ClientName}, {y.ClientName}>();")));

			return $@"
	public static class {Settings.ClientInterfaceName}Installer
	{{
		/// <summary>
		/// Register the autogenerated clients into the container with a lifecycle of scoped.
		/// </summary>
		/// <param name=""services""></param>
		/// <param name=""configure"">Overrides for client configuration</param>
		/// <returns></returns>
		public static IServiceCollection InstallClients(this IServiceCollection services, Action<ClientConfiguration> configure = null)
		{{
			var configuration = new ClientConfiguration();
			configure?.Invoke(configuration);

			services.AddScoped<{Settings.ClientInterfaceName}>((provider) =>
			{{
				var client = provider.GetService<HttpClient>();

				var wrappedClient = new {Settings.ClientInterfaceName}(client);
				wrappedClient.BaseAddress = configuration.HttpBaseAddress;
				wrappedClient.Timeout = configuration.Timeout;
				return wrappedClient;

			}});

{clients}

			return configuration.ApplyConfiguration(services);;
		}}
	}}
";
		}

		private static string GetServiceClients()
		{
			return $@"

	public class {Settings.ClientInterfaceName}
	{{
		public string BaseAddress {{ get; internal set; }}
		public TimeSpan Timeout {{ get; internal set; }}
		public readonly {nameof(FlurlClient)} {Constants.FlurlClientVariable};

		public {Settings.ClientInterfaceName}({nameof(HttpClient)} client)
		{{
			{Constants.FlurlClientVariable} = new {nameof(FlurlClient)}(client);
			if(!string.IsNullOrEmpty(BaseAddress))
			{{
				{Constants.FlurlClientVariable}.BaseUrl = BaseAddress;
			}}
		}}

	}}

	public interface I{Settings.ClientInterfaceName} : {nameof(IClient)} {{ }}";
		}

		public static void WriteClientsFile(IList<ParsedFile> parsedFiles)
		{
			IList<string> requiredUsingStatements = new List<string>
			{
				"using System;",
				"using System.Collections.Generic;",
				"using System.Linq;",
				"using System.Net;",
				"using System.Threading.Tasks;",
				"using System.Threading.Tasks;",
				"using System.Net.Http;",
				"using Flurl.Http;",
				"using Flurl;",
				"using System.Runtime.CompilerServices;",
				"using AspNetCore.Client.Core;",
				"using AspNetCore.Client.Core.Authorization;",
				"using AspNetCore.Client.Core.Exceptions;",
				"using Microsoft.Extensions.DependencyInjection;",
				"using System.Threading;",
				"using AspNetCore.Client.Core.Serializers;",
				"using AspNetCore.Client.Core.Http;"
			};

			var distinctUsingStatements = parsedFiles
											.SelectMany(x => x.UsingStatements)
											.Union(requiredUsingStatements)
											.Distinct()
											.ToArray();

			string usingBlock = string.Join(Environment.NewLine, distinctUsingStatements);

			string str =
$@"//------------------------------------------------------------------------------
// <auto-generated>
//		This code was generated from a template.
//		Manual changes to this file may cause unexpected behavior in your application.
//		Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

{usingBlock}

namespace {Settings.ClientNamespace}
{{

{GetInstaller(parsedFiles)}
{GetServiceClients()}
{string.Join(Environment.NewLine, parsedFiles.SelectMany(x => x.Classes).Where(x => x.NotEmpty).Select(x => x.GetText()))}
}}
";

			Helpers.SafelyWriteToFile($"{Environment.CurrentDirectory}/Clients.cs", str);

		}

	}
}
