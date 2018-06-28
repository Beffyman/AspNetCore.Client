using AspNetCore.Client.Generator.Data.RouteConstraints;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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

			var ignoreAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith(AspNetCore.Client.Core.NoClientAttribute.AttributeName));
			if (ignoreAttribute != null)
			{
				IsNotEndpoint = true;
				return;
			}


			//Route Attribute

			var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith("Route"));
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				Options.Route = routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}


			//HTTP Attribute

			var httpAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith("Http"));
			if (httpAttribute == null)
			{
				IsNotEndpoint = true;
				return;
			}

			Options.HttpType = (HttpAttributeType)Enum.Parse(typeof(HttpAttributeType),
				httpAttribute.Name
				.ToFullString()
				.Replace("Http", "")
				.Replace("Attribute", ""));

			if (Options.Route == null && httpAttribute.ArgumentList != null)//If Route was never fetched from RouteAttribute or if they used the Http(template) override
			{
				Options.Route = httpAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}
			else if (Options.Route == null)
			{
				Options.Route = string.Empty;
			}

			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith("Obsolete"));
			if (obsoleteAttribute != null)
			{
				Options.Obsolete = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			//Authorize Attribute
			Options.Authorize = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith("Authorize")) != null;
			//AllowAnonymous Attribute
			Options.AllowAnonymous = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith("AllowAnonymous")) != null;


			//Response types
			var responseTypes = attributes.Where(x => x.Name.ToFullString().StartsWith("ProducesResponseType"));
			Responses = responseTypes.Select(x => new ResponseTypeDefinition(this, x)).ToList();

			foreach (var responseType in Settings.Instance.KnownStatusesAndResponseTypes)
			{
				if (!Responses.Any(x => x.StatusValue == responseType.Key))
				{
					Responses.Add(new ResponseTypeDefinition(this, responseType.Key, responseType.Value));
				}
			}
			Responses.Add(new ResponseTypeDefinition(this, true));

			Parameters = MethodSyntax.ParameterList.Parameters.Select(x => new ParameterDefinition(this, x)).ToList();


			Headers = attributes.Where(x => x.Name.ToFullString().StartsWith(AspNetCore.Client.Core.IncludesHeaderAttribute.AttributeName))
				.Select(x => new HeaderDefinition(x))
				.ToList();

			Options.ActionResultReturn = MethodSyntax.ReturnType.ToFullString().Contains("IActionResult");


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
					if (parameter.RouteName == "code")
					{

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



				routeUnformatted = routeUnformatted.Replace("[controller]", "{controller}");
				routeUnformatted = routeUnformatted.Replace("[action]", "{action}");
				return routeUnformatted;
			}
		}


		public string MethodParametersOutput(bool async, bool actionResponses)
		{
			if (!async && actionResponses)
			{
				return string.Join(", ", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), Responses.Select(x => x.SyncMethodOutput) }.SelectMany(x => x));
			}
			else if (async && actionResponses)
			{
				return string.Join(", ", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), Responses.Select(x => x.AsyncMethodOutput) }.SelectMany(x => x));
			}
			else if (!actionResponses)
			{
				return string.Join(", ", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput) }.SelectMany(x => x));
			}
			throw new Exception("Failed to parse boolean tree");
		}

		public IList<HeaderDefinition> GetAllHeaders()
		{
			List<HeaderDefinition> additionalHeaders = null;
			if (Settings.Instance.RequiredHttpHeaders.ContainsKey(Options.HttpType.ToString()))
			{
				additionalHeaders = Settings.Instance.RequiredHttpHeaders[Options.HttpType.ToString()];
			}
			else
			{
				additionalHeaders = new List<HeaderDefinition>();
			}

			var allHeaders = ParentClass.Headers.Union(this.Headers).Union(additionalHeaders).ToList();
			return allHeaders;
		}

		public string GetInterfaceText()
		{
			string correctedName = Name.Replace("Async", "");

			var allHeaders = GetAllHeaders();

			string syncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), Responses.Select(x => x.SyncMethodOutput), OptionParameters() }.SelectMany(x => x));
			string rawParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), OptionParameters() }.SelectMany(x => x));
			string asyncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), Responses.Select(x => x.AsyncMethodOutput), OptionParameters() }.SelectMany(x => x));

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

				if(returnType == "void")
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

			var allHeaders = GetAllHeaders();

			string syncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), Responses.Select(x => x.SyncMethodOutput), OptionParameters() }.SelectMany(x => x));
			string rawParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), OptionParameters() }.SelectMany(x => x));
			string asyncParameters = string.Join($", {Environment.NewLine}			", new List<IEnumerable<string>> { Parameters.Select(x => x.MethodParameterOutput), allHeaders.Select(x => x.ParameterOutput()), Responses.Select(x => x.AsyncMethodOutput), OptionParameters() }.SelectMany(x => x));

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


				if (returnType == "void")
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
			return response;
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
			return response;
		}}
";
		}

		private string ResultTypeReturn(bool async, string returnType)
		{

			string read = null;
			if (Helpers.KnownPrimitives.Contains(returnType, StringComparer.CurrentCultureIgnoreCase))
			{
				read = $@"{(async ? "await " : string.Empty)}response.Content.ReadAsNonJsonAsync<{returnType}>(){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")}";
			}
			else
			{
				read = $@"{Helpers.GetJsonDeserializer()}<{returnType}>({(async ? "await " : string.Empty)}response.Content.ReadAsStringAsync(){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")})";
			}

			return $@"
			if(response.IsSuccessStatusCode)
			{{
				return {read};
			}}
			else
			{{
				return null;
			}}
";

		}

		private string ResponseBuilder(bool async)
		{
			var additionalVars = $@"
			var controller = ""{ParentClass.ControllerName}"";
			var action = ""{Name}"";";
			var route = FullRoute;

			bool contains = route.Contains("{controller}") || route.Contains("{action}");


			var str =
$@"{(contains ? additionalVars : string.Empty)}

			string url = $@""{FullRoute}"";
			HttpResponseMessage response = {(async ? "await " : "")}{GetHttpText()}{(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")};
";

			return str;
		}


		private string GetHttpText()
		{
			string str = $@"Client.ClientWrapper
				.Request(url)";

			const string tabs = "\t\t\t\t";


			if (Options.Authorize)
			{
				str = $"{str}{Environment.NewLine}{tabs}.WithAuth(auth)";
			}


			var allHeaders = GetAllHeaders();

			foreach (var defaultHeader in Settings.Instance.DefaultHttpHeaders)
			{
				if (Options.HttpType.ToString().Equals(defaultHeader.Key, StringComparison.CurrentCultureIgnoreCase))
				{
					foreach (var header in defaultHeader.Value)
					{
						str = $"{str}{Environment.NewLine}{tabs}{HeaderDefinition.GetMethodOutput($@"""{header.Key}""", header.Value)}";
					}
				}
			}

			foreach (var header in allHeaders)
			{
				str = $"{str}{Environment.NewLine}{tabs}{header.MethodOutput()}";
			}


			var bodyParameter = Parameters.SingleOrDefault(x => x.Options?.Body ?? false);

			str = $"{str}{Environment.NewLine}{tabs}.AllowAnyHttpStatus()";
			//str = $@"{str}{Environment.NewLine}{tabs}.WithTimeout({Settings.Instance.ServiceName}HttpClient.Timeout)";
			switch (Options.HttpType)
			{
				case HttpAttributeType.Delete:
					return $"{str}{Environment.NewLine}{tabs}.DeleteAsync()";
				case HttpAttributeType.Get:
					return $"{str}{Environment.NewLine}{tabs}.GetAsync()";
				case HttpAttributeType.Put:
					return $"{str}{Environment.NewLine}{tabs}.PutJsonAsync({bodyParameter?.HttpCallOutput ?? "null"})";
				case HttpAttributeType.Patch:
					return $"{str}{Environment.NewLine}{tabs}.PatchJsonAsync({bodyParameter?.HttpCallOutput ?? "null"})";
				case HttpAttributeType.Post:
					return $"{str}{Environment.NewLine}{tabs}.PostJsonAsync({bodyParameter?.HttpCallOutput ?? "null"})";
				default:
					throw new Exception("Unexpected HTTPType");
			}
		}


		private string GetObsolete()
		{
			if (Options.Obsolete != null)
			{
				return $@"		[Obsolete(""{Options.Obsolete}"")]";
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
				optionParameters.Add($"SecurityHeader auth = null");
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
