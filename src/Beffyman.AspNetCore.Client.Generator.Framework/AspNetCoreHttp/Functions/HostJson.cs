using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Functions
{
	/// <summary>
	/// Root Json object for the host.json
	/// </summary>
	public class HostJson
	{
		/// <summary>
		/// Http section
		/// </summary>
		public Http http { get; set; }
	}

	/// <summary>
	/// Http section
	/// </summary>
	public class Http
	{
		/// <summary>
		/// Prefix for http requests
		/// </summary>
		public string routePrefix { get; set; }
	}

}
