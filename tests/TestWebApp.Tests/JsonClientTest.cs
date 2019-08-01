using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Beffyman.AspNetCore.Client.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TestWebApp.Clients;
using TestWebApp.Contracts;
using Xunit;

namespace TestWebApp.Tests
{
	public class JsonClientTest
	{
		[Fact]
		public void GetTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var values = valuesClient.GetEnumerable();


				Assert.Equal(new List<string> { "value1", "value2" }, values);
			}
		}

		[Fact]
		public void HeaderTestString()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestString("Val1", "Val2");


				Assert.Equal("Val1", value);
			}
		}

		[Fact]
		public void HeaderTestInt()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var value = valuesClient.HeaderTestInt(15);


				Assert.Equal(15, value);
			}
		}


		[Fact]
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


				Assert.Equal(15, dto.Id);
			}
		}

		[Fact]
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


				Assert.NotEqual(Guid.Empty, g);
			}
		}

		[Fact]
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


				Assert.NotEqual(DateTime.MinValue, g);
			}
		}


		[Fact]
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


				Assert.NotEqual(false, g);
			}
		}


		[Fact]
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

		[Fact]
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

		[Fact]
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


				Assert.Equal(dto.Collision, returnedDto.Collision);
			}
		}


		[Fact]
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


				Assert.Equal(expected, returnedEnumerable);
			}
		}

		[Fact]
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


				Assert.Equal(expected, returnedEnumerable);
			}
		}


		[Fact]
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


				Assert.Equal(expected, actual);
			}
		}


		[Fact]
		public void FileResultTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var fileStream = valuesClient.FileReturn();
				using (var reader = new StreamReader(fileStream))
				{
					var str = reader.ReadToEnd();
					Assert.Equal("Hello World Text", str);
				}
			}
		}


		[Fact]
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

				Assert.True(success);
			}
		}

		[Fact]
		public void TestPreFunc()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();

				bool success = valuesClient.TestPreFunc();

				Assert.True(success);
			}
		}


		[Fact]
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


				Assert.Equal(expected.Collision, actual.Collision);
			}
		}


		[Fact]
		public void DefaultRouteConstraintTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				int? result = valuesClient.DefaultRouteConstraint(null);
				int? expected = 5;

				Assert.Equal(expected, result);
			}
		}

		[Fact]
		public void OptionalRouteConstraintTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				int? result = valuesClient.OptionalRouteConstraint(null);
				int? expected = null;

				Assert.Equal(expected, result);

				int? result2 = valuesClient.OptionalRouteConstraint(123);
				int? expected2 = 123;

				Assert.Equal(expected2, result2);
			}
		}

		[Fact]
		public void DateTimeRouteTests()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var date = DateTime.UtcNow;
				var expected = DateTime.Parse(date.ToString("s", System.Globalization.CultureInfo.InvariantCulture));

				DateTime result = valuesClient.CheckDateTime(date);

				Assert.Equal(expected, result);

				DateTime? nullableResult = valuesClient.CheckDateTimeNullable(date);

				Assert.Equal(expected, nullableResult);
			}
		}

		[Fact]
		public void DateTimeOffsetRouteTests()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var valuesClient = endpoint.Provider.GetService<IValuesClient>();
				var date = DateTimeOffset.UtcNow;
				var expected = DateTimeOffset.Parse(date.ToString("s", System.Globalization.CultureInfo.InvariantCulture));

				DateTimeOffset result = valuesClient.CheckDateTimeOffset(date);

				Assert.Equal(expected, result);

				DateTimeOffset? nullableResult = valuesClient.CheckDateTimeOffsetNullable(date);

				Assert.Equal(expected, nullableResult);
			}
		}

		[Fact]
		public void ApiRouteVersioningTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var v3Client = endpoint.Provider.GetService<Clients.V3_0.ITestRouteClient>();

				var index = v3Client.Endpoint(5);

				Assert.Equal(6, index);
			}
		}

		[Fact]
		public void ApiQueryVersioningTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var v3Client = endpoint.Provider.GetService<Clients.V3.ITestQueryClient>();

				var index = v3Client.Endpoint(5);

				Assert.Equal(6, index);
			}
		}


		[Fact]
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

				Assert.Equal(expected, actual);
			}
		}

		[Fact]
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

				Assert.Equal(100, val.Id);
				Assert.Equal(DateTime.Now.Date, val.When);
				Assert.Equal("Hello", val.Description);

				var responseJson = JsonConvert.DeserializeObject<MyFancyDto>(responseVal);

				Assert.Equal(val.Id, responseJson.Id);
				Assert.Equal(val.When, responseJson.When);
				Assert.Equal(val.Description, responseJson.Description);
			}
		}

		[Fact]
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

		[Fact]
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

				Assert.Equal("Something is right!", str.Single());

			}
		}



		[Fact]
		public void UrlEncodingCheckTest()
		{
			using (var endpoint = new JsonServerInfo())
			{
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
				});

				client.UrlEncodingQueryCheck(val,
				OKCallback: _ =>
				{
					Assert.Equal(val, _);
				},
				BadRequestCallback: _ =>
				{
					throw new Exception("BadRequest should not be thrown");
				});
			}
		}


		[Fact]
		public async Task RouteReplacement()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var client = endpoint.Provider.GetService<IValuesClient>();

				bool hit = false;
				await client.ActionRouteAsync(OKCallback: () =>
				{
					hit = true;
				});

				Assert.True(hit);
			}
		}

		[Fact]
		public async Task InheritedRouteAsync()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var client = endpoint.Provider.GetService<IRouteInheritanceClient>();

				bool hit = false;
				await client.NoRouteAsync(OKCallback: () =>
				{
					hit = true;
				});

				Assert.True(hit);
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
