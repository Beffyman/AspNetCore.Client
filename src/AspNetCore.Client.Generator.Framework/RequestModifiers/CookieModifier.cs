using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.Parameters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.RequestModifiers
{
	public class CookieModifier : IRequestModifier, IParameter
	{

		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name => "cookies";

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type => $"{nameof(IEnumerable)}<{nameof(Cookie)}>";

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue => "null";


		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}