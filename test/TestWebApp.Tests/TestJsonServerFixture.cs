using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TestWebApp.Clients;
using AspNetCore.Client.Core;

namespace TestWebApp.Tests
{
	public class TestJsonServerFixture : IDisposable
	{
		private readonly TestServer Server;
		public IServiceProvider Provider { get; }

		public TestJsonServerFixture()
		{
			Server = new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder()
					.UseStartup<JsonStartup>());

			var client = Server.CreateClient();

			var services = new ServiceCollection();
			services.AddSingleton<HttpClient>(client);
			services.InstallClients(config =>
			{
				config.UseJsonClientSerializer();
			});

			Provider = services.BuildServiceProvider();
		}

		public void Dispose()
		{
			Server.Dispose();
		}
	}
}
