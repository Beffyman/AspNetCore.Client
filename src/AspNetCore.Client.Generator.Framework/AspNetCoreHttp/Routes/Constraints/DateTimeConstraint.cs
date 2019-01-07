using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints
{
	/// <summary>
	/// Matches a valid DateTime value (in the invariant culture - see warning)
	/// </summary>
	public class DateTimeConstraint : RouteConstraint
	{
		/// <summary>
		/// Calls the base constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		public DateTimeConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		/// <summary>
		/// Detects whether or not the ParameterName + Constraint values are valid for this constraint
		/// </summary>
		/// <returns></returns>
		public override bool IsMatch()
		{
			return Constraint?.Equals("datetime", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
