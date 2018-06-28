using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.Client.Generator.Data.RouteConstraints
{
	public abstract class RouteConstraint
	{
		protected static Regex GetConstraintValueRegex = new Regex(@".+\((.+)\)");


		public string ParameterName { get; }
		public string Constraint { get; }

		public RouteConstraint(string parameterName, string constraint)
		{
			ParameterName = parameterName;
			Constraint = constraint;
		}

		public abstract string GetText();

		public abstract bool IsMatch();


		public static RouteConstraint GetConstraint(string parameterName, string constraint)
		{
			var allContraints = typeof(RouteConstraint).GetTypeInfo().Assembly
								.GetTypes()
								.Where(x => typeof(RouteConstraint).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract)
								.ToList();

			foreach (var type in allContraints)
			{
				var instance = Activator.CreateInstance(type, new object[] { parameterName, constraint }) as RouteConstraint;
				if (instance.IsMatch())
				{
					return instance;
				}
			}

			return null;
		}

	}
}
