using AspNetCore.Client;
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
	public abstract class ServerInfo<T> where T : class
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
			services.AddTestWebClients(ConfigureClient);

			Provider = services.BuildServiceProvider();
		}

		protected abstract void ConfigureClient(ClientConfiguration configure);
	}


	public class ProtobufServerInfo : ServerInfo<ProtobufStartup>
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestWebAppClientWrapper>(Client)
				.WithProtobufBody()
				.UseProtobufSerializer();
		}
	}

	public class JsonServerInfo : ServerInfo<JsonStartup>
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestWebAppClientWrapper>(Client)
				.WithJsonBody()
				.UseJsonClientSerializer();
		}
	}
}
