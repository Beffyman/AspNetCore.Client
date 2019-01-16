using AspNetCore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestAzureFunction.Clients;

namespace TestAzureFunction.Tests
{
	public abstract class FunctionInfo
	{
		private readonly FunctiontInterceptorHttpHandler Handler;
		private readonly IServiceProvider FunctionProvider;
		public IServiceProvider Provider { get; }
		public HttpClient Client { get; }

		public FunctionInfo()
		{
			FunctionProvider = GetDefaultProvider();
			Handler = new FunctiontInterceptorHttpHandler(FunctionProvider);
			Client = new HttpClient(Handler);
			Client.BaseAddress = new Uri("http://localhost:5000");

			Provider = GetTestProvider();
		}

		private IServiceProvider GetDefaultProvider()
		{
			IServiceCollection collection = new ServiceCollection();

			collection.AddLogging(builder =>
			{
				builder.AddConsole();
			});

			collection.AddSingleton<ILogger>(provider =>
			{
				var factory = provider.GetService<ILoggerFactory>();
				return factory.CreateLogger("Test");
			});

			collection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
			collection.AddMvc();

			return collection.BuildServiceProvider();
		}

		private IServiceProvider GetTestProvider()
		{
			IServiceCollection collection = new ServiceCollection();

			collection.AddTestFunctionClients(ConfigureClient);

			return collection.BuildServiceProvider();
		}

		protected abstract void ConfigureClient(ClientConfiguration collection);
	}

	public class ProtobufServerInfo : FunctionInfo
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestAzureFunctionClientWrapper>(Client)
				.WithProtobufBody()
				.UseProtobufDeserializer()
				.UseProtobufSerializer();
		}
	}


	public class MessagePackServerInfo : FunctionInfo
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestAzureFunctionClientWrapper>(Client)
				.WithMessagePackBody()
				.UseMessagePackDeserializer()
				.UseMessagePackSerializer();
		}
	}

	public class JsonServerInfo : FunctionInfo
	{
		protected override void ConfigureClient(ClientConfiguration configure)
		{
			configure.UseTestServerClient<ITestAzureFunctionClientWrapper>(Client)
				.WithJsonBody()
				.UseJsonClientDeserializer()
				.UseJsonClientSerializer();
		}
	}
}
