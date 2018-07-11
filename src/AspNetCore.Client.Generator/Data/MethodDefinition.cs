using AspNetCore.Client;
using AspNetCore.Client.Attributes;
using AspNetCore.Client.Authorization;
using AspNetCore.Client.Serializers;
using AspNetCore.Client.Generator.Data.RouteConstraints;
using Flurl.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AspNetCore.Client.Generator.Data
{
	public class MethodDefinition
	{
		public bool IsNotEndpoint { get; set; } = false;


		public string Name { get; }
		public ClassDefinition ParentClass { get; }
		public MethodDeclarationSyntax MethodSyntax { get; }

		public IList<ParameterDefinition> Parameters { get; }
		public IList<ResponseTypeDefinition> Responses { get; }
		public IList<ParameterHeaderDefinition> ParameterHeaders { get; }
		public IList<HeaderDefinition> Headers { get; }

		public MethodOptions Options { get; }

		public IDictionary<string, string> FullRouteParameters => FullRouteTemplate.GetRouteParameters();
		public IList<RouteConstraint> RouteConstraints => FullRouteParameters.Select(x => RouteConstraint.GetConstraint(x.Key, x.Value))
																			.Where(x => x != null)
																			.ToList();



		public MethodDefinition(
			ClassDefinition parentClass,
			MethodDeclarationSyntax methodSyntax)
		{
			ParentClass = parentClass;
			MethodSyntax = methodSyntax;

			Name = MethodSyntax.Identifier.ValueText.Trim();

			Options = new MethodOptions();

			var attributes = MethodSyntax.DescendantNodes().OfType<AttributeListSyntax>().SelectMany(x => x.Attributes).ToList();


			//Ignore generator attribute

			var ignoreAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(NoClientAttribute.AttributeName));
			if (ignoreAttribute != null)
			{
				IsNotEndpoint = true;
				return;
			}


			//Route Attribute

			var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Route));
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				Options.Route = routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}


			//HTTP Attribute

			var knownHttpAttributes = new List<string>
			{
				$"{Constants.Http}{HttpAttributeType.Delete}",
				$"{Constants.Http}{HttpAttributeType.Get}",
				$"{Constants.Http}{HttpAttributeType.Patch}",
				$"{Constants.Http}{HttpAttributeType.Post}",
				$"{Constants.Http}{HttpAttributeType.Put}",
			};

			var httpAttribute = attributes.SingleOrDefault(x => knownHttpAttributes.Any(y=>x.Name.ToFullString().MatchesAttribute(y)));
			if (httpAttribute == null)
			{
				IsNotEndpoint = true;
				return;
			}

			Options.HttpType = (HttpAttributeType)Enum.Parse(typeof(HttpAttributeType),
				httpAttribute.Name
				.ToFullString()
				.Replace(Constants.Http, "")
				.Replace(Constants.Attribute, ""));

			if (Options.Route == null && httpAttribute.ArgumentList != null)//If Route was never fetched from RouteAttribute or if they used the Http(template) override
			{
				Options.Route = httpAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}
			else if (Options.Route == null)
			{
				Options.Route = string.Empty;
			}

			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Obsolete));
			if (obsoleteAttribute != null)
			{
				Options.Obsolete = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			//Authorize Attribute
			Options.Authorize = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Authorize)) != null;
			//AllowAnonymous Attribute
			Options.AllowAnonymous = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.AllowAnonymous)) != null;


			//Response types
			var responseTypes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(Constants.ProducesResponseType));
			Responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();
			Responses.Add(new ResponseTypeDefinition(true));

			Parameters = MethodSyntax.ParameterList.Parameters.Select(x => new ParameterDefinition(this, x)).ToList();


			ParameterHeaders = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(HeaderParameterAttribute.AttributeName))
				.Select(x => new ParameterHeaderDefinition(x))
				.ToList();

			Headers = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(IncludeHeaderAttribute.AttributeName))
				.Select(x => new HeaderDefinition(x))
				.ToList();

			Options.ActionResultReturn = MethodSyntax.ReturnType.ToFullString().Contains(Constants.IActionResult);


		}


		public string FullRouteTemplate
		{
			get
			{
				if (string.IsNullOrEmpty(Options.Route))
				{

					return ParentClass.Route;
				}
				else
				{
					return $"{ParentClass.Route}/{Options.Route}";
				}
			}
		}


		public string FullRoute
		{
			get
			{

				const string RouteParseRegex = @"{([^}]+)}";

				string routeUnformatted = FullRouteTemplate;

				var patterns = Regex.Matches(routeUnformatted, RouteParseRegex);

				foreach (var group in patterns)
				{
					Match match = group as Match;
					string filtered = match.Value.Replace("{", "").Replace("}", "");
					string[] split = filtered.Split(new char[] { ':' });

					string variable = split[0];


					if (!Parameters.Any(x => x.RouteName.Equals(variable, StringComparison.CurrentCultureIgnoreCase)))
					{
						throw new Exception($"{variable} is missing from passed in parameters. Please check your route.");
					}
					var parameter = Parameters.SingleOrDefault(x => x.RouteName.Equals(variable, StringComparison.CurrentCultureIgnoreCase));
					if (Helpers.IsRoutableType(parameter.Type))
					{
						routeUnformatted = routeUnformatted.Replace(match.Value, $"{{{Helpers.GetRouteStringTransform(parameter.RouteName, parameter.Type)}}}");
					}
				}

				var queryVariables = Parameters.Where(x => Helpers.IsRoutableType(x.Type)
														|| Helpers.IsEnumerable(x.Type))
													.Select(x => x.RouteName)
													.Except(FullRouteParameters.Select(x => x.Key))
													.ToList();
				var queryParams = Parameters.Where(x => queryVariables.Contains(x.RouteName)).ToList();


				if (queryParams != null && queryParams.Any())
				{
					string queryString = $"?{string.Join("&", queryParams.Select(x => x.RouteOutput))}";

					routeUnformatted += $"{queryString}";
				}



				routeUnformatted = routeUnformatted.Replace($"[{Constants.ControllerRouteReserved}]", $"{{{Constants.ControllerRouteReserved}}}");
				routeUnformatted = routeUnformatted.Replace($"[{Constants.ActionRouteReserved}]", $"{{{Constants.ActionRouteReserved}}}");
				return routeUnformatted;
			}
		}


		public string MethodParametersOutput(bool async, bool actionResponses)
		{
			if (!async && actionResponses)
			{
				return string.Join(", ", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), ParentClass.Responses.Select(x => x.SyncMethodOutput), Responses.Select(x => x.SyncMethodOutput) }.SelectMany(x => x));
			}
			else if (async && actionResponses)
			{
				return string.Join(", ", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), ParentClass.Responses.Select(x => x.AsyncMethodOutput), Responses.Select(x => x.AsyncMethodOutput) }.SelectMany(x => x));
			}
			else if (!actionResponses)
			{
				return string.Join(", ", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput) }.SelectMany(x => x));
			}
			throw new Exception("Failed to parse boolean tree");
		}

		public IList<ParameterHeaderDefinition> GetAllParameterHeaders()
		{
			var allHeaders = ParentClass.ParameterHeaders.Union(this.ParameterHeaders).ToList();
			return allHeaders.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
		}

		public IList<HeaderDefinition> GetAllHeaders()
		{
			var allHeaders = ParentClass.Headers.Union(this.Headers).ToList();
			return allHeaders.OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
		}

		static readonly IEnumerable<string> CANCELLATION_TOKEN_PARAM = new List<string> { $"{nameof(CancellationToken)} {Constants.CancellationTokenParameter} = default({nameof(CancellationToken)})" };

		public string GetInterfaceText()
		{
			string correctedName = Name.Replace("Async", "");

			var allHeaders = GetAllParameterHeaders();

			string syncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), ParentClass.Responses.Select(x => x.SyncMethodOutput), Responses.Select(x => x.SyncMethodOutput), OptionParameters(), CANCELLATION_TOKEN_PARAM }.SelectMany(x => x));
			string rawParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), OptionParameters(), CANCELLATION_TOKEN_PARAM }.SelectMany(x => x));
			string asyncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), ParentClass.Responses.Select(x => x.AsyncMethodOutput), Responses.Select(x => x.AsyncMethodOutput), OptionParameters(), CANCELLATION_TOKEN_PARAM }.SelectMany(x => x));

			var returnType = MethodSyntax.ReturnType?.ToFullString();

			if (!Options.ActionResultReturn)
			{
				var regex = new Regex(@"(ValueTask|Task|ActionResult)<(.+)>");
				var match = regex.Match(returnType);
				if (match.Success)
				{
					returnType = match.Groups[2].Value;
				}

				returnType = returnType.Trim();


				if (returnType == "void" || returnType == "Task")
				{
					returnType = null;
				}
			}
			else
			{
				returnType = null;
			}


			return
$@"		{GetObsolete()}
		{(returnType == null ? "void" : returnType)} {correctedName}({syncParameters});

		{GetObsolete()}
		HttpResponseMessage {correctedName}Raw({rawParameters});

		{GetObsolete()}
		{(returnType == null ? "Task" : $"{Helpers.GetTaskType()}<{returnType}>")} {correctedName}Async({asyncParameters});

		{GetObsolete()}
		{Helpers.GetTaskType()}<HttpResponseMessage> {correctedName}RawAsync({rawParameters});
";
		}

		public string GetImplementationText()
		{
			string correctedName = Name.Replace("Async", "");

			var allHeaders = GetAllParameterHeaders();

			string syncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), ParentClass.Responses.Select(x => x.SyncMethodOutput), Responses.Select(x => x.SyncMethodOutput), OptionParameters(), CANCELLATION_TOKEN_PARAM }.SelectMany(x => x));
			string rawParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), OptionParameters(), CANCELLATION_TOKEN_PARAM }.SelectMany(x => x));
			string asyncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), ParentClass.Responses.Select(x => x.AsyncMethodOutput), Responses.Select(x => x.AsyncMethodOutput), OptionParameters(), CANCELLATION_TOKEN_PARAM }.SelectMany(x => x));

			var returnType = MethodSyntax.ReturnType?.ToFullString();

			if (!Options.ActionResultReturn)
			{
				var regex = new Regex(@"(ValueTask|Task|ActionResult)<(.+)>");
				var match = regex.Match(returnType);
				if (match.Success)
				{
					returnType = match.Groups[2].Value;
				}

				returnType = returnType.Trim();


				if (returnType == "void" || returnType == "Task")
				{
					returnType = null;
				}
			}
			else
			{
				returnType = null;
			}


			return
$@"{GetObsolete()}
		public {(returnType == null ? "void" : returnType)} {correctedName}({syncParameters})
		{{
{string.Join(Environment.NewLine, RouteConstraints.Select(x => x.GetText()).Where(x => !string.IsNullOrEmpty(x)))}
			{ResponseBuilder(false)}
{string.Join(Environment.NewLine, Responses.Select(x => x.SyncMethodBlock))}
			{(returnType == null ? "return;" : ResultTypeReturn(false, returnType))}
		}}

{GetObsolete()}
		public HttpResponseMessage {correctedName}Raw({rawParameters})
		{{
{string.Join(Environment.NewLine, RouteConstraints.Select(x => x.GetText()).Where(x => !string.IsNullOrEmpty(x)))}
			{ResponseBuilder(false)}
			return {Constants.ResponseVariable};
		}}

{GetObsolete()}
		public async {(returnType == null ? "Task" : $"{Helpers.GetTaskType()}<{returnType}>")} {correctedName}Async({asyncParameters})
		{{
{string.Join(Environment.NewLine, RouteConstraints.Select(x => x.GetText()).Where(x => !string.IsNullOrEmpty(x)))}
			{ResponseBuilder(true)}
{string.Join(Environment.NewLine, Responses.Select(x => x.AsyncMethodBlock))}
			{(returnType == null ? "return;" : ResultTypeReturn(true, returnType))}
		}}

{GetObsolete()}
		public async {Helpers.GetTaskType()}<HttpResponseMessage> {correctedName}RawAsync({rawParameters})
		{{
{string.Join(Environment.NewLine, RouteConstraints.Select(x => x.GetText()).Where(x => !string.IsNullOrEmpty(x)))}
			{ResponseBuilder(true)}
			return {Constants.ResponseVariable};
		}}
";
		}

		private string ResultTypeReturn(bool async, string returnType)
		{

			string read = $@"{(async ? "await " : string.Empty)}{Constants.SerializerField}.{nameof(IHttpSerializer.Deserialize)}<{returnType}>({Constants.ResponseVariable}.Content){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")}";

			return $@"
			if({Constants.ResponseVariable}.IsSuccessStatusCode)
			{{
				return {read};
			}}
			else
			{{
				return default({returnType});
			}}
";

		}

		private string ResponseBuilder(bool async)
		{
			var controllerVar = $@"			var {Constants.ControllerRouteReserved} = ""{ParentClass.ControllerName}"";";
			var actionVar = $@"			var {Constants.ActionRouteReserved} = ""{Name}"";";


			var route = FullRoute;

			bool containsController = route.Contains($"{{{Constants.ControllerRouteReserved}}}");
			bool containsAction = route.Contains($"{{{Constants.ActionRouteReserved}}}");


			var str =
$@"
{(containsController ? controllerVar : string.Empty)}
{(containsAction ? actionVar : string.Empty)}

			string {Constants.UrlVariable} = $@""{FullRoute}"";
			HttpResponseMessage {Constants.ResponseVariable} = null;
			{GetHttpOverridePre(async)}
			if({Constants.ResponseVariable} == null)
			{{
				{Constants.ResponseVariable} = {(async ? "await " : "")}{GetHttpText()}{(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")};
				{GetHttpOverridePost(async)}
			}}
";

			return str;
		}

		private string GetHttpOverridePre(bool async)
		{
			return $@"{Constants.ResponseVariable} = {(async ? "await " : "")}{Constants.HttpOverrideField}.{Constants.HttpOverrideGetMethod}({Constants.UrlVariable}, {Constants.CancellationTokenParameter}){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")};";
		}

		private string GetHttpOverridePost(bool async)
		{
			return $@"
				{(async ? "await " : "")}{Constants.HttpOverrideField}.{Constants.HttpOverrideOnNonOverridedResponse}({Constants.UrlVariable}, {Constants.ResponseVariable}, {Constants.CancellationTokenParameter}){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")};";
		}

		private string GetHttpText()
		{
			string str = $@"{Constants.ClientInterfaceName}.{Constants.FlurlClientVariable}
				.{nameof(FlurlClient.Request)}({Constants.UrlVariable})";

			const string tabs = "\t\t\t\t";


			if (Options.Authorize)
			{
				str = $"{str}{Environment.NewLine}{tabs}.{nameof(GeneratorExtensions.WithAuth)}({Constants.AuthParameter})";
			}


			foreach (var header in GetAllParameterHeaders())
			{
				str = $"{str}{Environment.NewLine}{tabs}{header.MethodOutput()}";
			}

			foreach (var header in GetAllHeaders())
			{
				str = $"{str}{Environment.NewLine}{tabs}{header.MethodOutput()}";
			}


			str = $"{str}{Environment.NewLine}{tabs}.{nameof(GeneratorExtensions.WithRequestModifiers)}({Constants.RequestModifierField})";

			var bodyParameter = Parameters.SingleOrDefault(x => x.Options?.Body ?? false);

			str = $"{str}{Environment.NewLine}{tabs}.{nameof(SettingsExtensions.AllowAnyHttpStatus)}()";
			str = $"{str}{Environment.NewLine}{tabs}.{nameof(SettingsExtensions.WithTimeout)}({Constants.ClientInterfaceName}.Timeout)";

			switch (Options.HttpType)
			{
				case HttpAttributeType.Delete:
					return $"{str}{Environment.NewLine}{tabs}.{nameof(GeneratedExtensions.DeleteAsync)}({Constants.CancellationTokenParameter})";
				case HttpAttributeType.Get:
					return $"{str}{Environment.NewLine}{tabs}.{nameof(GeneratedExtensions.GetAsync)}({Constants.CancellationTokenParameter})";
				case HttpAttributeType.Put:
					return $"{str}{Environment.NewLine}{tabs}.{nameof(GeneratedExtensions.PutStringAsync)}({Constants.SerializerField}.{nameof(IHttpSerializer.Serialize)}({bodyParameter?.HttpCallOutput ?? "null"}),{Constants.CancellationTokenParameter})";
				case HttpAttributeType.Patch:
					return $"{str}{Environment.NewLine}{tabs}.{nameof(GeneratedExtensions.PatchStringAsync)}({Constants.SerializerField}.{nameof(IHttpSerializer.Serialize)}({bodyParameter?.HttpCallOutput ?? "null"}),{Constants.CancellationTokenParameter})";
				case HttpAttributeType.Post:
					return $"{str}{Environment.NewLine}{tabs}.{nameof(GeneratedExtensions.PostStringAsync)}({Constants.SerializerField}.{nameof(IHttpSerializer.Serialize)}({bodyParameter?.HttpCallOutput ?? "null"}),{Constants.CancellationTokenParameter})";
				default:
					throw new Exception("Unexpected HTTPType");
			}
		}


		private string GetObsolete()
		{
			if (Options.Obsolete != null)
			{
				return $@"		[{Constants.Obsolete}(""{Options.Obsolete}"")]";
			}
			else
			{
				return string.Empty;
			}

		}

		public IEnumerable<string> OptionParameters()
		{
			var optionParameters = new List<string>();

			if (Options.Authorize || ParentClass.Options.Authorize)
			{
				optionParameters.Add($"{nameof(SecurityHeader)} {Constants.AuthParameter} = null");
			}


			return optionParameters;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class MethodOptions
	{
		public string Route { get; set; }
		public HttpAttributeType HttpType { get; set; }
		public string Obsolete { get; set; }
		public bool ActionResultReturn { get; set; }
		public bool Authorize { get; set; }
		public bool AllowAnonymous { get; set; }
	}
}
