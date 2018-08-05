﻿using AspNetCore.Client.Generator.CSharp;
using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Framework.AttributeInterfaces;
using AspNetCore.Client.Generator.Framework.Dependencies;
using AspNetCore.Client.Generator.Framework.Parameters;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Client.Generator.Framework.ResponseTypes;
using AspNetCore.Client.Generator.Framework.Routes.Constraints;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Temp
{
	public static class ClassWriter
	{
		public static void WriteClientsFile(IList<ParsedFile> parsedFiles)
		{
			var str = WriteFile(parsedFiles);

			Helpers.SafelyWriteToFile($"{Environment.CurrentDirectory}/Clients.cs", str);

		}


		private static string WriteFile(IEnumerable<ParsedFile> files)
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


			var errorFiles = files.Where(x => x.Failed).ToList();
			var correctFiles = files.Where(x => !x.Failed).ToList();

			var distinctUsingStatements = correctFiles
											.SelectMany(x => x.UsingStatements)
											.Union(requiredUsingStatements)
											.Distinct()
											.ToArray();

			var context = new GenerationContext();
			foreach (var file in correctFiles)
			{
				context = context.Merge(file.Context);
			}

			return
$@"//------------------------------------------------------------------------------
// <auto-generated>
//		This code was generated from a template.
//		Manual changes to this file may cause unexpected behavior in your application.
//		Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

{string.Join(Environment.NewLine, distinctUsingStatements)}

namespace {Settings.ClientNamespace}
{{

{string.Join(Environment.NewLine, errorFiles.Select(WriteErrorMessage))}

{WriteInstaller(context)}

{WriteBaseClients()}

{WriteRepositories(context)}

}}


{WriteVersionBlocks(context)}
";
		}

		private static string WriteErrorMessage(ParsedFile file)
		{
			return $@"#warning {(file.UnexpectedFailure ? "PLEASE MAKE A GITHUB REPO ISSUE" : "")} File {Path.GetFullPath(file.FileName)} {(file.UnexpectedFailure ? "has failed generation withunexpected error" : "is misconfigured for generation")} :: {file.Error.Replace('\r', ' ').Replace('\n', ' ')}";
		}

		#region Installer

		private static string WriteInstaller(GenerationContext context)
		{
			var versions = context.Clients.Where(x => !x.Ignored)
				.GroupBy(x => x.NamespaceVersion)
				.Select(x => x.Key)
				.ToList();

			return
$@"
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
			configuration.{nameof(ClientConfiguration.UseClientWrapper)}<I{Settings.ClientInterfaceName}Wrapper, {Settings.ClientInterfaceName}Wrapper>((provider) => new {Settings.ClientInterfaceName}Wrapper(provider.GetService<{nameof(HttpClient)}>(), configuration.{nameof(ClientConfiguration.GetSettings)}()));

			configure?.Invoke(configuration);

{string.Join(Environment.NewLine, versions.Select(WriteRepositoryRegistration))}
{string.Join(Environment.NewLine, context.Clients.Select(WriteClientRegistration))}

			return configuration.{nameof(ClientConfiguration.ApplyConfiguration)}(services);;
		}}
	}}
";
		}

		private static string WriteRepositoryRegistration(string version)
		{
			return $@"services.AddScoped<I{Settings.ClientInterfaceName}{version}Repository,{Settings.ClientInterfaceName}{version}Repository>();";
		}

		private static string WriteClientRegistration(Controller controller)
		{
			string namespaceVersion = $@"{(controller.NamespaceVersion != null ? $"{controller.NamespaceVersion}." : "")}{(controller.NamespaceSuffix != null ? $"{controller.NamespaceSuffix}." : string.Empty)}";
			string interfaceName = $@"{namespaceVersion}I{controller.Name}";
			string implementationName = $@"{namespaceVersion}{controller.Name}";
			return $@"services.AddScoped<{interfaceName}, {implementationName}>();";
		}


		#endregion Installer


		private static string WriteBaseClients()
		{
			return
$@"
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

	public interface I{Settings.ClientInterfaceName} : {nameof(IClient)} {{ }}
";
		}


		#region Repository

		private static string WriteRepositories(GenerationContext context)
		{
			var versions = context.Clients.Where(x => !x.Ignored)
				.GroupBy(x => x.NamespaceVersion)
				.ToList();


			return
$@"

{string.Join(Environment.NewLine, versions.Select(WriteRepository))}

";
		}

		private static string WriteRepository(IGrouping<string, Controller> version)
		{


			return
$@"
	public interface I{Settings.ClientInterfaceName}{version.Key}Repository
	{{
{string.Join($@"{Environment.NewLine}", version.Select(x => WriteRepositoryInterfaceProperty(version.Key, x)))}
	}}

	{(Settings.UseInternalClients ? "internal" : "public")} class {Settings.ClientInterfaceName}{version.Key}Repository : I{Settings.ClientInterfaceName}{version.Key}Repository
	{{
{string.Join($@"{Environment.NewLine}", version.Select(x => WriteRepositoryProperty(version.Key, x)))}

		public {Settings.ClientInterfaceName}{version.Key}Repository
		(
{string.Join($@",{Environment.NewLine}", version.Select(x => WriteRepositoryParameter(version.Key, x)))}
		)
		{{
{string.Join($@"{Environment.NewLine}", version.Select(x => WriteRepositoryAssignment(version.Key, x)))}
		}}
	}}

";
		}


		private static string WriteRepositoryInterfaceProperty(string key, Controller controller)
		{
			return $@"{key}{(key != null ? "." : "")}{(controller.NamespaceSuffix != null ? $"{controller.NamespaceSuffix}." : string.Empty)}I{controller.Name} {controller.Name} {{ get; }}";
		}

		private static string WriteRepositoryProperty(string key, Controller controller)
		{
			return $@"public {key}{(key != null ? "." : "")}{(controller.NamespaceSuffix != null ? $"{controller.NamespaceSuffix}." : string.Empty)}I{controller.Name} {controller.Name} {{ get; }}";
		}

		private static string WriteRepositoryParameter(string key, Controller controller)
		{
			return $@"{key}{(key != null ? "." : "")}{(controller.NamespaceSuffix != null ? $"{controller.NamespaceSuffix}." : string.Empty)}I{controller.Name} param_{controller.Name.ToLower()}";
		}

		private static string WriteRepositoryAssignment(string key, Controller controller)
		{
			return $@"this.{controller.Name} = param_{controller.Name.ToLower()};";
		}


		#endregion Repository

		#region Version Blocks

		private static string WriteVersionBlocks(GenerationContext context)
		{
			var versions = context.Clients.Where(x => !x.Ignored)
				.GroupBy(x => x.NamespaceVersion)
				.ToList();

			return
$@"

{string.Join(Environment.NewLine, versions.Select(WriteVersionGroup))}

";

		}

		private static string WriteVersionGroup(IGrouping<string, Controller> version)
		{
			return
$@"
namespace { Settings.ClientNamespace }{(version.Key != null ? "." : "")}{version.Key}
{{
{string.Join(Environment.NewLine, version.Select(WriteController))}
}}
";
		}

		#endregion Version Blocks


		#region Class


		private static string WriteController(Controller controller)
		{
			return
$@"
{(controller.NamespaceSuffix != null ? $@"namespace {controller.NamespaceSuffix}
{{" : string.Empty)}

{WriteClassInterface(controller)}

{WriteClassImplementation(controller)}

{(controller.NamespaceSuffix != null ? $@"}}" : string.Empty)}
";
		}

		private static string WriteClassInterface(Controller controller)
		{
			return
$@"
{GetObsolete(controller)}
	public interface I{controller.Name} : I{Settings.ClientInterfaceName}
	{{
{string.Join($"{Environment.NewLine}", controller.Endpoints.Select(WriteEndpointInterface))}
	}}
";
		}

		private static string WriteClassImplementation(Controller controller)
		{
			var dependencies = controller.GetInjectionDependencies().ToList();
			dependencies.Insert(0, new ClientDependency($"I{Settings.ClientInterfaceName}Wrapper"));

			return
$@"
{GetObsolete(controller)}
	{(Settings.UseInternalClients ? "internal" : "public")} class {controller.Name} : I{controller.Name}
	{{
{string.Join($"{Environment.NewLine}", dependencies.Select(WriteDependenciesField))}

		public {controller.Name}({string.Join($",{Environment.NewLine}", dependencies.Select(WriteDependenciesParameter))})
		{{
{string.Join($"{Environment.NewLine}", dependencies.Select(WriteDependenciesAssignment))}
		}}

{string.Join($"{Environment.NewLine}", controller.Endpoints.Select(WriteEndpointImplementation))}
	}}
";
		}

		#endregion Class

		#region Dependencies

		private static string WriteDependenciesField(IDependency dependency)
		{
			return $@"		protected readonly {dependency.Type} {dependency.Name};";
		}

		private static string WriteDependenciesParameter(IDependency dependency)
		{
			return $@"{dependency.Type} param_{dependency.Name.ToLower()}";
		}

		private static string WriteDependenciesAssignment(IDependency dependency)
		{
			return $@"{dependency.Name} = param_{dependency.Name.ToLower()};";
		}

		#endregion Dependencies


		#region Endpoint

		private static string WriteEndpointInterface(Endpoint endpoint)
		{
			return
$@"
{GetObsolete(endpoint)}
{GetInterfaceReturnType(endpoint.ReturnType, false)} {endpoint.Name}
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
);

{GetObsolete(endpoint)}
{GetInterfaceReturnType(nameof(HttpResponseMessage), false)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
);

{GetObsolete(endpoint)}
{GetInterfaceReturnType(endpoint.ReturnType, true)} {endpoint.Name}Async
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
);

{GetObsolete(endpoint)}
{GetInterfaceReturnType(nameof(HttpResponseMessage), true)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
);

";
		}

		private static string WriteEndpointImplementation(Endpoint endpoint)
		{
			return
$@"

{GetObsolete(endpoint)}
public {GetImplementationReturnType(endpoint.ReturnType, false)} {endpoint.Name}
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
)
{{
{GetMethodDetails(endpoint, false, false)}
}}

{GetObsolete(endpoint)}
public {GetImplementationReturnType(nameof(HttpResponseMessage), false)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
)
{{
{GetMethodDetails(endpoint, false, true)}
}}

{GetObsolete(endpoint)}
public {GetImplementationReturnType(endpoint.ReturnType, true)} {endpoint.Name}Async
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
)
{{
{GetMethodDetails(endpoint, true, false)}
}}

{GetObsolete(endpoint)}
public {GetImplementationReturnType(nameof(HttpResponseMessage), true)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
)
{{
{GetMethodDetails(endpoint, true, true)}
}}

";
		}


		private static string GetMethodDetails(Endpoint endpoint, bool async, bool raw)
		{
			var cancellationToken = endpoint.GetRequestModifiers().OfType<CancellationTokenModifier>().SingleOrDefault();
			var clientDependency = new ClientDependency($"I{Settings.ClientInterfaceName}Wrapper");

			var requestModifiers = endpoint.GetRequestModifiers().ToList();

			var bodyParameter = endpoint.GetBodyParameter();
			string bodyVariable = bodyParameter?.Name ?? "null";

			var responseTypes = endpoint.GetResponseTypes();

			var routeConstraints = endpoint.GetRouteConstraints();

			return
$@"
{string.Join(Environment.NewLine, routeConstraints.Select(WriteRouteConstraint))}
{GetEndpointInfoVariables(endpoint)}

string url = $@""{GetRoute(endpoint)}"";
HttpResponseMessage response = null;
response = {GetAwait(async)}HttpOverride.GetResponseAsync(HttpMethod.{endpoint.HttpType.Method}, url, null, {cancellationToken.Name}){GetAsyncEnding(async)};

if(response == null)
{{
	response = {GetAwait(async)}{clientDependency.Name}.{nameof(IClientWrapper.ClientWrapper)}
				.Request(url)
{string.Join($"				{Environment.NewLine}", requestModifiers.Select(WriteRequestModifiers))}
				.AllowAnyHttpStatus()
				{GetHttpMethod(endpoint)}
				{GetAsyncEnding(async)};

{GetAwait(async)}HttpOverride.OnNonOverridedResponseAsync(HttpMethod.{endpoint.HttpType.Method}, url, {bodyVariable}, response, {cancellationToken.Name}){GetAsyncEnding(async)};
}}

{string.Join(Environment.NewLine, responseTypes.Select(x => WriteResponseType(x, async, raw)))}
{WriteActionResultReturn(endpoint, async, raw)}
";
		}

		private static string WriteRouteConstraint(RouteConstraint constraint)
		{
			if (constraint is AlphaConstraint)
			{
				return $@"
			if(string.IsNullOrWhiteSpace({constraint.ParameterName}) || {constraint.ParameterName}.Any(x=>char.IsNumber(x)))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} must only contain characters that are not numbers."");
			}}";
			}
			else if (constraint is BoolConstraint)
			{
				return $@"
			if(!bool.TryParse({constraint.ParameterName}.ToString(),out _))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not parse into an bool."");
			}}";
			}
			else if (constraint is DateTimeConstraint)
			{
				return $@"
			if(!DateTime.TryParse({constraint.ParameterName}.ToString(),out _))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not parse into an DateTime."");
			}}";
			}
			else if (constraint is DecimalConstraint)
			{
				return $@"
			if(!decimal.TryParse({constraint.ParameterName}.ToString(),out _))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not parse into an decimal."");
			}}";
			}
			else if (constraint is FloatConstraint)
			{
				return $@"
			if(!float.TryParse({constraint.ParameterName}.ToString(),out _))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not parse into an float."");
			}}";
			}
			else if (constraint is GuidConstraint)
			{
				return $@"
			if(!Guid.TryParse({constraint.ParameterName}.ToString(),out _))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not parse into an Guid."");
			}}";
			}
			else if (constraint is IntConstraint)
			{
				return $@"
			if(!int.TryParse({constraint.ParameterName}.ToString(),out _))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not parse into an int."");
			}}";
			}
			else if (constraint is LengthConstraint)
			{
				var value = constraint.GetConstraintValue();
				if (value.Contains(','))
				{
					var split = value.Split(',');
					string minL = split[0];
					string maxL = split[1];

					return $@"
			if({constraint.ParameterName}.Length <= {minL} || {constraint.ParameterName}.Length >= {maxL})
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} has a length that is not between {minL} and {maxL}."");
			}}";
				}
				else
				{
					return $@"
			if({constraint.ParameterName}.Length == {value})
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} has a length that is not {value}."");
			}}";
				}
			}
			else if (constraint is LongConstraint)
			{
				return $@"
			if(!long.TryParse({constraint.ParameterName}.ToString(),out _))
		{{
			throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not parse into an long."");
		}}";
			}
			else if (constraint is MaxConstraint)
			{
				var value = constraint.GetConstraintValue();

				return $@"
			if({constraint.ParameterName} >= {value})
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} has a value more than {value}."");
			}}";
			}
			else if (constraint is MaxLengthConstraint)
			{
				var value = constraint.GetConstraintValue();

				return $@"
			if({constraint.ParameterName}.Length >= {value})
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} has a length greater than {value}."");
			}}";
			}
			else if (constraint is MinConstraint)
			{
				var value = constraint.GetConstraintValue();

				return $@"
			if({constraint.ParameterName} <= {value})
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} has a value less than {value}."");
			}}";
			}
			else if (constraint is MinLengthConstraint)
			{
				var value = constraint.GetConstraintValue();

				return $@"
			if({constraint.ParameterName}.Length <= {value})
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} has a length less than {value}."");
			}}";
			}
			else if (constraint is RangeConstraint)
			{
				var value = constraint.GetConstraintValue();

				var split = value.Split(',');
				string minL = split[0];
				string maxL = split[1];

				return $@"
			if({constraint.ParameterName} <= {minL} || {constraint.ParameterName} >= {maxL})
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} has a value that is not between {minL} and {maxL}."");
			}}";
			}
			else if (constraint is RegexConstraint)
			{
				var value = constraint.GetConstraintValue();

				return $@"
			if(!(new Regex(@""{value}"").IsMatch({constraint.ParameterName})))
			{{
				throw new InvalidRouteException(""Parameter {constraint.ParameterName} does not follow the regex \""{value}\""."");
			}}";
			}
			else if (constraint is RequiredConstraint)
			{
				return null;
			}
			else
			{
				return $@"#error A route constraint of type {constraint.GetType().Name} and text of {constraint.Constraint} is not supported";
			}
		}

		private static string WriteActionResultReturn(Endpoint endpoint, bool async, bool raw)
		{
			if (raw)
			{
				return null;
			}

			if (endpoint.ReturnType != null)
			{
				return
$@"
if(response.IsSuccessStatusCode)
{{
	return {GetAwait(async)}Serializer.Deserialize<{endpoint.ReturnType}>(response.Content){GetAsyncEnding(async)};
}}
else
{{
	return default({endpoint.ReturnType});
}}
";
			}

			return null;
		}

		private static string WriteResponseType(ResponseType responseType, bool async, bool raw)
		{
			if (raw)
			{
				return $@"return response;";
			}

			return
$@"
{GetResponseTypeCheck(responseType)}
{GetResponseTypeInvoke(responseType, async)}
";
		}

		private static string GetResponseTypeCheck(ResponseType responseType)
		{
			return
$@"
if({responseType.Name} != null && {responseType.Name}.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
{{
	throw new NotSupportedException(""Async void action delegates for {responseType.Name} are not supported.As they will run out of the scope of this call."");
}}
";
		}

		private static string GetResponseTypeInvoke(ResponseType responseType, bool async)
		{
			if (responseType.Status == null)
			{
				return $@"{responseType.Name}?.Invoke(response);";
			}
			else
			{
				return
$@"
if(response.StatusCode == System.Net.HttpStatusCode.{responseType.Status})
{{
	{responseType.Name}?.Invoke(Serializer.Deserialize<{responseType.ActionType}>(response.Content).{GetAsyncEnding(async)});
}}
";
			}
		}

		private static string GetHttpMethod(Endpoint endpoint)
		{
			var cancellationToken = endpoint.GetRequestModifiers().OfType<CancellationTokenModifier>().SingleOrDefault();
			var bodyParameter = endpoint.GetBodyParameter();
			string bodyComma = bodyParameter != null ? "," : "";

			if (endpoint.HttpType.Method == HttpMethod.Delete.Method)
			{
				return $".{nameof(GeneratedExtensions.DeleteAsync)}({cancellationToken?.Name})";
			}
			else if (endpoint.HttpType.Method == HttpMethod.Get.Method)
			{
				return $".{nameof(GeneratedExtensions.GetAsync)}({cancellationToken?.Name})";
			}
			else if (endpoint.HttpType.Method == HttpMethod.Put.Method)
			{
				return $".{nameof(GeneratedExtensions.PutAsync)}({bodyParameter.Name}{bodyComma}{cancellationToken?.Name})";
			}
			else if (endpoint.HttpType.Method == new HttpMethod("PATCH").Method)
			{
				return $".{nameof(GeneratedExtensions.PatchAsync)}({bodyParameter.Name}{bodyComma}{cancellationToken?.Name})";
			}
			else if (endpoint.HttpType.Method == HttpMethod.Post.Method)
			{
				return $".{nameof(GeneratedExtensions.PostAsync)}({bodyParameter.Name}{bodyComma}{cancellationToken?.Name})";
			}
			else
			{
				return $"#error Unsupported HttpMethod of {endpoint.HttpType.Method}";
			}
		}

		private static string WriteRequestModifiers(IRequestModifier modifier)
		{
			if (modifier is CookieModifier cm)
			{
				return $@".WithCookies({cm.Name})";
			}
			else if (modifier is HeadersModifier hm)
			{
				return $@".WithHeaders({hm.Name})";
			}
			else if (modifier is RequestModifierDependency rm)
			{
				return $@".WithRequestModifiers({rm.Name})";
			}
			else if (modifier is SecurityModifier sm)
			{
				return $@".WithAuth({sm.Name})";
			}
			else if (modifier is TimeoutModifier tm)
			{
				var clientDependency = new ClientDependency(null);
				return $@".WithTimeout({tm.Name} ?? {clientDependency.Name}.{nameof(IClientWrapper.Timeout)})";
			}
			else
			{
				return $@"#warning IRequestModifier of type {modifier.GetType().Name} is not supported";
			}
		}

		private static string GetEndpointInfoVariables(Endpoint endpoint)
		{

			var controllerVar = $@"			var {Constants.ControllerRouteReserved} = ""{endpoint.Parent.Name}"";";
			var actionVar = $@"			var {Constants.ActionRouteReserved} = ""{endpoint.Name}"";";


			if (!endpoint.FullRoute.Contains($"{{{Constants.ControllerRouteReserved}}}"))
			{
				controllerVar = null;
			}

			if (!endpoint.FullRoute.Contains($"{{{Constants.ActionRouteReserved}}}"))
			{
				actionVar = null;
			}


			return
$@"
{controllerVar}
{actionVar}
";
		}

		private static string GetRoute(Endpoint endpoint)
		{

			const string RouteParseRegex = @"{([^}]+)}";

			string routeUnformatted = endpoint.FullRoute;

			var patterns = Regex.Matches(routeUnformatted, RouteParseRegex);

			var routeParameters = endpoint.GetRouteParameters();
			var queryParameters = endpoint.GetQueryParameters();

			foreach (var group in patterns)
			{
				Match match = group as Match;
				string filtered = match.Value.Replace("{", "").Replace("}", "");
				string[] split = filtered.Split(new char[] { ':' });

				string variable = split[0];


				if (!routeParameters.Any(x => x.Name.Equals(variable, StringComparison.CurrentCultureIgnoreCase)))
				{
					throw new Exception($"{variable} is missing from passed in parameters. Please check your route.");
				}
				var parameter = routeParameters.SingleOrDefault(x => x.Name.Equals(variable, StringComparison.CurrentCultureIgnoreCase));
				if (Helpers.IsRoutableType(parameter.Type))
				{
					routeUnformatted = routeUnformatted.Replace(match.Value, $"{{{Helpers.GetRouteStringTransform(parameter.Name, parameter.Type)}}}");
				}
			}

			if (queryParameters.Any())
			{
				string queryString = $"?{string.Join("&", queryParameters.Select(WriteQueryParameter))}";

				routeUnformatted += $"{queryString}";
			}



			routeUnformatted = routeUnformatted.Replace($"[{Constants.ControllerRouteReserved}]", $"{{{Constants.ControllerRouteReserved}}}");
			routeUnformatted = routeUnformatted.Replace($"[{Constants.ActionRouteReserved}]", $"{{{Constants.ActionRouteReserved}}}");
			return routeUnformatted;
		}

		private static string WriteQueryParameter(QueryParameter parameter)
		{
			string name = $"{{nameof({parameter.Name})}}";

			if (Helpers.IsEnumerable(parameter.Type))
			{
				return $@"{{string.Join(""&"",{parameter.Name}.Select(x => $""{name}={{{Helpers.GetRouteStringTransform("x", parameter.Type)}}}""))}}";
			}
			else if (parameter.DefaultValue != null)
			{
				return $"{name}={{{Helpers.GetRouteStringTransform(parameter.Name, parameter.Type)}}}";
			}
			else
			{
				return $"{{{Helpers.GetRouteStringTransform(parameter.Name, parameter.Type)}}}";
			}
		}

		private static string GetAsyncEnding(bool async)
		{
			if (async)
			{
				return $@".ConfigureAwait(false)";
			}
			else
			{
				return $@".ConfigureAwait(false).GetAwaiter().GetResult()";
			}
		}

		private static string GetAwait(bool async)
		{
			if (async)
			{
				return "await ";
			}
			return null;
		}

		private static string GetInterfaceReturnType(string returnType, bool async)
		{
			if (async)
			{
				if (returnType == null)
				{
					return $"Task";
				}
				else
				{
					return $"{Helpers.GetTaskType()}<{returnType}>";
				}
			}
			else
			{
				if (returnType == null)
				{
					return $"void";
				}
				else
				{
					return $"{returnType}";
				}
			}
		}

		private static string GetImplementationReturnType(string returnType, bool async)
		{
			if (async)
			{
				if (returnType == null)
				{
					return $"async Task";
				}
				else
				{
					return $"async {Helpers.GetTaskType()}<{returnType}>";
				}
			}
			else
			{
				if (returnType == null)
				{
					return $"void";
				}
				else
				{
					return $"{returnType}";
				}
			}
		}

		private static string GetParameter(IParameter parameter)
		{
			return $@"{parameter.Type} {parameter.Name}{(parameter.DefaultValue != null ? $" = {parameter.DefaultValue}" : $"")}";
		}

		private static string GetObsolete(IObsolete ob)
		{
			if (ob.Obsolete)
			{
				return $@"	[{Constants.Obsolete}(""{ob.ObsoleteMessage}"")]";
			}
			else
			{
				return string.Empty;
			}

		}

		#endregion Endpoint
	}
}
