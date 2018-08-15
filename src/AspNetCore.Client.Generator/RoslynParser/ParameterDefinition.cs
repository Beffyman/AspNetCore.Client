using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AspNetCore.Client.Generator.RoslynParser
{
	public class ParameterDefinition
	{
		public string Name { get; }
		public string Type { get; }
		public string Default { get; }

		public ParameterAttributeOptions Options { get; }


		public ParameterDefinition(ParameterSyntax parameter, string fullRoute)
		{
			Name = parameter.Identifier.ValueText.Trim();
			Type = parameter.Type.ToFullString().Trim();
			Default = parameter.Default?.Value.ToFullString().Trim();

			var attributes = parameter.AttributeLists.SelectMany(x => x.Attributes).ToList();

			Options = new ParameterAttributeOptions
			{
				FromRoute = attributes.Any(x => x.Name.ToFullString().StartsWith(Constants.FromRoute)),
				FromBody = attributes.Any(x => x.Name.ToFullString().StartsWith(Constants.FromBody)) || !(Helpers.IsRoutableType(Helpers.GetEnumerableType(Type)))
			};

			var fromQueryAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.FromQuery));
			if (fromQueryAttribute != null)//Fetch route from RouteAttribute
			{
				Options.FromQuery = true;
				Options.QueryName = fromQueryAttribute.ArgumentList?.Arguments.ToFullString().Replace("\"", "").Split('=')[1].Trim();

				if (string.IsNullOrEmpty(Options.QueryName))
				{
					Options.QueryName = Name;
				}
			}


			var fromRouteAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.FromRoute));
			if (fromRouteAttribute != null)//Fetch route from RouteAttribute
			{
				Options.FromRoute = true;
				Options.RouteName = fromRouteAttribute.ArgumentList?.Arguments.ToFullString().Replace("\"", "").Split('=')[1].Trim();

				if (string.IsNullOrEmpty(Options.RouteName))
				{
					Options.RouteName = Name;
				}
			}

			Options.FromRoute = Options.FromRoute || (Helpers.IsRouteParameter(Name, fullRoute) || (Helpers.IsEnumerable(Type) && Helpers.IsRoutableType(Helpers.GetEnumerableType(Type))));


			if ((Helpers.IsRoutableType(Helpers.GetEnumerableType(Type)))
				&& !Options.FromRoute
				&& !Options.FromBody)
			{
				Options.FromQuery = true;
				Options.QueryName = Name;
			}

			if (Options.FromQuery)
			{
				Options.FromRoute = false;
				Options.RouteName = null;
			}

			if (Options.FromBody)
			{
				Options.FromRoute = false;
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
	}
}
