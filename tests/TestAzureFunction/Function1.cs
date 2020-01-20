using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AspNetCore.Server.Attributes.Functions;
using TestAzureFunction.Contracts;
using AspNetCore.Server.Attributes.Http;
using MessagePack.Resolvers;
using ProtoBuf;
using System.Threading;

namespace TestAzureFunction
{
	public static class Function1
	{
		[ExpectedBodyParameter(nameof(HttpMethods.Post), typeof(User))]
		[ExpectedQueryParameter("Get", "name", typeof(string))]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		[HeaderParameter("ID", typeof(Guid))]
		[FunctionName("Function1")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", nameof(HttpMethods.Post), Route = "helloMe")] HttpRequest req,
			ILogger log,
			CancellationToken token = default)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			if (!req.Headers.ContainsKey("ID"))
			{
				return new BadRequestObjectResult("Header ID was not found");
			}

			Guid header = Guid.Parse(req.Headers["ID"]);

			string name = null;

			if (req.Query.ContainsKey("name"))
			{
				string queryName = req.Query["name"];
				name = queryName;
			}
			else
			{
				if (!HttpMethods.IsPost(req.Method))
				{
					return new BadRequestObjectResult("POST requires a body with Name field");
				}


				if (req.ContentType.Contains("application/json", StringComparison.CurrentCultureIgnoreCase))
				{
					using (var reader = new StreamReader(req.Body))
					{
						name = JsonConvert.DeserializeObject<User>(await reader.ReadToEndAsync())?.Name;
					}
				}
				else if (req.ContentType.Contains("application/x-msgpack", StringComparison.CurrentCultureIgnoreCase))
				{
					name = (await MessagePack.MessagePackSerializer.DeserializeAsync<User>(req.Body, ContractlessStandardResolver.Instance)).Name;
				}
				else if (req.ContentType.Contains("application/x-protobuf", StringComparison.CurrentCultureIgnoreCase))
				{
					name = Serializer.Deserialize<User>(req.Body)?.Name;
				}
				else
				{
					return new BadRequestObjectResult("Content-Type of body not supported. Supported Content-Types are the following: [application/json],[application/x-msgpack],[application/x-protobuf]");
				}
			}

			if (name != null)
			{
				return new OkObjectResult($"Hello, {name}");
			}
			else
			{
				return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
			}
		}
	}
}
