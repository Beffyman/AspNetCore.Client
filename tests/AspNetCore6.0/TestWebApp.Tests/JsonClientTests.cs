using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Beffyman.AspNetCore.Client.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TestWebApp.Clients;
using TestWebApp.Contracts;
using Xunit;

namespace TestWebApp.Tests
{
	public class JsonClientTest
	{
		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void GetTest()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var values = valuesClient.GetEnumerable(cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(new List<string> { "value1", "value2" }, values);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void HeaderTestString()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var value = valuesClient.HeaderTestString("Val1", "Val2", cancellationToken: endpoint.TimeoutToken);


			Assert.Equal("Val1", value);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void HeaderTestInt()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var value = valuesClient.HeaderTestInt(15, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(15, value);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void DtoReturns()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			MyFancyDto dto = null;

			valuesClient.FancyDtoReturn(15,
				OKCallback: (_) =>
				{
					dto = _;
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.Equal(15, dto.Id);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void GuidReturns()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			Guid g = Guid.Empty;

			valuesClient.GuidReturn(
				OKCallback: (_) =>
				{
					g = _;
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.NotEqual(Guid.Empty, g);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void DateTimeReturns()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			DateTime g = DateTime.MinValue;

			valuesClient.DateTimeReturns(
				OKCallback: (_) =>
				{
					g = _;
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.NotEqual(DateTime.MinValue, g);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void BoolReturns()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			bool g = false;

			valuesClient.BoolReturns(
				OKCallback: (_) =>
				{
					g = _;
				}, cancellationToken: endpoint.TimeoutToken);

			Assert.True(g);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void RequestAndResponseChecks()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			var response = valuesClient.DtoForDtoRaw(new MyFancyDto
			{
				Id = 1,
				Collision = Guid.NewGuid(),
				Description = "Helo",
				When = DateTime.Now
			}, cancellationToken: endpoint.TimeoutToken);


			Assert.True(response.RequestMessage.Content.Headers.ContentType.MediaType == "application/json");
			Assert.True(response.Content.Headers.ContentType.MediaType == "application/json");
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void PostNoBodyCheck()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			bool ok = false;

			valuesClient.PostWithNoBody(Guid.NewGuid(),
				OKCallback: () =>
				{
					ok = true;
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.True(ok);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void ComplexPostCheck()
		{
			using var endpoint = new JsonServerInfo();
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
				},
				ExceptionCallback: ex =>
				{
					throw ex;
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.Equal(dto.Collision, returnedDto.Collision);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void EnumerableRouteGet()
		{
			using var endpoint = new JsonServerInfo();
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
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.Equal(expected, returnedEnumerable);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void EnumerableRouteGetNullCheck()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			bool badRequest = false;

			valuesClient.EnumerableGet(null, null,
				BadRequestCallback: (_) =>
				{
					badRequest = true;
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.True(badRequest);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void EnumerableRouteGetCustom()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			IEnumerable<int> returnedEnumerable = null;

			var expected = new List<int>
				{
					1,2,3
				};

			valuesClient.EnumerableGetCustom(expected, new List<bool> { true },
			OKCallback: (_) =>
			{
				returnedEnumerable = _;
			}, cancellationToken: endpoint.TimeoutToken);


			Assert.Equal(expected, returnedEnumerable);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void QueryParameterReturns()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			string expected = "FUN";
			string actual = null;

			valuesClient.QueryParameter(expected,
				OKCallback: _ =>
				{
					actual = _;
				}, cancellationToken: endpoint.TimeoutToken);


			Assert.Equal(expected, actual);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task FileResultTest()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var fileStream = await valuesClient.FileReturnAsync(cancellationToken: endpoint.TimeoutToken);
			using var reader = new StreamReader(fileStream);
			var str = await reader.ReadToEndAsync();
			Assert.Equal("Hello World Text", str);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void DeleteAuth()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			bool validRequest = valuesClient.Delete(5, 0, auth: new BasicAuthHeader("Tester", "Test123"), cancellationToken: endpoint.TimeoutToken);
			Assert.True(validRequest);

			bool invalidRequest = valuesClient.Delete(5, 0, auth: new BasicAuthHeader("Tester", "Test12"), cancellationToken: endpoint.TimeoutToken);
			Assert.False(invalidRequest);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void TestPreFunc()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();

			bool success = valuesClient.TestPreFunc(cancellationToken: endpoint.TimeoutToken);

			Assert.True(success);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void QueryObjectTest()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IFullClient>();
			MyFancyDto expected = new MyFancyDto
			{
				Id = 1,
				Collision = Guid.NewGuid(),
				Description = "Hello",
				When = DateTime.Now.Date
			};

			MyFancyDto actual = valuesClient.GetQueryObject(expected, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(expected.Collision, actual.Collision);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void DefaultRouteConstraintTest()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			int? result = valuesClient.DefaultRouteConstraint(null, cancellationToken: endpoint.TimeoutToken);
			int? expected = 5;

			Assert.Equal(expected, result);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void OptionalRouteConstraintTest()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			int? result = valuesClient.OptionalRouteConstraint(null, cancellationToken: endpoint.TimeoutToken);
			int? expected = null;

			Assert.Equal(expected, result);

			int? result2 = valuesClient.OptionalRouteConstraint(123, cancellationToken: endpoint.TimeoutToken);
			int? expected2 = 123;

			Assert.Equal(expected2, result2);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void DateTimeRouteTests()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var date = DateTime.UtcNow;
			var expected = DateTime.Parse(date.ToString("s", System.Globalization.CultureInfo.InvariantCulture));

			DateTime result = valuesClient.CheckDateTime(date, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(expected, result);

			DateTime? nullableResult = valuesClient.CheckDateTimeNullable(date, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(expected, nullableResult);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void DateTimeOffsetRouteTests()
		{
			using var endpoint = new JsonServerInfo();
			var valuesClient = endpoint.Provider.GetService<IValuesClient>();
			var date = DateTimeOffset.UtcNow;
			var expected = DateTimeOffset.Parse(date.ToString("s", System.Globalization.CultureInfo.InvariantCulture));

			DateTimeOffset result = valuesClient.CheckDateTimeOffset(date, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(expected, result);

			DateTimeOffset? nullableResult = valuesClient.CheckDateTimeOffsetNullable(date, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(expected, nullableResult);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void ApiRouteVersioningTest()
		{
			using var endpoint = new JsonServerInfo();
			var v3Client = endpoint.Provider.GetService<Clients.V3_0.ITestRouteClient>();

			var index = v3Client.Endpoint(5, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(6, index);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void ApiQueryVersioningTest()
		{
			using var endpoint = new JsonServerInfo();
			var v3Client = endpoint.Provider.GetService<Clients.V3.ITestQueryClient>();

			var index = v3Client.Endpoint(5, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(6, index);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void InheritanceGenerationBuildTest()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IInheritanceGenerationClient>();

			string expected = "Woops";
			string actual = null;

			client.Get(BadRequestCallback: (str) =>
			{
				actual = str;
			}, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(expected, actual);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void DuplicateMethodReturnAndResponseTypeAttributeTest()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IValuesClient>();

			string responseVal = null;
			MyFancyDto val = client.DuplicateMethodReturnAndResponse(ResponseCallback: msg =>
			{
				responseVal = msg.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
			}, cancellationToken: endpoint.TimeoutToken);

			Assert.Equal(100, val.Id);
			Assert.Equal(DateTime.Now.Date, val.When);
			Assert.Equal("Hello", val.Description);

			var responseJson = JsonSerializer.Deserialize<MyFancyDto>(responseVal, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			Assert.Equal(val.Id, responseJson.Id);
			Assert.Equal(val.When, responseJson.When);
			Assert.Equal(val.Description, responseJson.Description);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void ProblemDetailsRequestTest()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IValuesClient>();

			ValidationProblemDetails errors1 = null;

			var dto = new RequiredDto()
			{
				Id = 1
			};

			client.ProblemDetailsRequest(dto, BadRequestCallback: _ =>
			{
				errors1 = _;
			}, cancellationToken: endpoint.TimeoutToken);

			var dto2 = new RequiredDto()
			{
				Id = 1,
				Field1 = "Hello"
			};

			client.ProblemDetailsRequest(dto2,
				OKCallback: () =>
				{

				}, cancellationToken: endpoint.TimeoutToken
			);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void ModelStateBadRequestTest()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IValuesClient>();

			IReadOnlyDictionary<string, IEnumerable<string>> errors = null;

			client.ModelStateBadRequest(BadRequestCallback: _ =>
			{
				errors = _;
			}, cancellationToken: endpoint.TimeoutToken);

			var str = errors["Test"];

			Assert.Equal("Something is right!", str.Single());
		}



		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public void UrlEncodingCheckTest()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IValuesClient>();

			string val = "?my=1&test=2";
			client.UrlEncodingCheck(val,
			OKCallback: _ =>
			{
				Assert.Equal(val, _);
			},
			BadRequestCallback: _ =>
			{
				throw new Exception("BadRequest should not be thrown");
			}, cancellationToken: endpoint.TimeoutToken);

			client.UrlEncodingQueryCheck(val,
			OKCallback: _ =>
			{
				Assert.Equal(val, _);
			},
			BadRequestCallback: _ =>
			{
				throw new Exception("BadRequest should not be thrown");
			}, cancellationToken: endpoint.TimeoutToken);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task RouteReplacement()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IValuesClient>();

			bool hit = false;
			await client.ActionRouteAsync(OKCallback: () =>
			{
				hit = true;
			}, cancellationToken: endpoint.TimeoutToken);

			Assert.True(hit);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task InheritedRouteAsync()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IRouteInheritanceClient>();

			bool hit = false;
			await client.NoRouteAsync(OKCallback: () =>
			{
				hit = true;
			}, cancellationToken: endpoint.TimeoutToken);

			Assert.True(hit);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task CancellationTokenParameter()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IValuesClient>();

			await client.CancellationTokenApiAsync(0, cancellationToken: endpoint.TimeoutToken);
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task FromServices()
		{
			using var endpoint = new JsonServerInfo();
			var client = endpoint.Provider.GetService<IValuesClient>();

			await client.TestFromServicesAttributeAsync(cancellationToken: endpoint.TimeoutToken);
		}

		/// <summary>
		/// Microsoft.AspNetCore.TestHost.ClientHandler does not respect the CancellationToken and will always complete a request. Their unit test around it ClientCancellationAbortsRequest has a "hack" that cancels in TestServer when the token is canceled.
		/// When the HttpClient has the default HttpMessageHandler, the SendAsync will cancel approriately, until they match this functionality, this test will be disabled
		/// </summary>
		/// <returns></returns>
		//[Fact(Timeout = Constants.TEST_TIMEOUT)]
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
