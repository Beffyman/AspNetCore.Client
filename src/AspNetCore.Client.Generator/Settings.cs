using AspNetCore.Client.Generator.CSharp.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AspNetCore.Client.Generator
{
	internal class Settings
	{
		public static string RouteToServiceProjectFolder { get; set; }
		public static string ClientInterfaceName { get; set; }
		public static string RegisterName { get; set; }
		public static bool UseValueTask { get; set; }
		public static bool UseInternalClients { get; set; }
		public static string ClientNamespace { get; set; }
		public static string HubNamespace { get; set; }

		public static string[] AllowedNamespaces { get; set; }

		public static string[] ExcludedNamespaces { get; set; }

	}
}
