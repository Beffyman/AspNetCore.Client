using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public class RequiredConstraint : RouteConstraint
	{
		public RequiredConstraint(string parameterName, string constraint)
			: base(parameterName, constraint)
		{ }

		public override string GetText()
		{
			return null;
		}


		public override bool IsMatch()
		{
			return Constraint?.Equals("required", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
