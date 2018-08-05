using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class RangeConstraint : RouteConstraint
	{
		public RangeConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("range(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
