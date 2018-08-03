using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class MinConstraint : RouteConstraint
	{
		public MinConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("min(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
