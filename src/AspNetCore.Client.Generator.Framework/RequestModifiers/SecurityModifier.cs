using AspNetCore.Client.Authorization;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.RequestModifiers
{
	public class SecurityModifier : IRequestModifier, IParameter
	{

		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name => "auth";

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type => $"{nameof(SecurityHeader)}?";

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue => "null";

		public SecurityModifier()
		{

		}


		public IRequestModifier ExtractModifier()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
