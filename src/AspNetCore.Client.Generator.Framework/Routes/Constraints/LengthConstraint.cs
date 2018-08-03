using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class LengthConstraint : RouteConstraint
	{
		public LengthConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("length(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
