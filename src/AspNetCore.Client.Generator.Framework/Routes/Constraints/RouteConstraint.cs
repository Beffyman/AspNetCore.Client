using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Routes.Constraints
{
	public abstract class RouteConstraint
	{
		private static IEnumerable<Type> _allContraints = typeof(RouteConstraint).GetTypeInfo().Assembly
											.GetTypes()
											.Where(x => typeof(RouteConstraint).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract)
											.ToList();


		public string ParamterName { get; set; }
		public string Constraint { get; set; }


		public RouteConstraint(string name, string constraint)
		{
			ParamterName = name;
			Constraint = constraint;
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
