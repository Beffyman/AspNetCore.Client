using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Client.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Contracts;

namespace TestWebApp.Controllers
{
	[Route("api/[controller]")]
	[IncludesHeader("ControllerHeader", typeof(int), "0")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
	[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		[Authorize]
		public void Delete(int id)
		{
		}

		[HttpGet("[action]")]
		public async Task CancellationTestEndpoint()
		{
			await Task.Delay(10000);
		}

		[NoClient]
		[HttpGet("[action]")]
		public void IgnoreMe()
		{

		}

		[IncludesHeader("SpecialValue1", typeof(String))]
		[IncludesHeader("SpecialValue2", "string")]
		[HttpGet("[action]")]
		public string HeaderTestString()
		{
			return Request.Headers["SpecialValue1"].SingleOrDefault();
		}

		[IncludesHeader("SpecialValue1", typeof(int))]
		[HttpGet("[action]")]
		public int HeaderTestInt()
		{
			return int.Parse(Request.Headers["SpecialValue1"].SingleOrDefault());
		}

		[HttpGet("[action]/{id:int}")]
		[ProducesResponseType(typeof(MyFancyDto), (int)HttpStatusCode.OK)]
		public IActionResult FancyDtoReturn(int id)
		{
			return Ok(new MyFancyDto
			{
				Id = id,
				Collision = Guid.NewGuid(),
				Description = "Hello There",
				When = DateTime.UtcNow
			});
		}
	}
}
