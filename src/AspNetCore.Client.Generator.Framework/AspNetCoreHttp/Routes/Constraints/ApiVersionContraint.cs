using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints
{
	/// <summary>
	/// RouteConstraint for apiVersioning
	/// </summary>
	public class ApiVersionContraint : RouteConstraint
	{
		/// <summary>
		/// Calls the base constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		public ApiVersionContraint(string name, string constraint) : base(name, constraint)
		{

		}

		/// <summary>
		/// Detects whether or not the ParameterName + Constraint values are valid for this constraint
		/// </summary>
		/// <returns></returns>
		public override bool IsMatch()
		{
			return Constraint?.Equals("apiVersion", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
