using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using TestWebApp.Clients;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace TestWebApp.Tests
{
	public class ClientTest
	{
		public IServiceProvider BuildServer()
		{
			var server = new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder()
					.UseStartup<Startup>());

			Client = server.CreateClient();

			var services = new ServiceCollection();
			services.AddSingleton<HttpClient>(Client);
			services.InstallClients();
			services.AddScoped<IHttpOverride, FakeHttpOverride>();
			return services.BuildServiceProvider();
		}

		private HttpClient Client;
		private readonly IServiceProvider Provider;

		public ClientTest()
		{
			Provider = BuildServer();
		}


		[Fact]
		public void GetTest()
		{
			var valuesClient = Provider.GetService<IValuesClient>();
			var values = valuesClient.Get();


			Assert.Equal(new List<string> { "value1", "value2" }, values);


		}

		[Fact]
		public void HeaderTest()
		{
			var valuesClient = Provider.GetService<IValuesClient>();
			var value = valuesClient.HeaderTest("Val1", "Val2");


			Assert.Equal("Val1", value, StringComparer.CurrentCultureIgnoreCase);


		}

		/// <summary>
		/// Microsoft.AspNetCore.TestHost.ClientHandler does not respect the CancellationToken and will always complete a request. Their unit test around it ClientCancellationAbortsRequest has a "hack" that cancels in TestServer when the token is canceled.
		/// When the HttpClient has the default HttpMessageHandler, the SendAsync will cancel approriately, until they match this functionality, this test will be disabled
		/// </summary>
		/// <returns></returns>
		//[Fact]
		public async Task CancelTestAsync()
		{
			var valuesClient = Provider.GetService<IValuesClient>();

			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			var token = cancellationTokenSource.Token;

			var ex = await Assert.ThrowsAsync<FlurlHttpException>(async () =>
			{
				var task = valuesClient.CancellationTestEndpointAsync(cancellationToken: token);
				cancellationTokenSource.CancelAfter(1500);
				await task.ConfigureAwait(false);
			});

			Assert.True(ex.InnerException is TaskCanceledException);
		}
	}
}
