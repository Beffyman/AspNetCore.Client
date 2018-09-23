using AspNetCore.Client.Generator.Framework.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework
{
	/// <summary>
	/// Indicates that the object should be placed as a parameter
	/// </summary>
	public interface IParameter : INavNode
	{
		/// <summary>
		/// Display name of the parameter
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Type of the parameter
		/// </summary>
		string Type { get; }

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		string DefaultValue { get; }

		/*
		 * Parameter Order
		 * 1) Body Parameters
		 * 2) Route Parameters
		 * 3) Query Parameters
		 * 4) Parameter Headers
		 * 5) Auth
		 * 6) Response Types
		 * 7) Headers
		 * 8) Cookies
		 * 9) Timeout
		 * 10) CancellationToken
		 */



		/// <summary>
		/// Order in which the parameter is inside the generated file
		/// </summary>
		int SortOrder { get; }
	}
}
