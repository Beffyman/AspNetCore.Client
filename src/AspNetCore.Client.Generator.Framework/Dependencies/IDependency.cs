using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	/// <summary>
	/// Indicates that the class can be injected into the client
	/// </summary>
	public interface IDependency
	{
		/// <summary>
		/// Injection Field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		string GetDependencyFieldType(string clientName);
		/// <summary>
		/// Injection Parameter
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		string GetDependencyParameterType(string clientName);
		/// <summary>
		/// Name of the parameter/field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		string GetDependencyName(string clientName);
		/// <summary>
		/// Whether or not to use GetAssignmentOverride for the constructor assignment
		/// </summary>
		bool HasAssignmentOverride { get; }
		/// <summary>
		/// Overwritten constructor assignment
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		string GetAssignmentOverride(string value);
	}
}
