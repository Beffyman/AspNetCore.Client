namespace AspNetCore.Client.Generator
{
	internal class Settings
	{
		public static string RouteToServiceProjectFolder { get; set; }
		public static string ClientInterfaceName { get; set; }
		public static string RegisterName { get; set; }
		public static bool UseValueTask { get; set; }
		public static bool UseInternalClients { get; set; }
		public static bool ClientRouteConstraints { get; set; }
		public static bool ErrorOnUnhandledCallback { get; set; }
		public static bool MultipleFiles { get; set; }
		public static string ClientNamespace { get; set; }
		public static string HubNamespace { get; set; }

		public static string[] AllowedNamespaces { get; set; }

		public static string[] ExcludedNamespaces { get; set; }

	}
}
