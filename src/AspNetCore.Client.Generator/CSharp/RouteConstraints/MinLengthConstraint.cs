using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.CSharp.RouteConstraints
{
	public class MinLengthConstraint : RouteConstraint
	{
		public MinLengthConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			string value = GetConstraintValueRegex.Match(Constraint).Groups[1].Value;

			return
$@"			if({ParameterName}.Length <= {value})
			{{
				throw new InvalidRouteException(""Parameter {ParameterName} has a length less than {value}."");
			}}
";
		}


		public override bool IsMatch()
		{
			return Constraint?.StartsWith("minlength(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
