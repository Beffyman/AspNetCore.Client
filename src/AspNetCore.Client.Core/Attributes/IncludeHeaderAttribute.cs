using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core.Attributes
{
	/// <summary>
	/// Adds a pre-defined header to the clients hitting this endpoint
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class IncludeHeaderAttribute : Attribute
	{
		/// <summary>
		/// Name of the attribute the generator looks for
		/// </summary>
		public const string AttributeName = "IncludeHeader";

		/// <summary>
		/// Name of the header
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Value of the header
		/// </summary>
		public string Value { get; }

		public IncludeHeaderAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}

	}
}
