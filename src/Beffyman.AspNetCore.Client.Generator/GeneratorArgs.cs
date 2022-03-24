using System.Collections.Generic;

namespace Beffyman.AspNetCore.Client.Generator
{
	public class GeneratorArgs
	{
		public GeneratorArgs() { }

		public GeneratorArgs(IDictionary<string, string> properties)
		{
			foreach (var prop in typeof(GeneratorArgs).GetProperties())
			{
				if (properties.ContainsKey(prop.Name))
				{
					prop.SetValue(this, properties[prop.Name]);
				}
			}
		}

		public string CurrentDirectory { get; set; }
		public string RouteToServiceProjectFolder { get; set; }
		public string ClientInterfaceName { get; set; }
		public string RegisterName { get; set; }
		public string UseValueTask { get; set; }
		public string UseInternalClients { get; set; }
		public string ClientRouteConstraints { get; set; }
		public string ErrorOnUnhandledCallback { get; set; }
		public string MultipleFiles { get; set; }
		public string GenerateStaticRoutes { get; set; }
		public string GenerateClientRepository { get; set; }
		public string GenerateLazyClientRepository { get; set; }
		public string RoutesNamespace { get; set; }
		public string ClientNamespace { get; set; }
		public string HubNamespace { get; set; }
		public string AllowedNamespaces { get; set; }
		public string ExcludedNamespaces { get; set; }
	}
}
