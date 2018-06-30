using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TestWebApp.Clients;

namespace TestWebApp.Tests
{
	public class TestServerFixture : IDisposable
	{
		private readonly TestServer Server;
		public IServiceProvider Provider { get; }

		public TestServerFixture()
		{
			Server = new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder()
					.UseStartup<Startup>());

			var client = Server.CreateClient();

			var services = new ServiceCollection();
			services.AddSingleton<HttpClient>(client);
			services.InstallClients();
			services.AddScoped<IHttpOverride, FakeHttpOverride>();
			Provider = services.BuildServiceProvider();
		}

		public void Dispose()
		{
			Server.Dispose();
		}
	}
}
