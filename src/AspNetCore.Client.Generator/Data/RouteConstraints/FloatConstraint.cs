using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class FloatConstraint : RouteConstraint
	{
		public FloatConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			return
$@"			if(!float.TryParse({ParameterName}.ToString(),out float {ParameterName}OUT))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} does not parse into an float."");
			}}
";
		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("float", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
