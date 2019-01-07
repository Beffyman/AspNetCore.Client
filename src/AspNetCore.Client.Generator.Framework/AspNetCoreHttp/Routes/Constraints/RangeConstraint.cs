using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints
{
	/// <summary>
	/// Integer value must be at least x but no more than y
	/// </summary>
	public class RangeConstraint : RouteConstraint
	{
		/// <summary>
		/// Calls the base constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		public RangeConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		/// <summary>
		/// Detects whether or not the ParameterName + Constraint values are valid for this constraint
		/// </summary>
		/// <returns></returns>
		public override bool IsMatch()
		{
			return Constraint?.StartsWith("range(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
