using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class MaxConstraint : RouteConstraint
	{
		public MaxConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			string value = GetConstraintValueRegex.Match(Constraint).Groups[1].Value;

			return
$@"			if({ParameterName} >= {value})
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} has a value more than {value}."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.StartsWith("max(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
