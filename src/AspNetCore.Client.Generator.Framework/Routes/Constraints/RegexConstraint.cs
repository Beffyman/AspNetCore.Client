using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public class RegexConstraint : RouteConstraint
	{
		public RegexConstraint(string name, string constraint) : base(name, constraint)
		{

		}


		public override string GetConstraintValue()
		{
			Regex regexParser = new Regex(@"regex\((.+)\)");

			return regexParser.Match(Constraint).Groups[1].Value;
		}

		public override bool IsMatch()
		{
			return Constraint?.StartsWith("regex(", StringComparison.CurrentCultureIgnoreCase) ?? false;
		}
	}
}
