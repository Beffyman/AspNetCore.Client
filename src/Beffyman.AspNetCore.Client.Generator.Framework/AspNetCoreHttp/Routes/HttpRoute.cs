using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints;
using Microsoft.AspNetCore.Routing.Template;

namespace Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes
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
		public IEnumerable<RouteConstraint> Constraints { get; set; } = Enumerable.Empty<RouteConstraint>();

		/// <summary>
		/// Version of the api
		/// </summary>
		public ApiVersion Version { get; set; }

		/// <summary>
		/// Creates a route and interprets the contraints from it
		/// </summary>
		/// <param name="value"></param>
		public HttpRoute(string value)
		{
			Value = value;

			Constraints = GetRouteParameters(Value)
				.SelectMany(x => x.Value.Select(y => RouteConstraint.GetConstraint(x.Key, y)))
				.Where(x => x != null)
				.ToList();
		}

		private static IDictionary<string, IEnumerable<string>> GetRouteParameters(string route)
		{
			IDictionary<string, IEnumerable<string>> parameters = new Dictionary<string, IEnumerable<string>>();

			if (route == null)
			{
				return parameters;
			}

			if (!route.EndsWith("/"))
			{
				route += "/";//Need the extra / for the regex regex parse(yes two regex)
			}

			var template = TemplateParser.Parse(route);

			foreach (var para in template.Parameters)
			{
				parameters.Add(para.Name, para.InlineConstraints?.Select(x => x.Constraint)?.ToList() ?? Enumerable.Empty<string>());
			}

			return parameters;
		}

		public IDictionary<string, (string type, string defaultValue)> GetRouteParameters()
		{
			IDictionary<string, (string type, string defaultValue)> parameters = new Dictionary<string, (string type, string defaultValue)>();

			string val = Value;

			if (val == null)
			{
				return parameters;
			}

			if (!val.EndsWith("/"))
			{
				val += "/";//Need the extra / for the regex regex parse(yes two regex)
			}

			var template = TemplateParser.Parse(Value);

			return template.Parameters.ToDictionary(x => x.Name, y => (y?.InlineConstraints.FirstOrDefault()?.Constraint, y.DefaultValue?.ToString()));
		}


		/// <summary>
		/// Merges the routes together and regenerates the constraints
		/// </summary>
		/// <param name="route"></param>
		/// <returns></returns>
		public HttpRoute Merge(HttpRoute route)
		{
			return new HttpRoute($"{this?.Value}/{route?.Value}")
			{
				Version = this?.Version ?? route?.Version
			};
		}

		public bool Contains(string str)
		{
			return Value?.Contains(str) ?? false;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
