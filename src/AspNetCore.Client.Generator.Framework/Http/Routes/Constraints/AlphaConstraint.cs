using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Http.Routes.Constraints
{
	/// <summary>
	/// String must consist of one or more alphabetical characters (a-z, case-insensitive)
	/// </summary>
	public class AlphaConstraint : RouteConstraint
	{
		/// <summary>
		/// Calls the base constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		public AlphaConstraint(string name, string constraint) : base(name, constraint)
		{

		}

		/// <summary>
		/// Detects whether or not the ParameterName + Constraint values are valid for this constraint
		/// </summary>
		/// <returns></returns>
		public override bool IsMatch()
		{
			return Constraint?.Equals("alpha", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
