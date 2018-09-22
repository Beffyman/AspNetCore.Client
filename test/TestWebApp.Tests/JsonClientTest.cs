using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TestWebApp.Clients;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using TestWebApp.Contracts;
using AspNetCore.Client;
using NUnit.Framework;
using System.IO;

namespace TestWebApp.Tests
{
	[TestFixture]
	public class JsonClientTest
	{
		[Test]
		public void GetTest()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var values = valuesClient.Get();


			Assert.AreEqual(new List<string> { "value1", "value2" }, values);


		}

		[Test]
		public void HeaderTestString()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var value = valuesClient.HeaderTestString("Val1", "Val2");


			Assert.AreEqual("Val1", value);


		}

		[Test]
		public void HeaderTestInt()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var value = valuesClient.HeaderTestInt(15);


			Assert.AreEqual(15, value);
		}


		[Test]
		public void DtoReturns()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			MyFancyDto dto = null;

			valuesClient.FancyDtoReturn(15,
				OKCallback: (_) =>
				{
					dto = _;
				});


			Assert.AreEqual(15, dto.Id);
		}

		[Test]
		public void GuidReturns()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			Guid g = Guid.Empty;

			valuesClient.GuidReturn(
				OKCallback: (_) =>
				{
					g = _;
				});


			Assert.AreNotEqual(Guid.Empty, g);
		}

		[Test]
		public void DateTimeReturns()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			DateTime g = DateTime.MinValue;

			valuesClient.DateTimeReturns(
				OKCallback: (_) =>
				{
					g = _;
				});


			Assert.AreNotEqual(DateTime.MinValue, g);
		}


		[Test]
		public void BoolReturns()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			bool g = false;

			valuesClient.BoolReturns(
				OKCallback: (_) =>
				{
					g = _;
				});


			Assert.AreNotEqual(false, g);
		}


		[Test]
		public void RequestAndResponseChecks()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			var response = valuesClient.DtoForDtoRaw(new MyFancyDto
			{
				Id = 1,
				Collision = Guid.NewGuid(),
				Description = "Helo",
				When = DateTime.Now
			});


			Assert.True(response.RequestMessage.Content.Headers.ContentType.MediaType == "application/json");
			Assert.True(response.Content.Headers.ContentType.MediaType == "application/json");
		}

		[Test]
		public void PostNoBodyCheck()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			bool ok = false;

			valuesClient.PostWithNoBody(Guid.NewGuid(),
				OKCallback: () =>
				{
					ok = true;
				});


			Assert.True(ok);
		}

		[Test]
		public void ComplexPostCheck()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			MyFancyDto returnedDto = null;

			var dto = new MyFancyDto
			{
				Collision = Guid.NewGuid(),
				Description = "Test",
				Id = 12,
				When = DateTime.Now
			};

			valuesClient.ComplexPost(dto, Guid.NewGuid(),
			OKCallback: (_) =>
			{
				returnedDto = _;
			});


			Assert.AreEqual(dto.Collision, returnedDto.Collision);
		}


		[Test]
		public void EnumerableRouteGet()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			IEnumerable<int> returnedEnumerable = null;

			var expected = new List<int>
			{
				1,2,3
			};

			valuesClient.EnumerableGet(expected, new List<bool> { true },
			OKCallback: (_) =>
			{
				returnedEnumerable = _;
			});


			Assert.AreEqual(expected, returnedEnumerable);
		}


		[Test]
		public void QueryParameterReturns()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			string expected = "FUN";
			string actual = null;

			valuesClient.QueryParameter(expected,
				OKCallback: _ =>
				 {
					 actual = _;
				 });


			Assert.AreEqual(expected, actual);
		}


		[Test]
		public void FileResultTest()
		{
			var endpoint = new JsonServerInfo();

			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var fileStream = valuesClient.FileReturn();
			using (var reader = new StreamReader(fileStream))
			{
				var str = reader.ReadToEnd();
				Assert.AreEqual("Hello World Text", str);
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
		//	var endpoint = new JsonServerInfo();

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
