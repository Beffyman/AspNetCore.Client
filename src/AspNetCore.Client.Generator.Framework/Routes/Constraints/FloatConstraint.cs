using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class FloatConstraint : RouteConstraint
	{
		public FloatConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("float", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
