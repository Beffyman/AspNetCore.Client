using AspNetCore.Client.Generator.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.RequestModifiers
{
	public class HeadersModifier : IRequestModifier, IParameter
	{

		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name => "headers";

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type => $"IDictionary<{nameof(String)},{nameof(Object)}>";

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue => "null";

		public HeadersModifier()
		{

		}



		public IRequestModifier ExtractModifier()
		{
			throw new NotImplementedException();
		}
	}
}
