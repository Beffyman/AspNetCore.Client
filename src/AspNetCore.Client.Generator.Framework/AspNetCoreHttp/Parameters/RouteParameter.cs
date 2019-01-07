using System;
using System.Collections.Generic;
using System.Text;
using AspNetCore.Client.Generator.Framework.Navigation;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Parameters
{
	/// <summary>
	/// Parameter for the endpoint parameter that is placed in the uri as a route parameter
	/// </summary>
	public class RouteParameter : IParameter
	{
		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type { get; }

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue { get; }

		/// <summary>
		/// Order in which the parameter is inside the generated file
		/// </summary>
		public int SortOrder => 2;

		/// <summary>
		/// Constructs a parameter with the provided info
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="defaultValue"></param>
		public RouteParameter(string name, string type, string defaultValue = null)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}

		/// <summary>
		/// Returns a string that represents the current object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Type} {Name} = {DefaultValue}";
		}
	}
}
