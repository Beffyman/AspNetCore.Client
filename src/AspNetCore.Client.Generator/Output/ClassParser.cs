using AspNetCore.Client.Attributes.Http;
using AspNetCore.Client.Attributes.SignalR;
using AspNetCore.Client.Generator.CSharp.Http;
using AspNetCore.Client.Generator.CSharp.SignalR;
using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Framework.Http;
using AspNetCore.Client.Generator.Framework.Http.Headers;
using AspNetCore.Client.Generator.Framework.Http.Parameters;
using AspNetCore.Client.Generator.Framework.Http.RequestModifiers;
using AspNetCore.Client.Generator.Framework.Http.ResponseTypes;
using AspNetCore.Client.Generator.Framework.Http.Routes;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Client.Generator.Framework.SignalR;
using AspNetCore.Client.Generator.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Output
{
	public static class ClassParser
	{

		private static Regex RouteVersionRegex = new Regex(@"\/([v|V]\d+)\/");

		#region Http

		public static HttpController ReadClassAsHttpController(ClassDeclarationSyntax syntax)
		{
			var attributes = syntax.AttributeLists.SelectMany(x => x.Attributes).ToList();


			var controller = new HttpController();
			try
			{
				controller.Name = $@"{syntax.Identifier.ValueText.Trim().Replace("Controller", "")}";

				controller.Abstract = syntax.Modifiers.Any(x => x.Text == "abstract");

				controller.BaseClass = syntax.BaseList.Types.Where(x => x.ToFullString().Trim().EndsWith("Controller")).SingleOrDefault()?.ToFullString().Trim().Replace("Controller", "");

				controller.Ignored = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(NotGeneratedAttribute))) != null;


				var namespaceAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(NamespaceSuffixAttribute)));
				if (namespaceAttribute != null)
				{
					controller.NamespaceSuffix = namespaceAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
				}

				var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(RouteAttribute)));
				if (routeAttribute != null)//Fetch route from RouteAttribute
				{
					controller.Route = new HttpRoute(routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", ""));
				}

				if (controller.Route == null && !controller.Abstract && !controller.Ignored)//No Route, invalid controller
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
				var responseTypes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(nameof(ProducesResponseTypeAttribute)));
				var responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();
				controller.ResponseTypes = responses.Select(x => new ResponseType(x.Type, Helpers.EnumParse<HttpStatusCode>(x.StatusValue))).ToList();



				var parameterHeaders = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(nameof(HeaderParameterAttribute)))
					.Select(x => new ParameterHeaderDefinition(x))
					.ToList();
				controller.ParameterHeader = parameterHeaders.Select(x => new ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();



				var headers = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(nameof(IncludeHeaderAttribute)))
					.Select(x => new HeaderDefinition(x))
					.ToList();
				controller.ConstantHeader = headers.Select(x => new ConstantHeader(x.Name, x.Value)).ToList();

				//Authorize Attribute
				controller.IsSecured = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(AuthorizeAttribute))) != null;

				//Obsolete Attribute
				var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(ObsoleteAttribute)));
				if (obsoleteAttribute != null)
				{
					controller.Obsolete = true;
					controller.ObsoleteMessage = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
				}

				//Only public endpoints can be hit anyways
				var methods = syntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
					.Where(x => x.Modifiers.Any(y => y.Text == "public"))
					.ToList();
				controller.Endpoints = methods.Select(x => ReadMethodAsHttpEndpoint(controller, x)).ToList();

			}
			catch (NotSupportedException nse)
			{
				controller.Failed = true;
				controller.Error = nse.Message;
			}
#if !DEBUG
			catch (Exception ex)
			{
				controller.Failed = true;
				controller.UnexpectedFailure = true;
				controller.Error = ex.Message;
			}
#endif
			return controller;
		}


		private static HttpEndpoint ReadMethodAsHttpEndpoint(HttpController parent, MethodDeclarationSyntax syntax)
		{
			var attributes = syntax.DescendantNodes().OfType<AttributeListSyntax>().SelectMany(x => x.Attributes).ToList();

			var endpoint = new HttpEndpoint(parent);

			endpoint.Name = syntax.Identifier.ValueText.CleanMethodName();


			endpoint.Virtual = syntax.Modifiers.Any(x => x.Text == "virtual");
			endpoint.Override = syntax.Modifiers.Any(x => x.Text == "override");
			endpoint.New = syntax.Modifiers.Any(x => x.Text == "new");


			//Ignore generator attribute
			endpoint.Ignored = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(NotGeneratedAttribute))) != null;


			//Route Attribute

			var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(RouteAttribute)));
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				endpoint.Route = new HttpRoute(routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim());
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



			if (endpoint.Route == null && httpAttribute?.ArgumentList != null)//If Route was never fetched from RouteAttribute or if they used the Http(template) override
			{
				endpoint.Route = new HttpRoute(httpAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim());
			}

			//Ignore method if it doesn't have a route or http attribute
			if (endpoint.Route == null && httpAttribute == null)
			{
				endpoint.Ignored = true;
				return endpoint;
			}


			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(ObsoleteAttribute)));
			if (obsoleteAttribute != null)
			{
				endpoint.Obsolete = true;
				endpoint.ObsoleteMessage = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			//Authorize Attribute
			endpoint.IsSecured = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(AuthorizeAttribute))) != null;


			//Response types
			var responseTypes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(nameof(ProducesResponseTypeAttribute)));
			var responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();
			responses.Add(new ResponseTypeDefinition(true));

			endpoint.ResponseTypes = responses.Select(x => new ResponseType(x.Type, Helpers.EnumParse<HttpStatusCode>(x.StatusValue))).ToList();

			var duplicateResponseTypes = endpoint.GetResponseTypes().GroupBy(x => x.Status).Where(x => x.Count() > 1).ToList();

			if (duplicateResponseTypes.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple response types of the same status defined. {string.Join(", ", duplicateResponseTypes.Select(x => x.Key?.ToString()))}");
			}
			//Add after so we don't get duplicate error from the null Status
			endpoint.ResponseTypes.Add(new ExceptionResponseType());




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


			var parameterHeaders = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(nameof(HeaderParameterAttribute)))
				.Select(x => new ParameterHeaderDefinition(x))
				.ToList();
			endpoint.ParameterHeader = parameterHeaders.Select(x => new ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();



			var headers = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(nameof(IncludeHeaderAttribute)))
				.Select(x => new HeaderDefinition(x))
				.ToList();
			endpoint.ConstantHeader = headers.Select(x => new ConstantHeader(x.Name, x.Value)).ToList();


			var rawReturnType = syntax.ReturnType?.ToFullString();

			HashSet<string> returnContainerTypes = new HashSet<string>()
			{
				typeof(ValueTask).FullName,
				typeof(Task).FullName,
				typeof(ActionResult).FullName
			};


			var returnType = Helpers.GetTypeFromString(rawReturnType.Trim());

			while (returnContainerTypes.Any(x => Helpers.IsType(x, returnType?.Name)))
			{
				returnType = returnType.Arguments.SingleOrDefault();
			}

			if (Helpers.IsType(typeof(IActionResult).FullName, returnType?.Name))
			{
				returnType = null;
			}

			if (returnType?.Name == "void"
				|| (Helpers.IsType(typeof(Task).FullName, returnType?.Name) && (!returnType?.Arguments.Any() ?? false)))
			{
				returnType = null;
			}

			HashSet<string> fileResults = new HashSet<string>()
			{
				nameof(PhysicalFileResult),
				nameof(FileResult),
				nameof(FileContentResult),
				nameof(FileStreamResult),
				nameof(VirtualFileResult)
			};

			if (fileResults.Any(x => Helpers.IsType(x, returnType?.Name)))
			{
				returnType = new Helpers.TypeString(typeof(Stream).FullName);
				endpoint.ReturnsStream = true;
			}

			rawReturnType = returnType?.ToString();

			endpoint.ReturnType = rawReturnType;



			var duplicateParameters = endpoint.GetParametersWithoutResponseTypes().GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();

			if (duplicateParameters.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple parameters of the same name defined. {string.Join(", ", duplicateParameters.Select(x => x.Key?.ToString()))}");
			}




			return endpoint;
		}

		#endregion Http


		#region SignalR

		public static HubController ReadClassAsHubController(ClassDeclarationSyntax syntax)
		{
			var attributes = syntax.AttributeLists.SelectMany(x => x.Attributes).ToList();


			var controller = new HubController();
			try
			{
				controller.Name = $@"{syntax.Identifier.ValueText.Trim().Replace("Hub", "")}";

				controller.Abstract = syntax.Modifiers.Any(x => x.Text == "abstract");

				controller.BaseClass = syntax.BaseList.Types.Where(x => x.ToFullString().Trim().EndsWith("Hub")).SingleOrDefault()?.ToFullString().Trim().Replace("Hub", "");

				controller.Ignored = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(NotGeneratedAttribute))) != null;


				var namespaceAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(NamespaceSuffixAttribute)));
				if (namespaceAttribute != null)
				{
					controller.NamespaceSuffix = namespaceAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
				}

				var routeAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(RouteAttribute)));
				if (routeAttribute != null)//Fetch route from RouteAttribute
				{
					controller.Route = routeAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
				}

				if (controller.Route == null && !controller.Abstract && !controller.Ignored)//No Route, invalid controller
				{
					controller.Ignored = true;
					throw new NotSupportedException("Controller must have a route to be valid for generation.");
				}

				if (controller.Route != null)
				{
					var match = RouteVersionRegex.Match(controller.Route);
					if (match.Success)
					{
						var group = match.Groups[1];
						controller.NamespaceVersion = group.Value.ToUpper();
					}
				}

				//Obsolete Attribute
				var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(ObsoleteAttribute)));
				if (obsoleteAttribute != null)
				{
					controller.Obsolete = true;
					controller.ObsoleteMessage = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
				}

				//Only public endpoints can be hit anyways
				var methods = syntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
					.Where(x => x.Modifiers.Any(y => y.Text == "public"))
					.ToList();
				controller.Endpoints = methods.Select(x => ReadMethodAsHubEndpoint(controller, x)).ToList();

			}
			catch (NotSupportedException nse)
			{
				controller.Failed = true;
				controller.Error = nse.Message;
			}
#if !DEBUG
			catch (Exception ex)
			{
				controller.Failed = true;
				controller.UnexpectedFailure = true;
				controller.Error = ex.Message;
			}
#endif
			return controller;
		}



		private static HubEndpoint ReadMethodAsHubEndpoint(HubController parent, MethodDeclarationSyntax syntax)
		{
			var attributes = syntax.DescendantNodes().OfType<AttributeListSyntax>().SelectMany(x => x.Attributes).ToList();

			var endpoint = new HubEndpoint(parent);

			endpoint.Name = syntax.Identifier.ValueText.CleanMethodName();


			endpoint.Virtual = syntax.Modifiers.Any(x => x.Text == "virtual");
			endpoint.Override = syntax.Modifiers.Any(x => x.Text == "override");
			endpoint.New = syntax.Modifiers.Any(x => x.Text == "new");


			//Ignore generator attribute
			endpoint.Ignored = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(NotGeneratedAttribute))) != null;

			//Obsolete Attribute
			var obsoleteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(ObsoleteAttribute)));
			if (obsoleteAttribute != null)
			{
				endpoint.Obsolete = true;
				endpoint.ObsoleteMessage = obsoleteAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
			}

			//Response types
			var messageAttributes = attributes.Where(x => x.Name.ToFullString().MatchesAttribute(nameof(ProducesMessageAttribute)));
			var messages = messageAttributes.Select(x => new MessageDefinition(x)).ToList();

			endpoint.Messages = messages.Select(x => new Message(x.Name, x.Types)).ToList();

			var duplicateMessages = endpoint.Messages.GroupBy(x => x.Name).Where(x => x.Count() > 1 && !x.All(y => y.Types.SequenceEqual(x.First().Types))).ToList();

			if (duplicateMessages.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple messages with different parameters defined. {string.Join(", ", duplicateMessages.Select(x => x.Key?.ToString()))}");
			}




			var parameters = syntax.ParameterList.Parameters.Select(x => new HubParameterDefinition(x)).ToList();
			var hubParams = parameters.Select(x => new HubParameter(x.Name, x.Type, x.Default)).ToList();

			endpoint.Parameters = hubParams.Cast<IParameter>().NotNull().ToList();

			var duplicateParameters = endpoint.GetParameters().GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();

			if (duplicateParameters.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple parameters of the same name defined. {string.Join(", ", duplicateParameters.Select(x => x.Key?.ToString()))}");
			}


			var rawReturnType = syntax.ReturnType?.ToFullString();

			HashSet<string> returnContainerTypes = new HashSet<string>()
			{
				typeof(ValueTask).FullName,
				typeof(Task).FullName
			};


			var returnType = Helpers.GetTypeFromString(rawReturnType.Trim());

			while (returnContainerTypes.Any(x => Helpers.IsType(x, returnType?.Name)))
			{
				returnType = returnType.Arguments.SingleOrDefault();
			}

			if (Helpers.IsType(typeof(ChannelReader<>).FullName.CleanGenericTypeDefinition(), returnType?.Name))
			{
				endpoint.Channel = true;
				endpoint.ChannelType = returnType.Arguments.SingleOrDefault().ToString();
			}


			return endpoint;
		}




		#endregion SignalR
	}
}
