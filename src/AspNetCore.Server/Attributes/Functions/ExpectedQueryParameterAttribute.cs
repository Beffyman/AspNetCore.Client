using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Server.Attributes.Functions
{
	/// <summary>
	/// Attribute to signify the parameters the function expects for the given HttpType in the query string
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ExpectedQueryParameterAttribute : Attribute
	{
		/// <summary>
		/// What HttpMethod is the parameter mapped to?
		/// </summary>
		public string HttpMethod { get; }

		/// <summary>
		/// Name of the parameter in the query string
		/// </summary>
		public string QueryName { get; }

		/// <summary>
		/// The type of the expected parameter
		/// </summary>
		public Type ExpectedType { get; }

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="method"></param>
		/// <param name="expectedType"></param>
		public ExpectedQueryParameterAttribute(string method, string queryName, Type expectedType)
		{
			HttpMethod = method;
			QueryName = queryName;
			ExpectedType = expectedType;
		}
	}
}
