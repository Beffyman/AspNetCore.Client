using MessagePack.AspNetCoreMvcFormatter;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestWebApp.FakeServices;
using TestWebApp.GoodServices;
using TestWebApp.Hubs;

namespace TestWebApp
{
	public class MessagePackStartup
	{
		public MessagePackStartup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddMvc()
					.AddMvcOptions(option =>
					{
						option.OutputFormatters.Clear();
						option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Options));
						option.InputFormatters.Clear();
						option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Options));
					})
					.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddApiVersioning(options =>
			{
				options.AssumeDefaultVersionWhenUnspecified = true;
			});


			services.AddSignalR()
				.AddMessagePackProtocol();

			services.AddTransient<IFakeService, FakeService>();
			services.AddTransient<IGoodService, GoodService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<ChatHub>("/Chat");
			});
		}
	}
}
