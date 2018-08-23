using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Client.Http;
using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	/// <summary>
	/// Injection for <see cref="IHttpRequestModifier"/>
	/// </summary>
	public class RequestModifierDependency : IDependency, IRequestModifier
	{
		/// <summary>
		/// Injection Field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyFieldType(string clientName)
		{
			return nameof(IHttpRequestModifier);
		}

		/// <summary>
		/// Injection Parameter
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyParameterType(string clientName)
		{
			return $"Func<{clientName},{nameof(IHttpRequestModifier)}>";
		}

		/// <summary>
		/// Name of the parameter/field
		/// </summary>
		/// <param name="clientName"></param>
		/// <returns></returns>
		public string GetDependencyName(string clientName)
		{
			return $"Modifier";
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

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
