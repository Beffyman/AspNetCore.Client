using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using TestWebApp.Clients;
using System.Net.Http;
using System.Collections.Generic;

namespace TestWebApp.Tests
{
	public class ClientTest
	{
		public IServiceProvider BuildServer()
		{
			var server = new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder()
					.UseStartup<Startup>());

			var client = server.CreateClient();

			var services = new ServiceCollection();
			services.AddSingleton<HttpClient>(client);
			services.InstallClients();
			return services.BuildServiceProvider();
		}


		[Fact]
		public void Test1()
		{
			var provider = BuildServer();

			var valuesClient = provider.GetService<IValuesClient>();
			var values = valuesClient.Get();


			Assert.Equal(new List<string> { "value1", "value2" }, values);
		}
	}
}
