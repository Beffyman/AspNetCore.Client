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

namespace AspNetCore.Client.Generator.RoslynParser
{
	public class ClassDefinition
	{
		public string Name { get; }
		public string NamespaceVersion { get; }

		public IList<MethodDefinition> Methods { get; }
		public IList<ResponseTypeDefinition> Responses { get; }

		public string Route { get; }
		public IList<ParameterHeaderDefinition> ParameterHeaders { get; }
		public IList<HeaderDefinition> Headers { get; }

		private static Regex RouteVersionRegex = new Regex(@"\/([v|V]\d+)\/");

		public ClassOptions Options { get; set; }
		

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
