using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TestWebApp.Clients;
using TestWebApp.Contracts;
using Xunit;

namespace TestWebApp.Tests
{
	public class ProtobufClientTest
	{

		[Fact]
		public void GetTest()
		{
			using (var endpoint = new ProtobufServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var values = valuesClient.GetEnumerable();


				Assert.Equal(new List<string> { "value1", "value2" }, values);
			}
		}

		[Fact]
		public void HeaderTestString()
		{
			using (var endpoint = new ProtobufServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestString("Val1", "Val2");


				Assert.Equal("Val1", value);
			}
		}

		[Fact]
		public void HeaderTestInt()
		{
			using (var endpoint = new ProtobufServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestInt(15);


				Assert.Equal(15, value);
			}
		}


		[Fact]
		public void DtoReturns()
		{
			using (var endpoint = new ProtobufServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				MyFancyDto dto = null;

				valuesClient.FancyDtoReturn(15,
					OKCallback: (_) =>
					{
						dto = _;
					});


				Assert.Equal(15, dto.Id);
			}
		}


		[Fact]
		public void RequestAndResponseChecks()
		{
			using (var endpoint = new ProtobufServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();

				var dto = new MyFancyDto
				{
					Id = 1,
					Collision = Guid.NewGuid(),
					Description = "Helo",
					When = DateTime.UtcNow
				};

				var response = valuesClient.DtoForDtoRaw(dto);


				Assert.True(response.RequestMessage.Content.Headers?.ContentType?.MediaType == "application/x-protobuf");
				Assert.True(response.Content.Headers.ContentType.MediaType == "application/x-protobuf");


				var actual = valuesClient.DtoForDto(dto);


				Assert.Equal(dto.Collision, actual.Collision);
				Assert.Equal(dto.Description, actual.Description);
				Assert.Equal(dto.Id, actual.Id);
				Assert.Equal(dto.When, actual.When);
			}
		}

		/// <summary>
		/// Microsoft.AspNetCore.TestHost.ClientHandler does not respect the CancellationToken and will always complete a request. Their unit test around it ClientCancellationAbortsRequest has a "hack" that cancels in TestServer when the token is canceled.
		/// When the HttpClient has the default HttpMessageHandler, the SendAsync will cancel approriately, until they match this functionality, this test will be disabled
		/// </summary>
		/// <returns></returns>
		//[Fact]
		//public async Task CancelTestAsync()
		//{
		//	var endpoint = new ProtobufServerInfo();

		//	var valuesClient = endpoint.Provider.GetService<IValuesClient>();

		//	CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		//	var token = cancellationTokenSource.Token;

		//	Assert.ThrowsAsync<FlurlHttpException>(async () =>
		//	{
		//		var task = valuesClient.CancellationTestEndpointAsync(cancellationToken: token);
		//		cancellationTokenSource.CancelAfter(1500);
		//		await task.ConfigureAwait(false);
		//	});

		//	//Assert.True(ex.InnerException is TaskCanceledException);
		//}
	}
}