using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using AspNetCore.Client.Generator.CSharp.AspNetCoreFunctions;
using AspNetCore.Client.Generator.CSharp.AspNetCoreHttp;
using AspNetCore.Client.Generator.CSharp.SignalR;
using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Functions;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Headers;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Parameters;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.RequestModifiers;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.ResponseTypes;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Client.Generator.Framework.SignalR;
using AspNetCore.Client.Generator.SignalR;
using AspNetCore.Server.Attributes;
using AspNetCore.Server.Attributes.Functions;
using AspNetCore.Server.Attributes.Http;
using AspNetCore.Server.Attributes.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCore.Client.Generator.Output
{
	public static class ClassParser
	{
		private static readonly Regex RouteVersionRegex = new Regex(@"\/([v|V]\d+)\/");

		#region Http

		public static AspNetCoreHttpController ReadClassAsHttpController(ClassDeclarationSyntax syntax)
		{
			var attributes = syntax.AttributeLists.SelectMany(x => x.Attributes).ToList();


			var controller = new AspNetCoreHttpController();
			try
			{
				controller.Name = $@"{syntax.Identifier.ValueText.Trim().Replace("Controller", "")}";

				controller.Abstract = syntax.Modifiers.Any(x => x.Text == "abstract");

				if (syntax.BaseList == null)
				{
					controller.Ignored = true;
					return controller;
				}

				controller.BaseClass = syntax.BaseList.Types.Where(x => x.ToFullString().Trim().EndsWith("Controller")).SingleOrDefault()?.ToFullString().Trim().Replace("Controller", "");

				controller.Ignored = attributes.HasAttribute<NotGeneratedAttribute>();


				var namespaceAttribute = attributes.GetAttribute<NamespaceSuffixAttribute>();
				if (namespaceAttribute != null)
				{
					controller.NamespaceSuffix = namespaceAttribute.GetAttributeValue();
				}

				var routeAttribute = attributes.GetAttribute<RouteAttribute>();
				if (routeAttribute != null)//Fetch route from RouteAttribute
				{
					controller.Route = new HttpRoute(routeAttribute.GetAttributeValue());
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
				var responseTypes = attributes.GetAttributes<ProducesResponseTypeAttribute>();
				var responses = responseTypes.Select(x => new ResponseTypeDefinition(x)).ToList();
				controller.ResponseTypes = responses.Select(x => new ResponseType(x.Type, Helpers.EnumParse<HttpStatusCode>(x.StatusValue))).ToList();



				var parameterHeaders = attributes.GetAttributes<HeaderParameterAttribute>()
					.Select(x => new ParameterHeaderDefinition(x))
					.ToList();
				controller.ParameterHeader = parameterHeaders.Select(x => new ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();



				var headers = attributes.GetAttributes<IncludeHeaderAttribute>()
					.Select(x => new HeaderDefinition(x))
					.ToList();
				controller.ConstantHeader = headers.Select(x => new ConstantHeader(x.Name, x.Value)).ToList();

				//Authorize Attribute
				controller.IsSecured = attributes.HasAttribute<AuthorizeAttribute>();

				//Obsolete Attribute
				var obsoleteAttribute = attributes.GetAttribute<ObsoleteAttribute>();
				if (obsoleteAttribute != null)
				{
					controller.Obsolete = true;
					controller.ObsoleteMessage = obsoleteAttribute.GetAttributeValue();
				}

				//Only public endpoints can be hit anyways
				var methods = syntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
					.Where(x => x.Modifiers.Any(y => y.Text == "public"))
					.ToList();
				controller.Endpoints = methods.Select(x => ReadMethodAsHttpEndpoint(controller, x)).ToList();

				if (!controller.Endpoints.Any(x => !x.Ignored))
				{
					controller.Ignored = true;
				}
			}
			catch (NotSupportedException nse)
			{
				if (controller.Ignored)
				{
					return controller;
				}

				controller.Failed = true;
				controller.Error = nse.Message;
			}
#if !DEBUG
			catch (Exception ex)
			{
				controller.Failed = true;
				controller.UnexpectedFailure = true;
				controller.Error = ex.ToString();
			}
#endif
			return controller;
		}


		private static AspNetCoreHttpEndpoint ReadMethodAsHttpEndpoint(AspNetCoreHttpController parent, MethodDeclarationSyntax syntax)
		{
			var attributes = syntax.DescendantNodes().OfType<AttributeListSyntax>().SelectMany(x => x.Attributes).ToList();

			var endpoint = new AspNetCoreHttpEndpoint(parent);

			endpoint.Name = syntax.Identifier.ValueText.CleanMethodName();


			endpoint.Virtual = syntax.Modifiers.Any(x => x.Text == "virtual");
			endpoint.Override = syntax.Modifiers.Any(x => x.Text == "override");
			endpoint.New = syntax.Modifiers.Any(x => x.Text == "new");


			//Ignore generator attribute
			endpoint.Ignored = attributes.HasAttribute<NotGeneratedAttribute>();


			//Route Attribute

			var routeAttribute = attributes.GetAttribute<RouteAttribute>();
			if (routeAttribute != null)//Fetch route from RouteAttribute
			{
				endpoint.Route = new HttpRoute(routeAttribute.GetAttributeValue());
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
				endpoint.Route = new HttpRoute(httpAttribute.GetAttributeValue());
			}

			//Ignore method if it doesn't have a route or http attribute
			if (endpoint.Route == null && httpAttribute == null)
			{
				endpoint.Ignored = true;
				return endpoint;
			}


			//Obsolete Attribute
			var obsoleteAttribute = attributes.GetAttribute<ObsoleteAttribute>();
			if (obsoleteAttribute != null)
			{
				endpoint.Obsolete = true;
				endpoint.ObsoleteMessage = obsoleteAttribute.GetAttributeValue();
			}

			//Authorize Attribute
			endpoint.IsSecured = attributes.HasAttribute<AuthorizeAttribute>();


			//Response types
			var responseTypes = attributes.GetAttributes<ProducesResponseTypeAttribute>();
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
			var queryParams = parameters.Where(x => x.Options.FromQuery).Select(x => new QueryParameter(x.Options.QueryName, x.Type, x.Default, x.Options.QueryObject)).ToList();
			var bodyParam = parameters.Where(x => x.Options.FromBody).Select(x => new BodyParameter(x.Name, x.Type, x.Default)).SingleOrDefault();


			endpoint.Parameters = routeParams.Cast<IParameter>().Union(queryParams).Union(new List<IParameter> { bodyParam }).NotNull().ToList();

			endpoint.Parameters.Add(new CancellationTokenModifier());
			endpoint.Parameters.Add(new CookieModifier());
			endpoint.Parameters.Add(new HeadersModifier());
			endpoint.Parameters.Add(new TimeoutModifier());
			if (endpoint.IsSecured)
			{
				endpoint.Parameters.Add(new SecurityModifier());
			}


			var parameterHeaders = attributes.GetAttributes<HeaderParameterAttribute>()
				.Select(x => new ParameterHeaderDefinition(x))
				.ToList();
			endpoint.ParameterHeader = parameterHeaders.Select(x => new ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();



			var headers = attributes.GetAttributes<IncludeHeaderAttribute>()
				.Select(x => new HeaderDefinition(x))
				.ToList();
			endpoint.ConstantHeader = headers.Select(x => new ConstantHeader(x.Name, x.Value)).ToList();


			var rawReturnType = syntax.ReturnType?.ToFullString();

			var returnType = Helpers.GetTypeFromString(rawReturnType.Trim());

			while (returnType.IsContainerReturnType())
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

			if (returnType.IsFileReturnType())
			{
				returnType = new Helpers.TypeString(typeof(Stream).FullName);
				endpoint.ReturnsStream = true;
			}

			rawReturnType = returnType?.ToString();

			endpoint.ReturnType = rawReturnType?.Trim();



			var duplicateParameters = endpoint.GetParametersWithoutResponseTypes().GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();

			if (duplicateParameters.Any())
			{
				throw new NotSupportedException($"Endpoint {parent.Name}.{endpoint.Name} has multiple parameters of the same name defined. {string.Join(", ", duplicateParameters.Select(x => x.Key?.ToString()))}");
			}

			var invalidParameters = endpoint.GetParameters().Where(x => !Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsValidIdentifier(x.Name)).ToList();

			if (invalidParameters.Any())
			{
				throw new NotSupportedException($"Endpoint {parent.Name}.{endpoint.Name} has parameters that are invalid variable names. {string.Join(", ", invalidParameters.Select(x => x.Name))}");
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

				if (syntax.BaseList == null)
				{
					controller.Ignored = true;
					return controller;
				}

				var generatedAttribute = attributes.GetAttribute<GenerateHubAttribute>();
				if (generatedAttribute == null)
				{
					controller.Ignored = true;
					return controller;
				}

				controller.BaseClass = syntax.BaseList.Types.Where(x => x.ToFullString().Trim().EndsWith("Hub")).SingleOrDefault()?.ToFullString().Trim().Replace("Hub", "");

				controller.Ignored = attributes.HasAttribute<NotGeneratedAttribute>();


				var namespaceAttribute = attributes.GetAttribute<NamespaceSuffixAttribute>();
				if (namespaceAttribute != null)
				{
					controller.NamespaceSuffix = namespaceAttribute.ArgumentList.Arguments.ToFullString().Replace("\"", "");
				}

				var routeAttribute = attributes.GetAttribute<RouteAttribute>();
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
				var obsoleteAttribute = attributes.GetAttribute<ObsoleteAttribute>();
				if (obsoleteAttribute != null)
				{
					controller.Obsolete = true;
					controller.ObsoleteMessage = obsoleteAttribute.GetAttributeValue();
				}

				//Only public endpoints can be hit anyways
				var methods = syntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
					.Where(x => x.Modifiers.Any(y => y.Text == "public"))
					.ToList();
				controller.Endpoints = methods.Select(x => ReadMethodAsHubEndpoint(controller, x)).ToList();

				if (!controller.Endpoints.Any(x => !x.Ignored))
				{
					controller.Ignored = true;
				}
			}
			catch (NotSupportedException nse)
			{
				if (controller.Ignored)
				{
					return controller;
				}

				controller.Failed = true;
				controller.Error = nse.Message;
			}
#if !DEBUG
			catch (Exception ex)
			{
				controller.Failed = true;
				controller.UnexpectedFailure = true;
				controller.Error = ex.ToString();
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
			endpoint.Ignored = attributes.HasAttribute<NotGeneratedAttribute>();

			//Obsolete Attribute
			var obsoleteAttribute = attributes.GetAttribute<ObsoleteAttribute>();
			if (obsoleteAttribute != null)
			{
				endpoint.Obsolete = true;
				endpoint.ObsoleteMessage = obsoleteAttribute.GetAttributeValue();
			}

			//Response types
			var messageAttributes = attributes.GetAttributes<ProducesMessageAttribute>();
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

			var invalidParameters = endpoint.GetParameters().Where(x => !Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsValidIdentifier(x.Name)).ToList();

			if (invalidParameters.Any())
			{
				throw new NotSupportedException($"Endpoint {parent.Name}.{endpoint.Name} has parameters that are invalid variable names. {string.Join(", ", invalidParameters.Select(x => x.Name))}");
			}


			var rawReturnType = syntax.ReturnType?.ToFullString();

			var returnType = Helpers.GetTypeFromString(rawReturnType.Trim());

			while (returnType.IsContainerReturnType())
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


		#region Functions

		public static FunctionEndpoint ReadMethodAsFunction(MethodDeclarationSyntax syntax)
		{
			var attributes = syntax.DescendantNodes().OfType<AttributeListSyntax>().SelectMany(x => x.Attributes).ToList();

			var endpoint = new FunctionEndpoint();
			try
			{
				var endpointName = attributes.GetAttribute<FunctionNameAttribute>();

				if (endpointName == null)
				{
					endpoint.Ignored = true;
					return endpoint;
				}

				endpoint.Name = endpointName.GetAttributeValue();

				//Ignore generator attribute
				endpoint.Ignored = attributes.HasAttribute<NotGeneratedAttribute>();


				//Obsolete Attribute
				var obsoleteAttribute = attributes.GetAttribute<ObsoleteAttribute>();
				if (obsoleteAttribute != null)
				{
					endpoint.Obsolete = true;
					endpoint.ObsoleteMessage = obsoleteAttribute.GetAttributeValue();
				}

				//Response types
				var responseTypes = attributes.GetAttributes<ProducesResponseTypeAttribute>();
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


				//Need to check if the function has a HttpTrigger
				var httpTriggerAttribute = syntax.ParameterList.Parameters.SingleOrDefault(x => x.AttributeLists.SelectMany(y => y.Attributes).HasAttribute<HttpTriggerAttribute>());

				if (httpTriggerAttribute == null)
				{
					endpoint.Ignored = true;
					return endpoint;
				}

				var triggerAttribute = new HttpTriggerParameter(httpTriggerAttribute);

				endpoint.SupportedMethods = triggerAttribute.Methods;

				if (triggerAttribute.Route != null)
				{
					var route = triggerAttribute.Route.TrimStart('/');
					if (!route.StartsWith("api"))
					{
						route = "api/" + route;
					}
					route = "/" + route;
					endpoint.Route = new HttpRoute(route);
				}
				else
				{
					endpoint.Route = new HttpRoute($"api/{endpoint.Name}");
				}


				var expectedBodyParameters = attributes.GetAttributes<ExpectedBodyParameterAttribute>()
					.Select(x => new ExpectedBodyParamterDefinition(x))
					.GroupBy(x => x.Method)
					.ToDictionary(x => x.Key, y => y.Select(z => (IParameter)new BodyParameter("body", z.Type, null)));

				var expectedQueryParameters = attributes.GetAttributes<ExpectedQueryParameterAttribute>()
					.Select(x => new ExpectedQueryParamterDefinition(x))
					.GroupBy(x => x.Method)
					.ToDictionary(x => x.Key, y => y.Select(z => (IParameter)new QueryParameter(z.Name, z.Type, null, false)));

				endpoint.HttpParameters = expectedBodyParameters.Union(expectedQueryParameters).ToDictionary();

				var parameters = syntax.ParameterList.Parameters.Select(x => new ParameterDefinition(x, endpoint.GetFullRoute())).ToList();

				var routeParams = parameters.Where(x => x.Options.FromRoute).Select(x => new RouteParameter(x.RouteName, x.Type, x.Default)).ToList();

				endpoint.Parameters = routeParams.Cast<IParameter>().NotNull().ToList();

				endpoint.Parameters.Add(new CancellationTokenModifier());
				endpoint.Parameters.Add(new CookieModifier());
				endpoint.Parameters.Add(new HeadersModifier());
				endpoint.Parameters.Add(new TimeoutModifier());

				if (triggerAttribute.AuthLevel == AuthorizationLevel.User)
				{
					if (!endpoint.ResponseTypes.Any(x => x.Status == HttpStatusCode.Unauthorized))
					{
						endpoint.ResponseTypes.Add(new ResponseType(HttpStatusCode.Unauthorized));
					}

					endpoint.Parameters.Add(new SecurityModifier());
				}
				else if (triggerAttribute.AuthLevel == AuthorizationLevel.Anonymous)
				{

				}
				else
				{
					if (!endpoint.ResponseTypes.Any(x => x.Status == HttpStatusCode.Unauthorized))
					{
						endpoint.ResponseTypes.Add(new ResponseType(HttpStatusCode.Unauthorized));
					}

					endpoint.Parameters.Add(new FunctionAuthModifier());
				}


				var parameterHeaders = attributes.GetAttributes<HeaderParameterAttribute>()
					.Select(x => new ParameterHeaderDefinition(x))
					.ToList();
				endpoint.ParameterHeader = parameterHeaders.Select(x => new ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();



				var headers = attributes.GetAttributes<IncludeHeaderAttribute>()
					.Select(x => new HeaderDefinition(x))
					.ToList();
				endpoint.ConstantHeader = headers.Select(x => new ConstantHeader(x.Name, x.Value)).ToList();


				var rawReturnType = syntax.ReturnType?.ToFullString();

				var returnType = Helpers.GetTypeFromString(rawReturnType.Trim());

				while (returnType.IsContainerReturnType())
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

				if (returnType.IsFileReturnType())
				{
					returnType = new Helpers.TypeString(typeof(Stream).FullName);
					endpoint.ReturnsStream = true;
				}

				rawReturnType = returnType?.ToString();

				endpoint.ReturnType = rawReturnType?.Trim();

				foreach (var method in endpoint.SupportedMethods)
				{
					var duplicateParameters = endpoint.GetParametersWithoutResponseTypesForHttpMethod(method).GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();

					if (duplicateParameters.Any())
					{
						throw new NotSupportedException($"Function has multiple parameters of the same name defined. {string.Join(", ", duplicateParameters.Select(x => x.Key?.ToString()))}");
					}


					var invalidParameters = endpoint.GetParametersForHttpMethod(method).Where(x => !Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsValidIdentifier(x.Name)).ToList();

					if (invalidParameters.Any())
					{
						throw new NotSupportedException($"Function {endpoint.Name} has parameters that are invalid variable names. {string.Join(", ", invalidParameters.Select(x => x.Name))}");
					}
				}
			}
			catch (NotSupportedException nse)
			{
				if (endpoint.Ignored)
				{
					return endpoint;
				}

				endpoint.Failed = true;
				endpoint.Error = nse.Message;
			}
#if !DEBUG
			catch (Exception ex)
			{
				endpoint.Failed = true;
				endpoint.UnexpectedFailure = true;
				endpoint.Error = ex.ToString();
			}
#endif


			return endpoint;
		}

		#endregion
	}
}