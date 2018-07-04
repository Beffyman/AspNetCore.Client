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
using TestWebApp.Contracts;

namespace TestWebApp.Tests
{
	public class ProtobufClientTest : IClassFixture<TestProtobufServerFixture>
	{
		private readonly TestProtobufServerFixture TestServer;

		public ProtobufClientTest(TestProtobufServerFixture testServer)
		{
			TestServer = testServer;
		}


		[Fact]
		public void GetTest()
		{
			var valuesClient = TestServer.Provider.GetService<IValuesClient>();
			var values = valuesClient.Get(Accept:"application/x-protobuf");


			Assert.Equal(new List<string> { "value1", "value2" }, values);


		}

		[Fact]
		public void HeaderTestString()
		{
			var valuesClient = TestServer.Provider.GetService<IValuesClient>();
			var value = valuesClient.HeaderTestString("Val1", "Val2", Accept: "application/x-protobuf");


			Assert.Equal("Val1", value);


		}

		[Fact]
		public void HeaderTestInt()
		{
			var valuesClient = TestServer.Provider.GetService<IValuesClient>();
			var value = valuesClient.HeaderTestInt(15, Accept: "application/x-protobuf");


			Assert.Equal(15, value);
		}


		[Fact]
		public void DtoReturns()
		{
			var valuesClient = TestServer.Provider.GetService<IValuesClient>();
			MyFancyDto dto = null;

			valuesClient.FancyDtoReturn(15, Accept: "application/x-protobuf",
				OKCallback: (_) =>
				{
					dto = _;
				});


			Assert.Equal(15, dto.Id);
		}

		/// <summary>
		/// Microsoft.AspNetCore.TestHost.ClientHandler does not respect the CancellationToken and will always complete a request. Their unit test around it ClientCancellationAbortsRequest has a "hack" that cancels in TestServer when the token is canceled.
		/// When the HttpClient has the default HttpMessageHandler, the SendAsync will cancel approriately, until they match this functionality, this test will be disabled
		/// </summary>
		/// <returns></returns>
		//[Fact]
		public async Task CancelTestAsync()
		{
			var valuesClient = TestServer.Provider.GetService<IValuesClient>();

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