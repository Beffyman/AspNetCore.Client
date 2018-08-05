using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class DecimalConstraint : RouteConstraint
	{
		public DecimalConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("decimal", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
