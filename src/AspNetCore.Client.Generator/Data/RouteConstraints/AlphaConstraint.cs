using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class AlphaConstraint : RouteConstraint
	{
		public AlphaConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			return
$@"			if(string.IsNullOrWhiteSpace({ParameterName}) || {ParameterName}.Any(x=>char.IsNumber(x)))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} must only contain characters that are not numbers."");
			}}
";
		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("alpha",StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
