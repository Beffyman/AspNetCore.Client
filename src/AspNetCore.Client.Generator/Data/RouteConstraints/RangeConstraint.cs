using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class RangeConstraint : RouteConstraint
	{
		public RangeConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			string value = GetConstraintValueRegex.Match(Constraint).Groups[1].Value;

			var split = value.Split(',');
			string minL = split[0];
			string maxL = split[1];

			return
$@"			if({ParameterName} <= {minL} || {ParameterName} >= {maxL})
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} has a value that is not between {minL} and {maxL}."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.StartsWith("range(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
