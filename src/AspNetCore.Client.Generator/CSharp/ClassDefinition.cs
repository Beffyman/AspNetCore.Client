using AspNetCore.Client.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AspNetCore.Client.Generator.Framework;
using System.Net;
using AspNetCore.Client.Generator.Framework.Routes;

namespace AspNetCore.Client.Generator.CSharp
{
	public class ClassDefinition
	{
		public string Name { get; }
		public string ControllerName => Name.Replace("Controller", "");
		public string ClientName => Name.Replace("Controller", "Client");
		public string NamespaceVersion { get; }

		public IList<MethodDefinition> Methods { get; }
		public IList<ResponseTypeDefinition> Responses { get; }

		public string Route { get; }
		public IList<ParameterHeaderDefinition> ParameterHeaders { get; }
		public IList<HeaderDefinition> Headers { get; }

		private static Regex RouteVersionRegex = new Regex(@"\/([v|V]\d+)\/");

		public ClassOptions Options { get; set; }

		public bool NotEmpty => !Options.NoClient && (Methods?.Any(x => !x.IsNotEndpoint) ?? false);

		public ClassDefinition(string @namespace,
			string className,
			ParsedFile file,
			ClassDeclarationSyntax classDeclaration,
			IList<AttributeSyntax> attributes,
			IList<MethodDeclarationSyntax> methods)
		{
			Name = className;

			Options = new ClassOptions();

			var ignoreAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(NoClientAttribute.AttributeName));
			if (ignoreAttribute != null)
			{
				Options.NoClient = true;
				return;
			}


			var namespaceAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(NamespaceSuffixAttribute.AttributeName));
			if (namespaceAttribute != null)
			{
				Options.NamespaceSuffix = namespaceAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
			}

			var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Route));
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				Route = routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
			}

			if (Route == null)//No Route, invalid controller
			{
				Options.NoClient = true;
				throw new NotSupportedException("Controller must have a route to be valid for generation.");
			}

			var match = RouteVersionRegex.Match(Route);
			if (match.Success)
			{
				var group = match.Groups[1];
				NamespaceVersion = group.Value.ToUpper();
			}

			//Response types
			var responseTypes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(Constants.ProducesResponseType));
			Responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();



			ParameterHeaders = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(HeaderParameterAttribute.AttributeName))
				.Select(x => new ParameterHeaderDefinition(x))
				.ToList();

			Headers = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(IncludeHeaderAttribute.AttributeName))
				.Select(x => new HeaderDefinition(x))
				.ToList();

			//Authorize Attribute
			Options.Authorize = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Authorize)) != null;

			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Obsolete));
			if (obsoleteAttribute != null)
			{
				Options.Obsolete = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			//Only public or internal endpoints can be hit anyways
			Methods = methods.Where(x => x.Modifiers.Any(y => y.Text == "public"))
				.Select(x => new MethodDefinition(this, x))
				.ToList();

		}


		public Framework.Controller GetClient()
		{
			var client = new Framework.Controller();
			client.Name = ControllerName;
			client.Ignored = Options.NoClient;
			client.ConstantHeader = Headers.Select(x => new Framework.Headers.ConstantHeader(x.Name, x.Value)).ToList();
			client.ParameterHeader = ParameterHeaders.Select(x => new Framework.Headers.ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();
			client.ResponseTypes = Responses.Select(x => new Framework.ResponseTypes.ResponseType(x.Type, Helpers.EnumParse<HttpStatusCode>(x.StatusValue))).ToList();
			client.Route = new Route(Route);
			client.NamespaceSuffix = Options.NamespaceSuffix;
			client.Obsolete = string.IsNullOrEmpty(Options.Obsolete);
			client.ObsoleteMessage = Options.Obsolete;
			client.Endpoints = Methods.Select(x => x.GetEndpoint(client)).ToList();

			return client;
		}

		public string GetText()
		{

			var fields = new List<string>();

			fields.Add($@"		public readonly I{Settings.ClientInterfaceName}Wrapper {Constants.ClientInterfaceName};");
			fields.Add($@"		public readonly {Constants.HttpOverride} {Constants.HttpOverrideField};");
			fields.Add($@"		public readonly {Constants.Serializer} {Constants.SerializerField};");
			fields.Add($@"		public readonly {Constants.RequestModifier} {Constants.RequestModifierField};");

			var classFields = string.Join(Environment.NewLine, fields);


			var parameters = new List<string>();

			parameters.Add($@"I{Settings.ClientInterfaceName}Wrapper client");
			parameters.Add($@"{Constants.HttpOverride} httpOverride");
			parameters.Add($@"{Constants.Serializer} serializer");
			parameters.Add($@"{Constants.RequestModifier} modifier");


			string @params = string.Join(", ", parameters);




			var initializers = new List<string>();

			initializers.Add($"			{Constants.ClientInterfaceName} = client;");
			initializers.Add($"			{Constants.HttpOverrideField} = httpOverride;");
			initializers.Add($"			{Constants.SerializerField} = serializer;");
			initializers.Add($"			{Constants.RequestModifierField} = modifier;");


			string init = string.Join(Environment.NewLine, initializers);

			var str =
$@"
{(Options.NamespaceSuffix != null ? $@"namespace {Options.NamespaceSuffix}
{{" : string.Empty)}

{GetObsolete()}
	public interface I{ClientName} : I{Settings.ClientInterfaceName}
	{{
{string.Join($"{Environment.NewLine}", Methods.Where(x => !x.IsNotEndpoint).Select(x => x.GetInterfaceText()))}
	}}

{GetObsolete()}
	{(Settings.UseInternalClients ? "internal" : "public")} class {ClientName} : I{ClientName}
	{{
{classFields}

		public {ClientName}({@params})
		{{
{init}
		}}

{string.Join($"{Environment.NewLine}", Methods.Where(x => !x.IsNotEndpoint).Select(x => x.GetImplementationText()))}
	}}
{(Options.NamespaceSuffix != null ? $@"}}" : string.Empty)}
";


			return str;
		}


		private string GetObsolete()
		{
			if (Options.Obsolete != null)
			{
				return $@"	[{Constants.Obsolete}(""{Options.Obsolete}"")]";
			}
			else
			{
				return string.Empty;
			}

		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class ClassOptions
	{
		public bool NoClient { get; set; }

		public string NamespaceSuffix { get; set; }

		public string Obsolete { get; set; }
		public bool Authorize { get; set; }
		public bool AllowAnonymous { get; set; }
	}
}
