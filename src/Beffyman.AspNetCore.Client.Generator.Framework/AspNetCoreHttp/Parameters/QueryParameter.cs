using System.Collections.Generic;
using Beffyman.AspNetCore.Client.Generator.Framework.Navigation;

namespace Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Parameters
{
	/// <summary>
	/// Parameter for the endpoint parameter that is placed in the uri as a query parameter
	/// </summary>
	public class QueryParameter : IParameter
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
		/// Whether or not this parameter needs to be converted into a query string from its properties
		/// </summary>
		public bool QueryObject { get; }

		/// <summary>
		/// Order in which the parameter is inside the generated file
		/// </summary>
		public int SortOrder => 3;

		/// <summary>
		/// Whether or not the Type is empty
		/// </summary>
		public bool IsConstant => string.IsNullOrEmpty(Type);

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="value"></param>
		public QueryParameter(string value)
		{
			Name = value;
		}

		/// <summary>
		/// Constructs a parameter with the provided info
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="defaultValue"></param>
		/// <param name="queryObject"></param>
		public QueryParameter(string name, string type, string defaultValue, bool queryObject)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
			QueryObject = queryObject;
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
			if (string.IsNullOrEmpty(Type))
			{
				return string.Empty;
			}

			return $"{Type} {Name} = {DefaultValue}";
		}
	}
}
