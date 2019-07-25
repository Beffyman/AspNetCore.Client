using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using NUnit.Framework;
using TestWebApp.Clients;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using TestWebApp.Contracts;
using Microsoft.AspNetCore.TestHost;
using Beffyman.AspNetCore.Client;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApp.Tests
{
	[TestFixture]
	public class MessagePackClientTest
	{

		[Test]
		public void GetTest()
		{
			using (var endpoint = new MessagePackServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var values = valuesClient.GetEnumerable();


				Assert.AreEqual(new List<string> { "value1", "value2" }, values);
			}
		}

		[Test]
		public void HeaderTestString()
		{
			using (var endpoint = new MessagePackServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestString("Val1", "Val2");


				Assert.AreEqual("Val1", value);
			}
		}

		[Test]
		public void HeaderTestInt()
		{
			using (var endpoint = new MessagePackServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestInt(15);


				Assert.AreEqual(15, value);
			}
		}


		[Test]
		public void DtoReturns()
		{
			using (var endpoint = new MessagePackServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				MyFancyDto dto = null;

				valuesClient.FancyDtoReturn(15,
					OKCallback: (_) =>
					{
						dto = _;
					});


				Assert.AreEqual(15, dto.Id);
			}
		}


		[Test]
		public void RequestAndResponseChecks()
		{
			using (var endpoint = new MessagePackServerInfo())
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


				Assert.True(response.RequestMessage.Content.Headers?.ContentType?.MediaType == "application/x-msgpack");
				Assert.True(response.Content.Headers.ContentType.MediaType == "application/x-msgpack");



				var actual = valuesClient.DtoForDto(dto);

				Assert.AreEqual(dto.Collision, actual.Collision);
				Assert.AreEqual(dto.Description, actual.Description);
				Assert.AreEqual(dto.Id, actual.Id);
				Assert.AreEqual(dto.When, actual.When);
			}
		}


		[Test]
		public void ProblemDetailsRequestTest()
		{
			//Doesn't seem like we need a application/problem+x-msgpack content-type
			using (var endpoint = new MessagePackServerInfo())
			{
				var client = endpoint.Provider.GetService<IValuesClient>();

				ValidationProblemDetails errors1 = null;

				var dto = new RequiredDto()
				{
					Id = 1
				};

				client.ProblemDetailsRequest(dto, BadRequestCallback: _ =>
				{
					errors1 = _;
				});

				var dto2 = new RequiredDto()
				{
					Id = 1,
					Field1 = "Hello"
				};

				client.ProblemDetailsRequest(dto2,
					OKCallback: () =>
					{

					}
				);
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