using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class DateTimeConstraint : RouteConstraint
	{
		public DateTimeConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{}

		public override string GetText()
		{
			return
$@"			if(!DateTime.TryParse({ParameterName}.ToString(),out DateTime {ParameterName}OUT))
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} does not parse into an DateTime."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.Equals("datetime", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
