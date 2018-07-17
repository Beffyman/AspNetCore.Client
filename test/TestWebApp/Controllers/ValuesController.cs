using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Client;
using AspNetCore.Client.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Contracts;

namespace TestWebApp.Controllers
{
	[Route("api/[controller]")]
	[HeaderParameter("ControllerHeader", typeof(int), "0")]
	//[HeaderParameter("Accept", typeof(string), "application/json")]//This is here so the unit tests have the option of which format
	//[IncludeHeader("Accept", "application/json")]
	//[IncludeHeader("Accept", "application/x-protobuf")]
	[IncludeHeader("Test", "EXTRA")]
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
		[IncludeHeader("GEEET", "FULL")]
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

		[HeaderParameterAttribute("SpecialValue1", typeof(String))]
		[HeaderParameterAttribute("SpecialValue2", "string")]
		[HttpGet("[action]")]
		public string HeaderTestString()
		{
			return Request.Headers["SpecialValue1"].SingleOrDefault();
		}

		[HeaderParameterAttribute("SpecialValue1", typeof(int))]
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

		[HttpPost("[action]")]
		public async ValueTask<IActionResult> TaskReturn(MyFancyDto dto)
		{
			await Task.CompletedTask;
			return Ok();
		}

		[HttpPost("[action]")]
		public MyFancyDto DtoForDto(MyFancyDto dto)
		{
			return dto;
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
		public IActionResult GuidReturn()
		{
			return Ok(Guid.NewGuid());
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(DateTime), (int)HttpStatusCode.OK)]
		public IActionResult DateTimeReturns()
		{
			return Ok(DateTime.Now);
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
		public IActionResult BoolReturns()
		{
			return Ok(true);
		}


		[HttpPost("[action]/{id:guid}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		public IActionResult PostWithNoBody(Guid id)
		{
			return Ok();
		}

		[HttpPost("[action]/{id:guid}")]
		[ProducesResponseType(typeof(MyFancyDto), (int)HttpStatusCode.OK)]
		public IActionResult ComplexPost(Guid id, MyFancyDto dto)
		{
			return Ok(dto);
		}


		[HttpPost("[action]")]
		[ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
		public IActionResult PostWithSimpleBody([FromBody]Guid id)
		{
			return Ok(id);
		}

#warning Until 2.2, the FromQuery with no params doesn't work.  https://github.com/aspnet/Mvc/issues/7712
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IEnumerable<int>), (int)HttpStatusCode.OK)]
		public IActionResult EnumerableGet([FromQuery(Name = "ids")]IEnumerable<int> ids, [FromQuery]IEnumerable<bool> truth)
		{
			if (!truth.Any())
			{
				return Ok(ids);
			}
			return BadRequest("BAD");
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IEnumerable<int>), (int)HttpStatusCode.OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(int))]
		[ProducesResponseType(typeof(int), StatusCodes.Status303SeeOther)]
		public IActionResult AttributeFormatting()
		{
			return Ok();
		}
	}
}
