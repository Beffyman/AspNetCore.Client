using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class IntConstraint : RouteConstraint
	{
		public IntConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("int", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
