using System;

namespace AspNetCore.Server.Attributes.Http
{
	/// <summary>
	/// Adds a parameter to the client methods that will be placed inside the request header
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class HeaderParameterAttribute : Attribute
	{
		/// <summary>
		/// Name of the header parameter
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Type to be used for the header parameter
		/// </summary>
		public string Type { get; }

		/// <summary>
		/// Default value (int i = *0*) to be used for the header parameter
		/// </summary>
		public string DefaultValue { get; }

		/// <summary>
		/// Will generate a optional parameter that will be: "<paramref name="type"/> <paramref name="name"/> = <paramref name="defaultValue"/>"
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="defaultValue"></param>
		public HeaderParameterAttribute(string name, string type, string defaultValue)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Will generate a required parameter that will be: "<paramref name="type"/> <paramref name="name"/>"
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public HeaderParameterAttribute(string name, string type)
		{
			Name = name;
			Type = type;
			DefaultValue = null;
		}

		/// <summary>
		/// Will generate a required parameter that will be: "<paramref name="type"/> <paramref name="name"/>"
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public HeaderParameterAttribute(string name, Type type)
		{
			Name = name;
			Type = type.Name;
			DefaultValue = null;
		}

		/// <summary>
		/// Will generate a optional parameter that will be: "<paramref name="type"/> <paramref name="name"/> = <paramref name="defaultValue"/>"
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="defaultValue"></param>
		public HeaderParameterAttribute(string name, Type type, string defaultValue)
		{
			Name = name;
			Type = type.Name;
			DefaultValue = defaultValue;
		}
	}
}
