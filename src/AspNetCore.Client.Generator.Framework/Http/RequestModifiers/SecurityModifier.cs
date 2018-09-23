using AspNetCore.Client.Authorization;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.Http.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using AspNetCore.Client.Generator.Framework.RequestModifiers;

namespace AspNetCore.Client.Generator.Framework.Http.RequestModifiers
{
	/// <summary>
	/// Parameter for the security information
	/// </summary>
	public class SecurityModifier : IRequestModifier, IParameter
	{

		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name => "auth";

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type => $"{nameof(SecurityHeader)}";

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue => "null";

		/// <summary>
		/// Order in which the parameter is inside the generated file
		/// </summary>
		public int SortOrder => 5;

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
