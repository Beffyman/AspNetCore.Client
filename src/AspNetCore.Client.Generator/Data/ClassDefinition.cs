using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Data
{
	public class ClassDefinition
	{
		public string Name { get; }
		public string ControllerName => Name.Replace("Controller", "");
		public string ClientName => Name.Replace("Controller", "Client");
		public string Namespace { get; }
		public string NamespaceVersion { get; }

		public ClassDeclarationSyntax ClassDeclaration;
		public ParsedFile File { get; }

		public IList<AttributeSyntax> Attributes { get; }
		public IList<MethodDefinition> Methods { get; }

		public string Route { get; }
		public IList<HeaderDefinition> Headers { get; }

		private static Regex RouteVersionRegex = new Regex(@"\/([v|V]\d+)\/");

		public ClassOptions Options { get; set; }

		public bool NotEmpty => !Options.NoClient && Methods.Any(x => !x.IsNotEndpoint);

		public ClassDefinition(string @namespace,
			string className,
			ParsedFile file,
			ClassDeclarationSyntax classDeclaration,
			IList<AttributeSyntax> attributes,
			IList<MethodDeclarationSyntax> methods)
		{
			Name = className;
			Namespace = @namespace;
			File = file;
			ClassDeclaration = classDeclaration;
			Attributes = attributes;

			Options = new ClassOptions();

			var ignoreAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith(AspNetCore.Client.Core.NoClientAttribute.AttributeName));
			if (ignoreAttribute != null)
			{
				Options.NoClient = true;
			}


			var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith(Constants.Route));
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				Route = routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
			}


			var match = RouteVersionRegex.Match(Route);
			if (match.Success)
			{
				var group = match.Groups[1];
				NamespaceVersion = group.Value.ToUpper();
			}


			Headers = attributes.Where(x => x.Name.ToFullString().StartsWith(AspNetCore.Client.Core.IncludesHeaderAttribute.AttributeName))
				.Select(x => new HeaderDefinition(x))
				.ToList();

			//Authorize Attribute
			Options.Authorize = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith(Constants.Authorize)) != null;

			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().StartsWith(Constants.Obsolete));
			if (obsoleteAttribute != null)
			{
				Options.Obsolete = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			Methods = methods.Select(x => new MethodDefinition(this, x)).ToList();

		}


		public string GetText()
		{

			var fields = new List<string>();

			fields.Add($@"		public readonly {Settings.Instance.ClientInterfaceName} {Constants.ClientInterfaceName};");
			if (Settings.Instance.IncludeHttpOverride)
			{
				fields.Add($@"		public readonly {Constants.HttpOverride} {Constants.HttpOverrideField};");
			}

			var classFields = string.Join(Environment.NewLine, fields);


			var parameters = new List<string>();

			parameters.Add($@"{Settings.Instance.ClientInterfaceName} client");

			if (Settings.Instance.IncludeHttpOverride)
			{
				parameters.Add($@"{Constants.HttpOverride} httpOverride");
			}


			string @params = string.Join(", ", parameters);




			var initializers = new List<string>();

			initializers.Add($"			{Constants.ClientInterfaceName} = client;");
			if (Settings.Instance.IncludeHttpOverride)
			{
				initializers.Add($"			{Constants.HttpOverrideField} = httpOverride;");
			}

			string init = string.Join(Environment.NewLine, initializers);

			var str =
$@"
{GetObsolete()}
	public interface I{ClientName} : I{Settings.Instance.ClientInterfaceName}
	{{
{string.Join($"{Environment.NewLine}", Methods.Where(x => !x.IsNotEndpoint).Select(x => x.GetInterfaceText()))}
	}}

{GetObsolete()}
	public class {ClientName} : I{ClientName}
	{{
{classFields}

		public {ClientName}({@params})
		{{
{init}
		}}

{string.Join($"{Environment.NewLine}", Methods.Where(x => !x.IsNotEndpoint).Select(x => x.GetImplementationText()))}
	}}
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

		public string Obsolete { get; set; }
		public bool Authorize { get; set; }
		public bool AllowAnonymous { get; set; }
	}
}
