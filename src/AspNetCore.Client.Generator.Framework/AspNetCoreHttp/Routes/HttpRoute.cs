using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes
{
	/// <summary>
	/// Route details
	/// </summary>
	public class HttpRoute
	{
		/// <summary>
		/// Raw route
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Constraints interpreted from the {:xyz}s in the route
		/// </summary>
		public IEnumerable<RouteConstraint> Constraints { get; set; } = new List<RouteConstraint>();

		/// <summary>
		/// Creates a route and interprets the contraints from it
		/// </summary>
		/// <param name="value"></param>
		public HttpRoute(string value)
		{
			Value = value;

			Constraints = GetRouteParameters(Value)
				.Select(x => RouteConstraint.GetConstraint(x.Key, x.Value))
				.Where(x => x != null)
				.ToList();
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
		public HttpRoute Merge(HttpRoute route)
		{
			return new HttpRoute($"{this?.Value}/{route?.Value}");
		}
	}
}
