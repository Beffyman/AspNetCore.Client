using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class RegexConstraint : RouteConstraint
	{
		public RegexConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			Regex regexParser = new Regex(@"regex\((.+)\)");

			var value = regexParser.Match(Constraint).Groups[1].Value;

			return
$@"			if(!(new Regex(@""{value}"").IsMatch({ParameterName})))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} does not follow the regex \""{value}\""."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.StartsWith("regex(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
