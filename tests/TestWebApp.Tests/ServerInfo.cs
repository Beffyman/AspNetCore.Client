﻿using System;
using System.Net.Http;
using System.Threading;
using Beffyman.AspNetCore.Client;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestWebApp.Clients;

namespace TestWebApp.Tests
{
	public abstract class ServerInfo<T> : IDisposable where T : class
	{
		public IHost Host { get; }
		public IServiceProvider Provider { get; }
		public TestServer Server { get; }
		public HttpClient Client { get; }

		private readonly CancellationTokenSource _tokenSource;
		public readonly CancellationToken TimeoutToken;

		public ServerInfo(int testTimeout = 10_000)
		{
			Host = new HostBuilder()
				.ConfigureWebHost(builder =>
				{
					builder.UseTestServer()
					////.ConfigureServices(services =>
					////{
					////	services.Configure<TestServer>(options =>
					////	{
					////		options.AllowSynchronousIO = true;
					////	});
					////})
					.UseStartup<T>()
					.ConfigureLogging(options =>
					{
						options.AddDebug();
					});
				}).Start();

			Server = Host.GetTestServer();

			Server.AllowSynchronousIO = true;

			Client = Server.CreateClient();

			var services = new ServiceCollection();
			services.AddTestWebClients(ConfigureClient);

			Provider = services.BuildServiceProvider();


			_tokenSource = new CancellationTokenSource(testTimeout);

			TimeoutToken = _tokenSource.Token;

			TimeoutToken.Register(() =>
			{
				if (Host != null)
				{
					var stopTask = Host.StopAsync();
					if (!stopTask.IsCompleted)
					{
						stopTask.GetAwaiter().GetResult();
					}
				}
			});
		}

		protected abstract void ConfigureClient(ClientConfiguration configure);

		public void Dispose()
		{
			Client.Dispose();
			Server.Dispose();
			Host.Dispose();
		}
	}


	public class ProtobufServerInfo : ServerInfo<ProtobufStartup>
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestWebAppClientWrapper>(Client)
				.WithProtobufBody()
				.UseProtobufDeserializer()
				.UseProtobufSerializer();
		}
	}


	public class MessagePackServerInfo : ServerInfo<MessagePackStartup>
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestWebAppClientWrapper>(Client)
				.WithMessagePackBody()
				.UseMessagePackDeserializer()
				.UseMessagePackSerializer();
		}
	}

	public class JsonServerInfo : ServerInfo<JsonStartup>
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestWebAppClientWrapper>(Client)
				.WithJsonBody()
				.WithRequestModifier(request =>
				{
					return request.WithHeader("TestPre", "YES");
				})
				.UseJsonClientDeserializer()
				.UseJsonClientSerializer();
		}
	}

}
