using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCore.Client.Generator.Output
{
	public static class ReflectionClassParser
	{
		public static Controller ReadAsClient(Type controllerType)
		{
			var controller = new Controller();

			controller.Name = controllerType.Name.Replace("Controller", "");

			controller.Ignored = controllerType.GetCustomAttribute<NoClientAttribute>() != null;
			controller.NamespaceSuffix = controllerType.GetCustomAttribute<NamespaceSuffixAttribute>()?.Suffix;














			return controller;
		}


	}
}
