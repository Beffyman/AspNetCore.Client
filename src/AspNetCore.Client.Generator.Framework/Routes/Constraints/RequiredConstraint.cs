using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class RequiredConstraint : RouteConstraint
	{
		public RequiredConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("required", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
