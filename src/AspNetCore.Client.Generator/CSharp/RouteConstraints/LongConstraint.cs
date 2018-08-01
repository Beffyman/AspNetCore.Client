using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.CSharp.RouteConstraints
{
	public class LongConstraint : RouteConstraint
	{
		public LongConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{}

		public override string GetText()
		{
			return
$@"		if(!long.TryParse({ParameterName}.ToString(),out long {ParameterName}OUT))
		{{
			throw new InvalidRouteException(""Parameter {ParameterName} does not parse into an long."");
		}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.Equals("long", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
