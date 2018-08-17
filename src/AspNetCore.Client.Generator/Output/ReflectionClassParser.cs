using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Framework.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Output
{
	public static class ReflectionClassParser
	{

		private static Regex RouteVersionRegex = new Regex(@"\/([v|V]\d+)\/");

		public static Framework.Controller ReadAsClient(Type controllerType)
		{
			var controller = new Framework.Controller();

			controller.Name = controllerType.Name.Replace("Controller", "");

			controller.Ignored = controllerType.GetCustomAttribute<NoClientAttribute>() != null;
			controller.NamespaceSuffix = controllerType.GetCustomAttribute<NamespaceSuffixAttribute>()?.Suffix;

			if (controllerType.GetCustomAttribute<RouteAttribute>() != null)
			{
				controller.Route = new Route(controllerType.GetCustomAttribute<RouteAttribute>()?.Template);
			}

			if (controller.Route == null)//No Route, invalid controller
			{
				controller.Ignored = true;
				throw new NotSupportedException("Controller must have a route to be valid for generation.");
			}

			var match = RouteVersionRegex.Match(controller.Route.Value);
			if (match.Success)
			{
				var group = match.Groups[1];
				controller.NamespaceVersion = group.Value.ToUpper();
			}

			var responseTypes = controllerType.GetCustomAttributes<ProducesResponseTypeAttribute>().ToList();
			controller.ResponseTypes = responseTypes.Select(x => new Framework.ResponseTypes.ResponseType(x.Type.FullName, (HttpStatusCode)x.StatusCode)).ToList();


			var parameterHeaders = controllerType.GetCustomAttributes<HeaderParameterAttribute>().ToList();
			controller.ParameterHeader = parameterHeaders.Select(x => new Framework.Headers.ParameterHeader(x.Name, x.Type, x.DefaultValue)).ToList();

			var headers = controllerType.GetCustomAttributes<IncludeHeaderAttribute>().ToList();
			controller.ConstantHeader = headers.Select(x => new Framework.Headers.ConstantHeader(x.Name, x.Value)).ToList();


			controller.IsSecured = controllerType.GetCustomAttributes<AuthorizeAttribute>() != null;

			var obsolete = controllerType.GetCustomAttribute<ObsoleteAttribute>();
			controller.Obsolete = obsolete != null;
			controller.ObsoleteMessage = obsolete?.Message;


			var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();

			controller.Endpoints = methods.Select(x => ReadAsEndpoint(controller, x)).ToList();



			return controller;
		}


		private static Endpoint ReadAsEndpoint(Framework.Controller parent, MethodInfo method)
		{
			Endpoint endpoint = new Endpoint(parent);

			endpoint.Name = method.Name;

			endpoint.Ignored = method.GetCustomAttribute<NoClientAttribute>() != null;

			if (method.GetCustomAttribute<RouteAttribute>() != null)
			{
				endpoint.Route = new Route(method.GetCustomAttribute<RouteAttribute>()?.Template);
			}

			var httpMethod = method.GetCustomAttribute<HttpMethodAttribute>();







			return endpoint;
		}

	}
}
