using System;
using System.Collections.Generic;
using System.Linq;
using Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreHttp
{
	public class ParameterDefinition
	{
		public string Name { get; }
		public string Type { get; }
		public string Default { get; }

		public ParameterAttributeOptions Options { get; }


		public ParameterDefinition(ParameterSyntax parameter, HttpRoute fullRoute)
		{
			Name = parameter.Identifier.ValueText.Trim();
			Type = parameter.Type.ToFullString().Trim();
			Default = parameter.Default?.Value.ToFullString().Trim();

			var attributes = parameter.AttributeLists.SelectMany(x => x.Attributes).ToList();

			Options = new ParameterAttributeOptions
			{
				FromQuery = attributes.Any(x => x.Name.ToFullString().MatchesAttribute(nameof(FromQueryAttribute))),
				FromRoute = attributes.Any(x => x.Name.ToFullString().MatchesAttribute(nameof(FromRouteAttribute))),
				FromBody = attributes.Any(x => x.Name.ToFullString().MatchesAttribute(nameof(FromBodyAttribute))) || !(Helpers.IsRoutableType(Helpers.GetEnumerableType(Type)))
			};

			var fromQueryAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(FromQueryAttribute)));
			if (fromQueryAttribute != null)//Fetch route from RouteAttribute
			{
				Options.FromQuery = true;
				Options.QueryName = fromQueryAttribute.ArgumentList?.Arguments.ToFullString().TrimQuotes().Split('=')[1].TrimQuotes();

				if (string.IsNullOrEmpty(Options.QueryName))
				{
					Options.QueryName = Name;
				}
			}


			var fromRouteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(nameof(FromRouteAttribute)));
			if (fromRouteAttribute != null)//Fetch route from RouteAttribute
			{
				Options.FromRoute = true;
				Options.RouteName = fromRouteAttribute.ArgumentList?.Arguments.ToFullString().TrimQuotes().Split('=')[1].TrimQuotes();

				if (string.IsNullOrEmpty(Options.RouteName))
				{
					Options.RouteName = Name;
				}
			}

			Options.FromRoute = Options.FromRoute || (Helpers.IsRouteParameter(Name, fullRoute) || (Helpers.IsEnumerable(Type) && Helpers.IsRoutableType(Helpers.GetEnumerableType(Type))));


			//Is it a query string parameter type, but isn't labeled as such?
			if ((Helpers.IsRoutableType(Helpers.GetEnumerableType(Type)))
				&& !Options.FromRoute
				&& !Options.FromBody)
			{
				Options.FromQuery = true;
				Options.QueryName = Name;
			}

			//if it is a query param, it can't be in the route
			if (Options.FromQuery)
			{
				Options.FromRoute = false;
				Options.RouteName = null;

				//A query object also can't be in the body
				if (Options.FromBody)
				{
					Options.QueryObject = true;
					Options.FromBody = false;
				}
			}

			//If it is a body param, it also can't be in the route
			if (Options.FromBody)
			{
				Options.FromRoute = false;
			}

			//Get Default if it exists inside the route constraint
			if (Options.FromRoute)
			{
				var defaultRouteConstraint = Helpers.GetDefaultRouteConstraint(Name, fullRoute);
				if (!string.IsNullOrEmpty(defaultRouteConstraint))
				{
					Default = defaultRouteConstraint;
				}
			}

			var types = new List<bool>
			{
				Options.FromBody,
				Options.FromRoute,
				Options.FromQuery
			};

			if (types.Count(x => x == true) > 1)
			{
				throw new Exception();
			}

		}



		/// <summary>
		/// Name of the parameter inside the route when replacement happens
		/// </summary>
		public string RouteName
		{
			get
			{
				if (Options.FromRoute)
				{
					return Options.RouteName ?? Name;
				}
				else
				{
					return Name;
				}
			}
		}

		public override string ToString()
		{
			if (Default != null)
			{
				return $"{Type} {Name} = {Default}";
			}
			else
			{
				return $"{Type} {Name}";
			}
		}

	}


	public class ParameterAttributeOptions
	{
		public bool FromRoute { get; set; }
		public string RouteName { get; set; }

		public bool FromBody { get; set; }

		public bool FromQuery { get; set; }
		public string QueryName { get; set; }
		public bool QueryObject { get; set; }
	}
}
