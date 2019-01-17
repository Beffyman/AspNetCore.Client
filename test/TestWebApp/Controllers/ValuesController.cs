using AspNetCore.Server.Attributes.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestWebApp.Contracts;
using TestWebApp.GoodServices;

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
		protected readonly IGoodService _goodService;

		public ValuesController(IGoodService goodService)
		{
			_goodService = goodService;
		}

		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<string>> GetEnumerable()
		{
			return new string[] { "value1", "value2" };
		}

		[HttpGet("getAsync")]
		public async Task<ActionResult<IEnumerable<string>>> GetEnumerableTaskAsync()
		{
			await Task.CompletedTask;
			return new string[] { "value1", "value2" };
		}

		[HttpGet("getQualified")]
		public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.IEnumerable<string>>> GetFullyQualified()
		{
			await Task.CompletedTask;
			return new string[] { "value1", "value2" };
		}

		[HttpGet("getTuple")]
		public ActionResult<IEnumerable<(string, int, bool)>> GetTuple()
		{
			return new List<(string, int, bool)>()
			{

			};
		}

		[HttpGet("getNested")]
		[Obsolete("Testing Obsolete")]
		public async Task<ActionResult<IDictionary<string, IEnumerable<Tuple<string, int, bool, char>>>>> GetNestedTypesAsync()
		{
			await Task.CompletedTask;
			return new Dictionary<string, IEnumerable<Tuple<string, int, bool, char>>>()
			{

			};
		}

		[HttpGet("{id}")]
		[IncludeHeader("GEEET", "FULL")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		[HttpGet("dontGenerateMeImPrivate")]
		private void NonClientEndpoint()
		{

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
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[Authorize]
		public bool Delete(int id)
		{
			return true;
		}


		[HttpGet("[action]")]
		public bool TestPreFunc()
		{
			return Request.Headers.ContainsKey("TestPre");
		}

		[HttpGet("[action]")]
		public async Task CancellationTestEndpoint()
		{
			await Task.Delay(10000);
		}

		[NotGenerated]
		[HttpGet("[action]")]
		public void IgnoreMe()
		{

		}

		[HttpGet("[action]/{id:int}")]
		[HeaderParameter("TestId", typeof(int?))]
		public void NullableParameterOrdering(int id, bool deleted = false)
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

		[HttpPost("[action]/{testId:guid}")]
		[ProducesResponseType(typeof(MyFancyDto), (int)HttpStatusCode.OK)]
		public IActionResult ComplexPost([FromRoute(Name = "testId")]Guid id, MyFancyDto dto)
		{
			return Ok(dto);
		}


		[HttpPost("[action]")]
		[ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
		public IActionResult PostWithSimpleBody([FromBody]Guid id)
		{
			return Ok(id);
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IEnumerable<int>), (int)HttpStatusCode.OK)]
		public IActionResult EnumerableGet([FromQuery]IEnumerable<int> ids, [FromQuery]IEnumerable<bool> truth)
		{
			if (truth.Any())
			{
				return Ok(ids);
			}
			return BadRequest("BAD");
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IEnumerable<int>), (int)HttpStatusCode.OK)]
		public IActionResult EnumerableGetCustom([FromQuery(Name = "customIds")]IEnumerable<int> ids, [FromQuery]IEnumerable<bool> truth)
		{
			if (truth.Any())
			{
				return Ok(ids);
			}
			return BadRequest("BAD");
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IEnumerable<int>), (int)HttpStatusCode.OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(int))]
		[ProducesResponseType(typeof(int), StatusCodes.Status303SeeOther)]
		[ProducesResponseType(typeof(string), 304)]
		public IActionResult AttributeFormatting()
		{
			return Ok();
		}


		[HttpGet("[action]")]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
		public IActionResult QueryParameter(string name)
		{
			return Ok(name);
		}


		[HttpGet("[action]")]
		public FileResult FileReturn()
		{
			//PhysicalFileResult
			//FileResult
			//FileContentResult
			//FileStreamResult
			//VirtualFileResult
			byte[] randomizeFile = System.Text.Encoding.UTF8.GetBytes("Hello World Text");

			return File(randomizeFile, "text/plain");
		}

		[HttpGet("[action]")]
		[ProducesResponseType(typeof(Stream), (int)HttpStatusCode.OK)]
		public IActionResult FileReturnResponseTypes(bool pass)
		{
			if (pass)
			{
				byte[] randomizeFile = System.Text.Encoding.UTF8.GetBytes("Hello World Text");
				return File(randomizeFile, "text/plain");
			}
			else
			{
				return BadRequest("Fail");
			}
		}

		[HttpGet("[action]/defaultConstraint/{x=5}")]
		public int? DefaultRouteConstraint(int? x)
		{
			return x;
		}

		[HttpGet("[action]/optional/{x?}")]
		public int? OptionalRouteConstraint(int? x)
		{
			return x;
		}


		[HttpGet("[action]/checkDate/{date}")]
		public DateTime CheckDateTime(DateTime date)
		{
			return date;
		}

		[HttpGet("[action]/checkDate/{date?}")]
		public DateTime CheckDateTimeNullable(DateTime? date)
		{
			return date ?? DateTime.UtcNow;
		}


		[HttpGet("[action]/checkDateOffset/{date}")]
		public DateTimeOffset CheckDateTimeOffset(DateTimeOffset date)
		{
			return date;
		}

		[HttpGet("[action]/checkDateOffset/{date?}")]
		public DateTimeOffset CheckDateTimeOffsetNullable(DateTimeOffset? date)
		{
			return date ?? DateTimeOffset.UtcNow;
		}


		[HttpGet("[action]/routeCheck/{name}/{id:int}/{val}")]
		public void RouteConstraintCheck(string name, int id, bool val)
		{
			return;
		}

	}
}
