using Beffyman.AspNetCore.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TestBlazorApp.Clients;
using TestBlazorApp.Server;

namespace TestBlazorApp.Tests
{
	public abstract class ServerInfo<T> : IDisposable where T : class
	{
		public IServiceProvider Provider { get; }
		public TestServer Server { get; }
		public HttpClient Client { get; }

		public ServerInfo()
		{
			Server = new TestServer(new WebHostBuilder()
					.UseStartup<T>());

			Client = Server.CreateClient();

			var services = new ServiceCollection();
			services.AddTestBlazorClients(ConfigureClient);

			Provider = services.BuildServiceProvider();
		}

		protected abstract void ConfigureClient(ClientConfiguration configure);

		public void Dispose()
		{
			Client.Dispose();
			Server.Dispose();
		}
	}


	public class BlazorJsonServerInfo : ServerInfo<Startup>
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestBlazorAppClientWrapper>(Client)
				.UseBlazorSimpleJsonSerializer()
				.UseBlazorSimpleJsonDeserializer()
				.UseExistingHttpClient();

		}
	}
}
