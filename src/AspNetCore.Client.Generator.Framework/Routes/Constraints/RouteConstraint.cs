using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public abstract class RouteConstraint
	{
		private static IEnumerable<Type> _allContraints = typeof(RouteConstraint).GetTypeInfo().Assembly
											.GetTypes()
											.Where(x => typeof(RouteConstraint).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract)
											.ToList();


		public string ParameterName { get; set; }
		public string Constraint { get; set; }


		public RouteConstraint(string name, string constraint)
		{
			ParameterName = name;
			Constraint = constraint;
		}


		protected static Regex GetConstraintValueRegex = new Regex(@".+\((.+)\)");

		public virtual string GetConstraintValue()
		{
			return GetConstraintValueRegex.Match(Constraint).Groups[1].Value;
		}

		public abstract bool IsMatch();



		public static RouteConstraint GetConstraint(string name, string constraint)
		{
			foreach (var type in _allContraints)
			{
				var instance = Activator.CreateInstance(type, new object[] { name, constraint }) as RouteConstraint;
				if (instance.IsMatch())
				{
					return instance;
				}
			}
			return null;
		}
	}
}
