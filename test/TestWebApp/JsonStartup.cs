using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestWebApp.FakeServices;
using TestWebApp.GoodServices;
using TestWebApp.Hubs;
using TestWebApp.Security;

namespace TestWebApp
{
	public class JsonStartup
	{
		public JsonStartup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddAuthentication("BasicAuthentication")
				.AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null);

			services.AddSignalR();

			services.AddTransient<IFakeService, FakeService>();
			services.AddTransient<IGoodService, GoodService>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();

			app.UseSignalR(routes =>
			{
				routes.MapHub<ChatHub>("/Chat");
			});


			app.UseMvc();
		}
	}
}
