using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	/// <summary>
	/// String must match the regular expression
	/// </summary>
	public class RegexConstraint : RouteConstraint
	{
		/// <summary>
		/// Calls the base constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		public RegexConstraint(string name, string constraint) : base(name, constraint)
		{

		}


		/// <summary>
		/// Gets the value of the constraint
		/// </summary>
		/// <returns></returns>
		public override string GetConstraintValue()
		{
			Regex regexParser = new Regex(@"regex\((.+)\)");

			return regexParser.Match(Constraint).Groups[1].Value;
		}

		/// <summary>
		/// Detects whether or not the ParameterName + Constraint values are valid for this constraint
		/// </summary>
		/// <returns></returns>
		public override bool IsMatch()
		{
			return Constraint?.StartsWith("regex(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
