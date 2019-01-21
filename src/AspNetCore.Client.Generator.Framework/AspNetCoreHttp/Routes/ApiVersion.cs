using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes
{
	/// <summary>
	/// Representation of an API version
	/// </summary>
	public class ApiVersion
	{
		/// <summary>
		/// Version of the api
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Was the version inside the route?
		/// </summary>
		public bool Query { get; set; }

		public ApiVersion(string version, bool query)
		{
			Version = version;
			Query = query;
		}

		public override string ToString()
		{
			return Version;
		}
	}
}
