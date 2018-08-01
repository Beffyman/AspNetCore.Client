using System;
using System.Collections.Generic;
using System.Text;
using AspNetCore.Client.Generator.Core.Navigation;

namespace AspNetCore.Client.Generator.Core.Parameters
{
	public class RouteParameter : IParameter
	{
		public string Name { get; }

		public string Type { get; }

		public string DefaultValue { get; }

		public RouteParameter(string name, string type, string defaultValue = null)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}

		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
