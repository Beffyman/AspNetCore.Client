using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class MaxLengthConstraint : RouteConstraint
	{
		public MaxLengthConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("maxlength(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
