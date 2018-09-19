using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Attributes.Http
{
	/// <summary>
	/// Modifies the namespace of the client to have the provided value added onto it
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class NamespaceSuffixAttribute : Attribute
	{
		/// <summary>
		/// Suffix to add to the namespace of the client.  TestWebApp.Clients.SUFFIX
		/// </summary>
		public string Suffix { get; }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="suffix"></param>
		public NamespaceSuffixAttribute(string suffix)
		{
			Suffix = suffix;
		}

	}
}
