using System;

namespace AspNetCore.Client.Core
{
	/// <summary>
	/// Used to place a header inside the generated method's parameters
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class IncludesHeaderAttribute : Attribute
	{
		public const string AttributeName = "IncludesHeader";

		public string Name { get; }
		public string Type { get; }
		public string DefaultValue { get; }

		public IncludesHeaderAttribute(string name, string type, string defaultValue)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}

		public IncludesHeaderAttribute(string name, string type)
		{
			Name = name;
			Type = type;
			DefaultValue = null;
		}

	}
}
