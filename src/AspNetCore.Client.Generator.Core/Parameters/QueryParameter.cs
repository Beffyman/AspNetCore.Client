using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.Parameters
{
	public class QueryParameter : IParameter
	{
		public string Name { get; }

		public string Type { get; }

		public string DefaultValue { get; }

		public QueryParameter(string name, string type, string defaultValue = null)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}
	}
}
