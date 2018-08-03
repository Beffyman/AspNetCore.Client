using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class GuidConstraint : RouteConstraint
	{
		public GuidConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("guid", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
