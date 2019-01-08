using System.Collections.Generic;

namespace AspNetCore.Client.Generator.Framework.Navigation
{
	/// <summary>
	/// A node that can provide its children
	/// </summary>
	public interface INavNode
	{
		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		IEnumerable<INavNode> GetChildren();
	}
}
