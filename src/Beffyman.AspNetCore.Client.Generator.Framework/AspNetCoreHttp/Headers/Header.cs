using System.Collections.Generic;
using Beffyman.AspNetCore.Client.Generator.Framework.Navigation;
using Beffyman.AspNetCore.Client.Generator.Framework.RequestModifiers;

namespace Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Headers
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

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<INavNode> GetChildren();
	}
}
