using AspNetCore.Client.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.Headers
{
	/// <summary>
	/// Handles <see cref="IncludeHeaderAttribute"/>
	/// </summary>
	public class ConstantHeader : Header
	{
		/// <summary>
		/// Whether a header is a constant or not
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public ConstantHeader(string key, string value) : base(key)
		{
			Value = value;
		}
	}
}
