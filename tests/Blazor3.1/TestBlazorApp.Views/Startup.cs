using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using TestBlazorApp.Clients;
using Beffyman.AspNetCore.Client;

namespace TestBlazorApp.Views
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTestBlazorClients(config =>
			{
				config.UseJsonClientSerializer()
					.UseJsonClientDeserializer()
					.UseExistingHttpClient();
			});
		}

		public void Configure(IComponentsApplicationBuilder app)
		{
			app.AddComponent<App>("app");
		}
	}
}
