using System.Collections.Generic;
using System.Threading;
using Beffyman.AspNetCore.Client.Generator.Framework.Navigation;

namespace Beffyman.AspNetCore.Client.Generator.Framework.RequestModifiers
{
	/// <summary>
	/// Parameter for the cancellation token for the request
	/// </summary>
	public class CancellationTokenModifier : IRequestModifier, IParameter
	{

		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name => "cancellationToken";

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type => $"{nameof(CancellationToken)}";

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue => "default";

		/// <summary>
		/// Order in which the parameter is inside the generated file
		/// </summary>
		public int SortOrder => 10;

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Type} {Name} = {DefaultValue}";
		}
	}
}
