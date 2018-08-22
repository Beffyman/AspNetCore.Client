using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.CSharp;
using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Framework.Parameters;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Client.Generator.Framework.Routes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Output
{
	public static class ClassParser
	{

		private static Regex RouteVersionRegex = new Regex(@"\/([v|V]\d+)\/");

		public static Controller ReadAsClient(ClassDeclarationSyntax syntax)
		{
			var attributes = syntax.AttributeLists.SelectMany(x => x.Attributes).ToList();


			var controller = new Controller();
			controller.Name = $@"{syntax.Identifier.ValueText.Trim().Replace("Controller", "")}";

			controller.Abstract = syntax.Modifiers.Any(x => x.Text == "abstract");

			controller.BaseClass = syntax.BaseList.Types.Where(x => x.ToFullString().Trim().EndsWith("Controller")).SingleOrDefault()?.ToFullString().Trim().Replace("Controller", "");

			controller.Ignored = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(NoClientAttribute.AttributeName)) != null;


			var namespaceAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(NamespaceSuffixAttribute.AttributeName));
			if (namespaceAttribute != null)
			{
				controller.NamespaceSuffix = namespaceAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
			}

			var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Route));
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				controller.Route = new Route(routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", ""));
			}

			if (controller.Route == null && !controller.Abstract)//No Route, invalid controller
			{
				controller.Ignored = true;
				throw new NotSupportedException("Controller must have a route to be valid for generation.");
			}

			if (controller.Route != null)
			{
				var match = RouteVersionRegex.Match(controller.Route.Value);
				if (match.Success)
				{
					var group = match.Groups[1];
					controller.NamespaceVersion = group.Value.ToUpper();
				}
			}


			//Response types
			var responseTypes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(Constants.ProducesResponseType));
			var responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();
			controller.ResponseTypes = responses.Select(x => new Framework.ResponseTypes.ResponseType(x.Type, Helpers.EnumParse<HttpStatusCode>(x.StatusValue))).ToList();



			var parameterHeaders = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(HeaderParameterAttribute.AttributeName))
				.Select(x => new ParameterHeaderDefinition(x))
				.ToList();
			controller.ParameterHeader = parameterHeaders.Select(x => new Framework.Headers.ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();



			var headers = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(IncludeHeaderAttribute.AttributeName))
				.Select(x => new HeaderDefinition(x))
				.ToList();
			controller.ConstantHeader = headers.Select(x => new Framework.Headers.ConstantHeader(x.Name, x.Value)).ToList();

			//Authorize Attribute
			controller.IsSecured = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Authorize)) != null;

			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Obsolete));
			if (obsoleteAttribute != null)
			{
				controller.Obsolete = true;
				controller.ObsoleteMessage = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			//Only public endpoints can be hit anyways
			var methods = syntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
				.Where(x => x.Modifiers.Any(y => y.Text == "public"))
				.ToList();
			controller.Endpoints = methods.Select(x => ReadAsEndpoint(controller, x)).ToList();

			return controller;
		}


		private static Endpoint ReadAsEndpoint(Controller parent, MethodDeclarationSyntax syntax)
		{
			var attributes = syntax.DescendantNodes().OfType<AttributeListSyntax>().SelectMany(x => x.Attributes).ToList();

			var endpoint = new Endpoint(parent);

			endpoint.Name = syntax.Identifier.ValueText.Trim();


			endpoint.Virtual = syntax.Modifiers.Any(x => x.Text == "virtual");
			endpoint.Override = syntax.Modifiers.Any(x => x.Text == "override");
			endpoint.New = syntax.Modifiers.Any(x => x.Text == "new");


			//Ignore generator attribute
			endpoint.Ignored = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(NoClientAttribute.AttributeName)) != null;


			//Route Attribute

			var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Route));
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				endpoint.Route = new Route(routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim());
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
				endpoint.Ignored = true;
			}
			else
			{
				var httpType = (HttpAttributeType)Enum.Parse(typeof(HttpAttributeType),
								httpAttribute.Name
								.ToFullString()
								.Replace(Constants.Http, "")
								.Replace(Constants.Attribute, ""));

				endpoint.HttpType = Helpers.HttpMethodFromEnum(httpType);
			}



			if (endpoint.Route == null && httpAttribute.ArgumentList != null)//If Route was never fetched from RouteAttribute or if they used the Http(template) override
			{
				endpoint.Route = new Route(httpAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim());
			}

			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Obsolete));
			if (obsoleteAttribute != null)
			{
				endpoint.Obsolete = true;
				endpoint.ObsoleteMessage = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			//Authorize Attribute
			endpoint.IsSecured = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.Authorize)) != null;


			//Response types
			var responseTypes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(Constants.ProducesResponseType));
			var responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();
			responses.Add(new ResponseTypeDefinition(true));

			endpoint.ResponseTypes = responses.Select(x => new Framework.ResponseTypes.ResponseType(x.Type, Helpers.EnumParse<HttpStatusCode>(x.StatusValue))).ToList();

			var duplicateResponseTypes = endpoint.GetResponseTypes().GroupBy(x => x.Status).Where(x => x.Count() > 1).ToList();

			if (duplicateResponseTypes.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple response types of the same status defined. {string.Join(", ", duplicateResponseTypes.Select(x => x.Key?.ToString()))}");
			}




			var parameters = syntax.ParameterList.Parameters.Select(x => new ParameterDefinition(x, endpoint.GetFullRoute(parent))).ToList();


			var routeParams = parameters.Where(x => x.Options.FromRoute).Select(x => new RouteParameter(x.RouteName, x.Type, x.Default)).ToList();
			var queryParams = parameters.Where(x => x.Options.FromQuery).Select(x => new QueryParameter(x.Options.QueryName, x.Type, x.Default)).ToList();
			var bodyParams = parameters.Where(x => x.Options.FromBody).Select(x => new BodyParameter(x.Name, x.Type, x.Default)).SingleOrDefault();


			endpoint.Parameters = routeParams.Cast<IParameter>().Union(queryParams).Union(new List<IParameter> { bodyParams }).NotNull().ToList();

			endpoint.Parameters.Add(new CancellationTokenModifier());
			endpoint.Parameters.Add(new CookieModifier());
			endpoint.Parameters.Add(new HeadersModifier());
			endpoint.Parameters.Add(new TimeoutModifier());
			if (endpoint.IsSecured)
			{
				endpoint.Parameters.Add(new SecurityModifier());
			}


			var parameterHeaders = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(HeaderParameterAttribute.AttributeName))
				.Select(x => new ParameterHeaderDefinition(x))
				.ToList();
			endpoint.ParameterHeader = parameterHeaders.Select(x => new Framework.Headers.ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();



			var headers = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(IncludeHeaderAttribute.AttributeName))
				.Select(x => new HeaderDefinition(x))
				.ToList();
			endpoint.ConstantHeader = headers.Select(x => new Framework.Headers.ConstantHeader(x.Name, x.Value)).ToList();


			var actionResultReturn = syntax.ReturnType.ToFullString().Contains(Constants.IActionResult);

			var returnType = syntax.ReturnType?.ToFullString();
			if (!actionResultReturn)
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
			endpoint.ReturnType = returnType;



			var duplicateParameters = endpoint.GetParametersWithoutResponseTypes().GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();

			if (duplicateParameters.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple parameters of the same name defined. {string.Join(", ", duplicateParameters.Select(x => x.Key?.ToString()))}");
			}




			return endpoint;
		}
	}
}
