using AspNetCore.Client.Generator.Core.Routes.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Core.Routes
{
	public class Route
	{
		public string Value { get; set; }

		public IEnumerable<RouteConstraint> Constraints { get; set; } = new List<RouteConstraint>();

		public Route(string value)
		{
			Value = value;

			Constraints = GetRouteParameters(Value).Select(x => RouteConstraint.GetConstraint(x.Key, x.Value)).ToList();
		}


		const string RouteParseRegex = @"{((.+?(?=:)):(.+?(?=}\/))|(.+?(?=}\/)))}";

		private static IDictionary<string, string> GetRouteParameters(string route)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();

			if (!route.EndsWith("/"))
			{
				route += "/";//Need the extra / for the regex regex parse(yes two regex)
			}

			if (route == null)
			{
				return parameters;
			}
			var patterns = Regex.Matches(route, RouteParseRegex);

			foreach (var group in patterns)
			{
				Match match = group as Match;
				string filtered = match.Value.Replace("{", "").Replace("}", "");
				string[] split = filtered.Split(new char[] { ':' });
				if (split.Length == 1)
				{
					string variable = split[0];
					string parsedType = null;
					parameters.Add(variable, parsedType);
				}
				else
				{
					string variable = split[0];

					string type = split[1];
					parameters.Add(variable, type);
				}

			}
			return parameters;
		}


		/// <summary>
		/// Merges the routes together and regenerates the constraints
		/// </summary>
		/// <param name="route"></param>
		/// <returns></returns>
		public Route Merge(Route route)
		{
			return new Route($"{this.Value}/{route?.Value}");
		}
	}
}
