using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class GuidConstraint : RouteConstraint
	{
		public GuidConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			return
$@"			if(!Guid.TryParse({ParameterName}.ToString(),out Guid {ParameterName}OUT))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} does not parse into an Guid."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.Equals("guid", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
