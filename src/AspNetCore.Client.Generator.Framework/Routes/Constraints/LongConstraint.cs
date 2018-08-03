using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class LongConstraint : RouteConstraint
	{
		public LongConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("long", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
