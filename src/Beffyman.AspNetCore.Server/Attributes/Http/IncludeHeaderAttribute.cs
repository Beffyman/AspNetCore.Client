﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Server.Attributes.Http
{
	/// <summary>
	/// Adds a pre-defined header to the clients hitting this endpoint
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class IncludeHeaderAttribute : Attribute
	{
		/// <summary>
		/// Name of the header
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Value of the header
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Will prepopulate a header with the name and value provided
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public IncludeHeaderAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}
