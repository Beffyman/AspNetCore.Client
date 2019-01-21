using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints
{
	/// <summary>
	/// Base route constraint implementation
	/// </summary>
	public abstract class RouteConstraint
	{
		private static IEnumerable<Type> _allContraints = typeof(RouteConstraint).GetTypeInfo().Assembly
											.GetTypes()
											.Where(x => typeof(RouteConstraint).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract)
											.ToList();

		/// <summary>
		/// Name of the parameter {0:1}, value 0
		/// </summary>
		public string ParameterName { get; set; }

		/// <summary>
		/// Value of the constraint {0:1}, value 1
		/// </summary>
		public string Constraint { get; set; }

		/// <summary>
		/// Assigns the base properties
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		public RouteConstraint(string name, string constraint)
		{
			ParameterName = name;
			Constraint = constraint;
		}


		private static Regex GetConstraintValueRegex = new Regex(@".+\((.+)\)");

		/// <summary>
		/// Gets the value of the constraint
		/// </summary>
		/// <returns></returns>
		public virtual string GetConstraintValue()
		{
			return GetConstraintValueRegex.Match(Constraint).Groups[1].Value;
		}

		/// <summary>
		/// Detects whether or not the ParameterName + Constraint values are valid for this constraint
		/// </summary>
		/// <returns></returns>
		public abstract bool IsMatch();


		/// <summary>
		/// Gets the constraint associated with the value of the constraint parameter
		/// </summary>
		/// <param name="name"></param>
		/// <param name="constraint"></param>
		/// <returns></returns>
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
