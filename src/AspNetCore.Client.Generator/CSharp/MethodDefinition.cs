using AspNetCore.Client;
using AspNetCore.Client.Attributes;
using AspNetCore.Client.Authorization;
using AspNetCore.Client.Serializers;
using Flurl.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Framework.Routes;

namespace AspNetCore.Client.Generator.CSharp
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

			var httpAttribute = attributes.SingleOrDefault(x => knownHttpAttributes.Any(y => x.Name.ToFullString().MatchesAttribute(y)));
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


			//Response types
			var responseTypes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(Constants.ProducesResponseType));
			Responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();
			Responses.Add(new ResponseTypeDefinition(true));

			Parameters = MethodSyntax.ParameterList.Parameters.Select(x => new ParameterDefinition(x,FullRouteTemplate)).ToList();


			ParameterHeaders = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(HeaderParameterAttribute.AttributeName))
				.Select(x => new ParameterHeaderDefinition(x))
				.ToList();

			Headers = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(IncludeHeaderAttribute.AttributeName))
				.Select(x => new HeaderDefinition(x))
				.ToList();

			Options.ActionResultReturn = MethodSyntax.ReturnType.ToFullString().Contains(Constants.IActionResult);

			Options.ReturnType = MethodSyntax.ReturnType?.ToFullString();
			if (!Options.ActionResultReturn)
			{
				var regex = new Regex(@"(ValueTask|Task|ActionResult)<(.+)>");
				var match = regex.Match(Options.ReturnType);
				if (match.Success)
				{
					Options.ReturnType = match.Groups[2].Value;
				}

				Options.ReturnType = Options.ReturnType.Trim();


				if (Options.ReturnType == "void" || Options.ReturnType == "Task")
				{
					Options.ReturnType = null;
				}
			}
			else
			{
				Options.ReturnType = null;
			}
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
		public string ReturnType { get; set; }
		public bool Authorize { get; set; }
	}
}
