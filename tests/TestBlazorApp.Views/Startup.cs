using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beffyman.AspNetCore.Client;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using TestBlazorApp.Clients;

namespace TestBlazorApp.Views
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTestBlazorClients(config =>
			{
				config.UseBlazorSimpleJsonSerializer()
						.UseExistingHttpClient();
			});
		}

		public void Configure(IBlazorApplicationBuilder app)
		{
			app.AddComponent<App>("app");
		}
	}
}
