using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	/// <summary>
	/// Dependency that handles the generated client wrapper
	/// </summary>
	public class ClientDependency : IDependency
	{
		private string _type { get; }


		/// <summary>
		/// Injection Field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyFieldType(string clientName)
		{
			return GetDependencyParameterType(clientName);
		}

		/// <summary>
		/// Injection Parameter
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyParameterType(string clientName)
		{
			return _type;
		}

		/// <summary>
		/// Name of the parameter/field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyName(string clientName)
		{
			return $"Client";
		}

		/// <summary>
		/// Whether or not to use GetAssignmentOverride for the constructor assignment
		/// </summary>
		public bool HasAssignmentOverride => false;

		/// <summary>
		/// Overwritten constructor assignment
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string GetAssignmentOverride(string value)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// Builds a client dependency of the type provided
		/// </summary>
		/// <param name="type"></param>
		public ClientDependency(string type)
		{
			_type = type;
		}
	}
}
