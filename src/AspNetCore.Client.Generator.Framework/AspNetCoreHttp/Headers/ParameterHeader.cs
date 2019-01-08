using System.Collections.Generic;
using AspNetCore.Client.Generator.Framework.Navigation;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Headers
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
		/// Sort order used for the display/writing of the parameter
		/// </summary>
		public int SortOrder => 4;

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

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<INavNode> GetChildren()
		{
			return null;
		}

		/// <summary>
		/// Returns a string that represents the current object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Type} {Key} = {DefaultValue}";
		}
	}
}
