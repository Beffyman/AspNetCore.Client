﻿using AspNetCore.Client;
using AspNetCore.Client.Generator.CSharp;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
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

		private static string GetInstaller(IList<ParsedFile> correctFiles)
		{
			var clients = string.Join(Environment.NewLine, correctFiles.SelectMany(x => x.Classes.Where(y => y.NotEmpty).Select(y => $@"			services.AddScoped<{(y.NamespaceVersion != null ? $"{y.NamespaceVersion}." : "")}{(y.Options.NamespaceSuffix != null ? $"{y.Options.NamespaceSuffix}." : string.Empty)}I{y.ClientName}, {(y.NamespaceVersion != null ? $"{y.NamespaceVersion}." : "")}{(y.Options.NamespaceSuffix != null ? $"{y.Options.NamespaceSuffix}." : string.Empty)}{y.ClientName}>();")));

			var repositories = correctFiles.SelectMany(x => x.Classes)
												.Where(x => x.NotEmpty)
												.GroupBy(x => x.NamespaceVersion)
												.Select(x => $"			services.AddScoped<I{Settings.ClientInterfaceName}{x.Key}Repository,{Settings.ClientInterfaceName}{x.Key}Repository>();")
												.ToList();

			var repoList = string.Join($@"			{Environment.NewLine}", repositories);

			return $@"
	public static class {Settings.ClientInterfaceName}Installer
	{{
		/// <summary>
		/// Register the autogenerated clients into the container with a lifecycle of scoped.
		/// </summary>
		/// <param name=""services""></param>
		/// <param name=""configure"">Overrides for client configuration</param>
		/// <returns></returns>
		public static {nameof(IServiceCollection)} InstallClients(this {nameof(IServiceCollection)} services, Action<{nameof(ClientConfiguration)}> configure)
		{{
			var configuration = new {nameof(ClientConfiguration)}();

			configuration.{nameof(ClientConfiguration.RegisterClientWrapperCreator)}({Settings.ClientInterfaceName}Wrapper.Create);
			configuration.{nameof(ClientConfiguration.UseClientWrapper)}<I{Settings.ClientInterfaceName}Wrapper, {Settings.ClientInterfaceName}Wrapper>((provider) => new {Settings.ClientInterfaceName}Wrapper(provider.GetService<HttpClient>(), configuration.{nameof(ClientConfiguration.GetSettings)}()));

			configure?.Invoke(configuration);

{string.Join(Environment.NewLine, repositories)}
{clients}

			return configuration.{nameof(ClientConfiguration.ApplyConfiguration)}(services);;
		}}
	}}
";
		}

		private static string GetServiceClients()
		{
			return $@"


	public interface I{Settings.ClientInterfaceName}Wrapper : IClientWrapper {{ }}

	public class {Settings.ClientInterfaceName}Wrapper :  I{Settings.ClientInterfaceName}Wrapper
	{{
		public TimeSpan Timeout {{ get; internal set; }}
		public {nameof(FlurlClient)} {Constants.FlurlClientVariable} {{ get; internal set; }}

		public {Settings.ClientInterfaceName}Wrapper({nameof(HttpClient)} client, {nameof(ClientSettings)} settings)
		{{
			if (!string.IsNullOrEmpty(settings.{nameof(ClientSettings.BaseAddress)}))
			{{
				client.BaseAddress = new Uri(settings.{nameof(ClientSettings.BaseAddress)});
			}}
			{Constants.FlurlClientVariable} = new {nameof(FlurlClient)}(client);
			Timeout = settings.{nameof(ClientSettings.Timeout)};
		}}

		public static I{Settings.ClientInterfaceName}Wrapper Create(HttpClient client, {nameof(ClientSettings)} settings)
		{{
			return new {Settings.ClientInterfaceName}Wrapper(client, settings);
		}}
	}}

	public interface I{Settings.ClientInterfaceName} : {nameof(IClient)} {{ }}";
		}


		private static string GetErrorMessages(IList<ParsedFile> errorFiles)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var errorFile in errorFiles)
			{
				sb.AppendLine($"#warning {(errorFile.UnexpectedFailure ? "PLEASE MAKE A GITHUB REPO ISSUE" : "")} File {Path.GetFullPath(errorFile.FileName)} {(errorFile.UnexpectedFailure ? "has failed generation withunexpected error" : "is misconfigured for generation")} :: {errorFile.Error.Replace('\r', ' ').Replace('\n', ' ')}");
			}

			return sb.ToString();
		}

		private static string GetRepositoryModels(IList<ParsedFile> correctFiles)
		{
			var versionedClients = correctFiles.SelectMany(x => x.Classes)
												.Where(x => x.NotEmpty)
												.GroupBy(x => x.NamespaceVersion)
												.ToList();

			StringBuilder sb = new StringBuilder();


			foreach (var repo in versionedClients)
			{
				var interfaceProperties = repo.Select(x => $@"		{repo.Key}{(repo.Key != null ? "." : "")}{(x.Options.NamespaceSuffix != null ? $"{x.Options.NamespaceSuffix}." : string.Empty)}I{x.ClientName} {x.ControllerName} {{ get; }}");
				var implementationProperties = repo.Select(x => $@"		public {repo.Key}{(repo.Key != null ? "." : "")}{(x.Options.NamespaceSuffix != null ? $"{x.Options.NamespaceSuffix}." : string.Empty)}I{x.ClientName} {x.ControllerName} {{ get; private set; }}");
				var implementationParameters = repo.Select(x => $@"			{repo.Key}{(repo.Key != null ? "." : "")}{(x.Options.NamespaceSuffix != null ? $"{x.Options.NamespaceSuffix}." : string.Empty)}I{x.ClientName} {x.ControllerName.ToLower()}");
				var implementationAssignment = repo.Select(x => $@"			this.{x.ControllerName} = {x.ControllerName.ToLower()};");


				sb.AppendLine(
$@"
	public interface I{Settings.ClientInterfaceName}{repo.Key}Repository
	{{
{string.Join(Environment.NewLine, interfaceProperties)}
	}}

	{(Settings.UseInternalClients ? "internal" : "public")} class {Settings.ClientInterfaceName}{repo.Key}Repository : I{Settings.ClientInterfaceName}{repo.Key}Repository
	{{
{string.Join(Environment.NewLine, implementationProperties)}

		public {Settings.ClientInterfaceName}{repo.Key}Repository
		(
{string.Join($",{Environment.NewLine}", implementationParameters)}
		)
		{{
{string.Join(Environment.NewLine, implementationAssignment)}
		}}
	}}
");

			}


			return sb.ToString();
		}


		public static void WriteClientsFile(IList<ParsedFile> parsedFiles)
		{
			IList<string> requiredUsingStatements = new List<string>
			{
				@"using AspNetCore.Client;",
				"using AspNetCore.Client.Authorization;",
				"using AspNetCore.Client.Exceptions;",
				"using AspNetCore.Client.Http;",
				"using AspNetCore.Client.RequestModifiers;",
				"using AspNetCore.Client.Serializers;",
				"using Flurl.Http;",
				"using Microsoft.Extensions.DependencyInjection;",
				"using System;",
				"using System.Linq;",
				"using System.Collections.Generic;",
				"using System.Net;",
				"using System.Net.Http;",
				"using System.Runtime.CompilerServices;",
				"using System.Threading;",
				"using System.Threading.Tasks;"
			};


			var errorFiles = parsedFiles.Where(x => x.Failed).ToList();
			var correctFiles = parsedFiles.Where(x => !x.Failed).ToList();

			var distinctUsingStatements = correctFiles
											.SelectMany(x => x.UsingStatements)
											.Union(requiredUsingStatements)
											.Distinct()
											.ToArray();

			string usingBlock = string.Join(Environment.NewLine, distinctUsingStatements);


			var versionedClients = correctFiles.SelectMany(x => x.Classes)
												.Where(x => x.NotEmpty)
												.GroupBy(x => x.NamespaceVersion)
												.ToList();

			StringBuilder versionBlocks = new StringBuilder();

			foreach (var version in versionedClients)
			{
				versionBlocks.AppendLine(
$@"
namespace {Settings.ClientNamespace}{(version.Key != null ? "." : "")}{version.Key}
{{
{string.Join(Environment.NewLine, version.Select(x => x.GetText()))}
}}
");
			}

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

{GetErrorMessages(errorFiles)}
{GetInstaller(correctFiles)}
{GetServiceClients()}

{GetRepositoryModels(correctFiles)}
}}

{string.Join(Environment.NewLine, versionBlocks)}
";

			Helpers.SafelyWriteToFile($"{Environment.CurrentDirectory}/Clients.cs", str);

		}

	}
}
