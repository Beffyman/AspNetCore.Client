using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class LengthConstraint : RouteConstraint
	{
		public LengthConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			string value = GetConstraintValueRegex.Match(Constraint).Groups[1].Value;
			if (value.Contains(','))
			{
				var split = value.Split(',');
				string minL = split[0];
				string maxL = split[1];

				return
$@"			if({ParameterName}.Length <= {minL} || {ParameterName}.Length >= {maxL})
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} has a length that is not between {minL} and {maxL}."");
			}}
";
			}
			else
			{
				return
$@"			if({ParameterName}.Length == {value})
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} has a length that is not {value}."");
			}}
";
			}



		}


		public override bool IsMatch()
		{
			return Constraint?.StartsWith("length(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
