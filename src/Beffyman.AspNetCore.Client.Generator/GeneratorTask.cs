using System.Linq;

namespace Beffyman.AspNetCore.Client.Generator
{
	public class GeneratorTask : Microsoft.Build.Utilities.Task
	{
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

		public override bool Execute()
		{
			var properties = typeof(GeneratorTask)
				.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Where(x => x.PropertyType == typeof(string))
				.ToDictionary(x => x.Name, y => (string)y.GetValue(this));

			return ClientGenerator.Generate(new GeneratorArgs(properties), Log);
		}
	}
}
