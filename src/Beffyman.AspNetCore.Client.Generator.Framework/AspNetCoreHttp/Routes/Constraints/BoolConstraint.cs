using System;

namespace Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints
{
	/// <summary>
	/// Matches true or false (case-insensitive)
	/// </summary>
	public class BoolConstraint : RouteConstraint
	{
		/// <summary>
		/// Calls the base constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		public BoolConstraint(string name, string constraint) : base(name, constraint)
		{

		}


		/// <summary>
		/// Detects whether or not the ParameterName + Constraint values are valid for this constraint
		/// </summary>
		/// <returns></returns>
		public override bool IsMatch()
		{
			return Constraint?.Equals("bool", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
