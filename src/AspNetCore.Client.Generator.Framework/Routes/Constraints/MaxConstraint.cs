using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class MaxConstraint : RouteConstraint
	{
		public MaxConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("max(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
