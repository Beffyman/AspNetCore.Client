using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.CSharp.RouteConstraints
{
	public class DecimalConstraint : RouteConstraint
	{
		public DecimalConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{}

		public override string GetText()
		{
			return
$@"			if(!decimal.TryParse({ParameterName}.ToString(),out decimal {ParameterName}OUT))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} does not parse into an decimal."");
			}}
";
		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("decimal", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
