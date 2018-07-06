using System;

namespace AspNetCore.Client.Core.Attributes
{
	/// <summary>
	/// Adds a parameter to the client methods that will be placed inside the request header
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class HeaderParameterAttribute : Attribute
	{
		/// <summary>
		/// Name of the attribute the generator looks for
		/// </summary>
		public const string AttributeName = "HeaderParameter";

		/// <summary>
		/// Name of the header
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Type to be used for the header
		/// </summary>
		public string Type { get; }
		/// <summary>
		/// Default value (int i = 0) to be used for the header
		/// </summary>
		public string DefaultValue { get; }

		public HeaderParameterAttribute(string name, string type, string defaultValue)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}

		public HeaderParameterAttribute(string name, string type)
		{
			Name = name;
			Type = type;
			DefaultValue = null;
		}

		public HeaderParameterAttribute(string name, Type type)
		{
			Name = name;
			Type = type.Name;
			DefaultValue = null;
		}

		public HeaderParameterAttribute(string name, Type type, string defaultValue)
		{
			Name = name;
			Type = type.Name;
			DefaultValue = defaultValue;
		}
	}
}
