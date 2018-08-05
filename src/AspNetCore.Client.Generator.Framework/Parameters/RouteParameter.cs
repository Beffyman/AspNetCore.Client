using System;
using System.Collections.Generic;
using System.Text;
using AspNetCore.Client.Generator.Framework.Navigation;

namespace AspNetCore.Client.Generator.Framework.Parameters
{
	public class RouteParameter : IParameter
	{
		public string Name { get; }

		public string Type { get; }

		public string DefaultValue { get; }
		public int SortOrder => 2;

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


		public override string ToString()
		{
			return $"{Type} {Name} = {DefaultValue}";
		}
	}
}
