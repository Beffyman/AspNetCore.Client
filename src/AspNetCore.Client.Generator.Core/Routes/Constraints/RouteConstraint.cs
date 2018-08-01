using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.Routes.Constraints
{
	public abstract class RouteConstraint
	{
		public string ParamterName { get; set; }
		public string Constraint { get; set; }


		public RouteConstraint(string name, string constraint)
		{
			ParamterName = name;
			Constraint = constraint;
		}

		public static RouteConstraint GetConstraint(string name, string constraint)
		{

		}
	}
}
