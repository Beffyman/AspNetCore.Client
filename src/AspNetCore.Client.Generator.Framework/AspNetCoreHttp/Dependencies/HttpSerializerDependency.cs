using AspNetCore.Client.Http;
using AspNetCore.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Dependencies
{
	/// <summary>
	/// Injection for <see cref="IHttpSerializer"/>
	/// </summary>
	public class HttpSerializerDependency : IDependency
	{
		/// <summary>
		/// Injection Field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyFieldType(string clientName)
		{
			return nameof(IHttpSerializer);
		}

		/// <summary>
		/// Injection Parameter
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyParameterType(string clientName)
		{
			return $"Func<{clientName},{nameof(IHttpSerializer)}>";
		}

		/// <summary>
		/// Name of the parameter/field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyName(string clientName)
		{
			return $"Serializer";
		}

		/// <summary>
		/// Whether or not to use GetAssignmentOverride for the constructor assignment
		/// </summary>
		public bool HasAssignmentOverride => true;

		/// <summary>
		/// Overwritten constructor assignment
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string GetAssignmentOverride(string value)
		{
			return $"{value}(this)";
		}
	}
}
