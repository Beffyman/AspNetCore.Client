using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.CSharp.RouteConstraints
{
	public class BoolConstraint : RouteConstraint
	{
		public BoolConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{}

		public override string GetText()
		{
			return
$@"			if(!bool.TryParse({ParameterName}.ToString(),out bool {ParameterName}OUT))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} does not parse into an bool."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.Equals("bool", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
