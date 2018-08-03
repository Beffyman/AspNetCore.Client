using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class MinLengthConstraint : RouteConstraint
	{
		public MinLengthConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("minlength(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
