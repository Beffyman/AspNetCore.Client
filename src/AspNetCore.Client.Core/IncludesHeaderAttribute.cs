using System;

namespace AspNetCore.Client.Core
{
	/// <summary>
	/// Used to place a header inside the generated method's parameters
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class IncludesHeaderAttribute : Attribute
	{
		/// <summary>
		/// Name of the attribute the generator looks for
		/// </summary>
		public const string AttributeName = "IncludesHeader";

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
