using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class BoolConstraint : RouteConstraint
	{
		public BoolConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("bool", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
