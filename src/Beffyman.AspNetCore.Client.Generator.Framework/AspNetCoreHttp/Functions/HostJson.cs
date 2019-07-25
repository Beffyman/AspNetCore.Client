using System;
using System.Collections.Generic;
using System.Text;

namespace Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Functions
{
	public class HostJson
	{
		public Http http { get; set; }
	}

	public class Http
	{
		public string routePrefix { get; set; }
	}

}
