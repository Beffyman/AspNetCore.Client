using AspNetCore.Client.Core;
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
	public class TestProtobufServerFixture : IDisposable
	{
		private readonly TestServer Server;
		public IServiceProvider Provider { get; }

		public TestProtobufServerFixture()
		{
			Server = new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder()
					.UseStartup<ProtobufStartup>());

			var client = Server.CreateClient();

			var services = new ServiceCollection();
			services.AddSingleton<HttpClient>(client);
			services.InstallClients(config=>
			{
				config.UseProtobufSerlaizer();
			});

			Provider = services.BuildServiceProvider();
		}

		public void Dispose()
		{
			Server.Dispose();
		}
	}
}
