using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.Parameters
{
	/// <summary>
	/// Indicates that the object should be placed as a parameter
	/// </summary>
	public interface IParameter
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
	}
}
