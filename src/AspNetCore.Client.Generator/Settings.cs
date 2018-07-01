using AspNetCore.Client.Generator.Data;
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
		public static bool UseValueTask { get; set; }
		public static string ClientNamespace { get; set; }
		/// <summary>
		/// Unsure if this will be needed eventually, if I implement a services config for the serializer
		/// </summary>
		public static bool BlazorClients { get; set; }

		public static string[] AllowedNamespaces { get; set; }

		public static string[] ExcludedNamespaces { get; set; }

	}
}
