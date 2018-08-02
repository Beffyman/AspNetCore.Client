using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Headers
{
	/// <summary>
	/// Handles <see cref="HeaderParameterAttribute"/>
	/// </summary>
	public class ParameterHeader : Header, IParameter
	{
		/// <summary>
		/// A Header's parameter name is the key
		/// </summary>
		public string Name => Key;

		/// <summary>
		/// Type of the header parameter
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// What the default value be as a parameter
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		/// Default parameter
		/// </summary>
		/// <param name="key"></param>
		/// <param name="type"></param>
		/// <param name="defaultValue"></param>
		public ParameterHeader(string key, string type, string defaultValue = null) : base(key)
		{
			Type = type;
			DefaultValue = defaultValue;
		}

		public override IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
