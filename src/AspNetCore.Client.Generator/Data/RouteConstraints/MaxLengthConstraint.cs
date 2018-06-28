using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class MaxLengthConstraint : RouteConstraint
	{
		public MaxLengthConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			string value = GetConstraintValueRegex.Match(Constraint).Groups[1].Value;

			return
$@"			if({ParameterName}.Length >= {value})
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} has a length greater than {value}."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.StartsWith("maxlength(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
