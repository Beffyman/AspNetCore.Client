using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.CSharp.RouteConstraints
{
	public class IntConstraint : RouteConstraint
	{
		public IntConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{}

		public override string GetText()
		{
			return
$@"			if(!int.TryParse({ParameterName}.ToString(),out int {ParameterName}OUT))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} does not parse into an int."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.Equals("int", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
