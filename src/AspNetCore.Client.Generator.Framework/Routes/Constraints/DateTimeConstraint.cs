using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class DateTimeConstraint : RouteConstraint
	{
		public DateTimeConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		public override bool IsMatch()
		{
			return Constraint?.Equals("datetime", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
