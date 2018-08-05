using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Headers
{
	/// <summary>
	/// Contains basic info about a header
	/// </summary>
	public abstract class Header : INavNode, IRequestModifier
	{
		/// <summary>
		/// Key of the header
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Requires key for a header
		/// </summary>
		/// <param name="key"></param>
		public Header(string key)
		{
			Key = key;
		}

		public abstract IEnumerable<INavNode> GetChildren();
	}
}
