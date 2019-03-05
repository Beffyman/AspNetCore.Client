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
using AspNetCore.Client.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TestWebApp.Tests
{
	[TestFixture]
	public class JsonClientTest
	{
		[Test]
		public void GetTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var values = valuesClient.GetEnumerable();


				Assert.AreEqual(new List<string> { "value1", "value2" }, values);
			}
		}

		[Test]
		public void HeaderTestString()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestString("Val1", "Val2");


				Assert.AreEqual("Val1", value);
			}
		}

		[Test]
		public void HeaderTestInt()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestInt(15);


				Assert.AreEqual(15, value);
			}
		}


		[Test]
		public void DtoReturns()
		{
			using (var endpoint = new JsonServerInfo())
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
		public void GuidReturns()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				Guid g = Guid.Empty;

				valuesClient.GuidReturn(
					OKCallback: (_) =>
					{
						g = _;
					});


				Assert.AreNotEqual(Guid.Empty, g);
			}
		}

		[Test]
		public void DateTimeReturns()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				DateTime g = DateTime.MinValue;

				valuesClient.DateTimeReturns(
					OKCallback: (_) =>
					{
						g = _;
					});


				Assert.AreNotEqual(DateTime.MinValue, g);
			}
		}


		[Test]
		public void BoolReturns()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				bool g = false;

				valuesClient.BoolReturns(
					OKCallback: (_) =>
					{
						g = _;
					});


				Assert.AreNotEqual(false, g);
			}
		}


		[Test]
		public void RequestAndResponseChecks()
		{
			using (var endpoint = new JsonServerInfo())
			{
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
		}

		[Test]
		public void PostNoBodyCheck()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();

				bool ok = false;

				valuesClient.PostWithNoBody(Guid.NewGuid(),
					OKCallback: () =>
					{
						ok = true;
					});


				Assert.True(ok);
			}
		}

		[Test]
		public void ComplexPostCheck()
		{
			using (var endpoint = new JsonServerInfo())
			{
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
		}


		[Test]
		public void EnumerableRouteGet()
		{
			using (var endpoint = new JsonServerInfo())
			{
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
		}

		[Test]
		public void EnumerableRouteGetCustom()
		{
			using (var endpoint = new JsonServerInfo())
			{
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
				});


				Assert.AreEqual(expected, returnedEnumerable);
			}
		}


		[Test]
		public void QueryParameterReturns()
		{
			using (var endpoint = new JsonServerInfo())
			{
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
		}


		[Test]
		public void FileResultTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var fileStream = valuesClient.FileReturn();
				using (var reader = new StreamReader(fileStream))
				{
					var str = reader.ReadToEnd();
					Assert.AreEqual("Hello World Text", str);
				}
			}
		}


		[Test]
		public void DeleteAuth()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();

				bool success = valuesClient.Delete(5, 0, auth: new BasicAuthHeader("Tester", "Test123"));

				Assert.Throws<InvalidOperationException>(() =>
				{
					valuesClient.Delete(5, 0, auth: new BasicAuthHeader("Tester", "Test12"));
				});

				Assert.IsTrue(success);
			}
		}

		[Test]
		public void TestPreFunc()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();

				bool success = valuesClient.TestPreFunc();

				Assert.IsTrue(success);
			}
		}


		[Test]
		public void QueryObjectTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IFullClient>();
				MyFancyDto expected = new MyFancyDto
				{
					Id = 1,
					Collision = Guid.NewGuid(),
					Description = "Hello",
					When = DateTime.Now.Date
				};

				MyFancyDto actual = valuesClient.GetQueryObject(expected);


				Assert.AreEqual(expected.Collision, actual.Collision);
			}
		}


		[Test]
		public void DefaultRouteConstraintTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				int? result = valuesClient.DefaultRouteConstraint(null);
				int? expected = 5;

				Assert.AreEqual(expected, result);
			}
		}

		[Test]
		public void OptionalRouteConstraintTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				int? result = valuesClient.OptionalRouteConstraint(null);
				int? expected = null;

				Assert.AreEqual(expected, result);

				int? result2 = valuesClient.OptionalRouteConstraint(123);
				int? expected2 = 123;

				Assert.AreEqual(expected2, result2);
			}
		}

		[Test]
		public void DateTimeRouteTests()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var date = DateTime.UtcNow;
				var expected = DateTime.Parse(date.ToString("s", System.Globalization.CultureInfo.InvariantCulture));

				DateTime result = valuesClient.CheckDateTime(date);

				Assert.AreEqual(expected, result);

				DateTime? nullableResult = valuesClient.CheckDateTimeNullable(date);

				Assert.AreEqual(expected, nullableResult);
			}
		}

		[Test]
		public void DateTimeOffsetRouteTests()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var date = DateTimeOffset.UtcNow;
				var expected = DateTimeOffset.Parse(date.ToString("s", System.Globalization.CultureInfo.InvariantCulture));

				DateTimeOffset result = valuesClient.CheckDateTimeOffset(date);

				Assert.AreEqual(expected, result);

				DateTimeOffset? nullableResult = valuesClient.CheckDateTimeOffsetNullable(date);

				Assert.AreEqual(expected, nullableResult);
			}
		}

		[Test]
		public void ApiRouteVersioningTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var v3Client = endpoint.Provider.GetService<Clients.V3_0.ITestRouteClient>();

				var index = v3Client.Endpoint(5);

				Assert.AreEqual(6, index);
			}
		}

		[Test]
		public void ApiQueryVersioningTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var v3Client = endpoint.Provider.GetService<Clients.V3.ITestQueryClient>();

				var index = v3Client.Endpoint(5);

				Assert.AreEqual(6, index);
			}
		}


		[Test]
		public void InheritanceGenerationBuildTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var client = endpoint.Provider.GetService<IInheritanceGenerationClient>();

				string expected = "Woops";
				string actual = null;

				client.Get(BadRequestCallback: (str) =>
				{
					actual = str;
				});

				Assert.AreEqual(expected, actual);
			}
		}

		[Test]
		public void DuplicateMethodReturnAndResponseTypeAttributeTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var client = endpoint.Provider.GetService<IValuesClient>();

				string responseVal = null;
				MyFancyDto val = client.DuplicateMethodReturnAndResponse(ResponseCallback: msg =>
				{
					responseVal = msg.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
				});

				Assert.AreEqual(100, val.Id);
				Assert.AreEqual(DateTime.Now.Date, val.When);
				Assert.AreEqual("Hello", val.Description);

				var responseJson = JsonConvert.DeserializeObject<MyFancyDto>(responseVal);

				Assert.AreEqual(val.Id, responseJson.Id);
				Assert.AreEqual(val.When, responseJson.When);
				Assert.AreEqual(val.Description, responseJson.Description);
			}
		}

		[Test]
		public void ProblemDetailsRequestTest()
		{
			using (var endpoint = new JsonServerInfo())
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

		[Test]
		public void ModelStateBadRequestTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var client = endpoint.Provider.GetService<IValuesClient>();

				IReadOnlyDictionary<string, IEnumerable<string>> errors = null;

				client.ModelStateBadRequest(BadRequestCallback: _ =>
				{
					errors = _;
				});

				var str = errors["Test"];

				Assert.AreEqual("Something is right!", str.Single());

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
