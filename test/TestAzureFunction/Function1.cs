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

namespace TestAzureFunction
{
	public static class Function1
	{
		[ExpectedBodyParameter(nameof(HttpMethods.Post), typeof(User))]
		[ExpectedQueryParameter("Get", "name", typeof(string))]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		[HeaderParameter("test-header", typeof(Guid))]
		[FunctionName("Function1")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			Guid header = Guid.Parse(req.Headers["test-header"]);

			string queryName = req.Query["name"];

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

			string name = null;

			if (HttpMethods.IsPost(req.Method))
			{
				name = JsonConvert.DeserializeObject<User>(requestBody)?.Name;
			}
			else
			{
				name = queryName;
			}

			return name != null
				? (ActionResult)new OkObjectResult($"Hello, {name}")
				: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
		}
	}
}
