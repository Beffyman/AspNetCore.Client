using AspNetCore.Server.Attributes.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TestWebApp.Contracts;
using TestWebApp.FakeServices;
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

		/// <summary>
		/// Basic Enumerable get
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult<IEnumerable<string>> GetEnumerable()
		{
			return new string[] { "value1", "value2" };
		}

		/// <summary>
		/// Async ActionResult get
		/// </summary>
		/// <returns></returns>
		[HttpGet("getAsync")]
		public async Task<ActionResult<IEnumerable<string>>> GetEnumerableTaskAsync()
		{
			await Task.CompletedTask;
			return new string[] { "value1", "value2" };
		}

		/// <summary>
		/// Qualified namespace checks
		/// </summary>
		/// <returns></returns>
		[HttpGet("getQualified")]
		public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.IEnumerable<string>>> GetFullyQualified()
		{
			await Task.CompletedTask;
			return new string[] { "value1", "value2" };
		}

		/// <summary>
		/// Tuple support
		/// </summary>
		/// <returns></returns>
		[HttpGet("getTuple")]
		public ActionResult<IEnumerable<(string, int, bool)>> GetTuple()
		{
			return new List<(string, int, bool)>()
			{

			};
		}

		/// <summary>
		/// Large amount of nested type support, as well as Obsolete attribute copying
		/// </summary>
		/// <returns></returns>
		[HttpGet("getNested")]
		[Obsolete("Testing Obsolete")]
		public async Task<ActionResult<IDictionary<string, IEnumerable<Tuple<string, int, bool, char>>>>> GetNestedTypesAsync()
		{
			await Task.CompletedTask;
			return new Dictionary<string, IEnumerable<Tuple<string, int, bool, char>>>()
			{

			};
		}

		/// <summary>
		/// Header injection support
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		[IncludeHeader("GEEET", "FULL")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		/// <summary>
		/// private methods should not be copied
		/// </summary>
		[HttpGet("dontGenerateMeImPrivate")]
		private void NonClientEndpoint()
		{

		}

		/// <summary>
		/// Basic Post
		/// </summary>
		/// <param name="value"></param>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		/// <summary>
		/// Basic Put
		/// </summary>
		/// <param name="id"></param>
		/// <param name="value"></param>
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		/// <summary>
		/// Delete example with Authoize attribute support adding the security parameter
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[Authorize]
		public bool Delete(int id)
		{
			return true;
		}


		/// <summary>
		/// Async naming support (truncates)
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult ActionRouteAsync()
		{
			return Ok();
		}

		/// <summary>
		/// Check if custom headers are being passed via clients
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		public bool TestPreFunc()
		{
			return Request.Headers.ContainsKey("TestPre");
		}

		/// <summary>
		/// Check if timeouts are handled via the server to the client
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		public async Task CancellationTestEndpoint()
		{
			await Task.Delay(10000);
		}

		/// <summary>
		/// Ensure the ignore attribute works
		/// </summary>
		[NotGenerated]
		[HttpGet("[action]")]
		public void IgnoreMe()
		{

		}

		/// <summary>
		/// make sure parameter ordering on the client method is correctish
		/// </summary>
		/// <param name="id"></param>
		/// <param name="deleted"></param>
		[HttpGet("[action]/{id:int}")]
		[HeaderParameter("TestId", typeof(int?))]
		public void NullableParameterOrdering(int id, bool deleted = false)
		{

		}

		/// <summary>
		/// Make sure header parameters are being passed through
		/// </summary>
		/// <returns></returns>
		[HeaderParameterAttribute("SpecialValue1", typeof(String))]
		[HeaderParameterAttribute("SpecialValue2", "string")]
		[HttpGet("[action]")]
		public string HeaderTestString()
		{
			return Request.Headers["SpecialValue1"].SingleOrDefault();
		}

		/// <summary>
		/// support other types
		/// </summary>
		/// <returns></returns>
		[HeaderParameterAttribute("SpecialValue1", typeof(int))]
		[HttpGet("[action]")]
		public int HeaderTestInt()
		{
			return int.Parse(Request.Headers["SpecialValue1"].SingleOrDefault());
		}

		/// <summary>
		/// Make sure "complex" objects can get returned and deserialized
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Make sure ValueTasks are detected as Tasks
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("[action]")]
		public async ValueTask<IActionResult> TaskReturn(MyFancyDto dto)
		{
			await Task.CompletedTask;
			return Ok();
		}

		/// <summary>
		/// Make sure postbacks are correct
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("[action]")]
		public MyFancyDto DtoForDto(MyFancyDto dto)
		{
			return dto;
		}

		/// <summary>
		/// Different return type support (primitives)
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
		public IActionResult GuidReturn()
		{
			return Ok(Guid.NewGuid());
		}

		/// <summary>
		/// Different return type support (primitives)
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(DateTime), (int)HttpStatusCode.OK)]
		public IActionResult DateTimeReturns()
		{
			return Ok(DateTime.Now);
		}

		/// <summary>
		/// Different return type support (primitives)
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
		public IActionResult BoolReturns()
		{
			return Ok(true);
		}

		/// <summary>
		/// Posts with no bodies are allowed
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost("[action]/{id:guid}")]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		public IActionResult PostWithNoBody(Guid id)
		{
			return Ok();
		}

		/// <summary>
		/// Posts with multiple parts for parameters work
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("[action]/{testId:guid}")]
		[ProducesResponseType(typeof(MyFancyDto), (int)HttpStatusCode.OK)]
		public IActionResult ComplexPost([FromRoute(Name = "testId")]Guid id, MyFancyDto dto)
		{
			return Ok(dto);
		}

		/// <summary>
		/// Posts with primitives as the body works
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost("[action]")]
		[ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
		public IActionResult PostWithSimpleBody([FromBody]Guid id)
		{
			return Ok(id);
		}

		/// <summary>
		/// Enumerable query string supported via the client
		/// </summary>
		/// <param name="ids"></param>
		/// <param name="truth"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Make sure the name attribute works
		/// </summary>
		/// <param name="ids"></param>
		/// <param name="truth"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Make sure the different types of response attributes work
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IEnumerable<int>), (int)HttpStatusCode.OK)]
		[ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(int))]
		[ProducesResponseType(typeof(int), StatusCodes.Status303SeeOther)]
		[ProducesResponseType(typeof(string), 304)]
		public IActionResult AttributeFormatting()
		{
			return Ok();
		}


		/// <summary>
		/// ensure that parameters not in route are passed correctly
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
		public IActionResult QueryParameter(string name)
		{
			return Ok(name);
		}

		/// <summary>
		/// Make sure file returns are supported
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Make sure "stream" response types are supported
		/// </summary>
		/// <param name="pass"></param>
		/// <returns></returns>
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

		/// <summary>
		/// make sure default constraints are applied in code if wanted
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		[HttpGet("[action]/defaultConstraint/{x=5}")]
		public int? DefaultRouteConstraint(int? x)
		{
			return x;
		}

		/// <summary>
		/// Ensure that different route constrains works
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		[HttpGet("[action]/optional/{x?}")]
		public int? OptionalRouteConstraint(int? x)
		{
			return x;
		}

		/// <summary>
		/// Make sure dates are passed along correctly
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		[HttpGet("[action]/checkDate/{date}")]
		public DateTime CheckDateTime(DateTime date)
		{
			return date;
		}

		/// <summary>
		/// Make sure dates are passed along correctly
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		[HttpGet("[action]/checkDate/{date?}")]
		public DateTime CheckDateTimeNullable(DateTime? date)
		{
			return date ?? DateTime.UtcNow;
		}

		/// <summary>
		/// Make sure dates are passed along correctly
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		[HttpGet("[action]/checkDateOffset/{date}")]
		public DateTimeOffset CheckDateTimeOffset(DateTimeOffset date)
		{
			return date;
		}

		/// <summary>
		/// Make sure dates are passed along correctly
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		[HttpGet("[action]/checkDateOffset/{date?}")]
		public DateTimeOffset CheckDateTimeOffsetNullable(DateTimeOffset? date)
		{
			return date ?? DateTimeOffset.UtcNow;
		}


		/// <summary>
		/// Checks more route constrains
		/// </summary>
		/// <param name="name"></param>
		/// <param name="id"></param>
		/// <param name="val"></param>
		[HttpGet("[action]/routeCheck/{name}/{id:int}/{val}")]
		public void RouteConstraintCheck(string name, int id, bool val)
		{
			return;
		}

		/// <summary>
		/// Make sure that when the method return matches the OK response type, it doesn't make two, detect them as the same
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(MyFancyDto), StatusCodes.Status200OK)]
		public ActionResult<MyFancyDto> DuplicateMethodReturnAndResponse()
		{
			return new MyFancyDto
			{
				Id = 100,
				Description = "Hello",
				When = DateTime.Now.Date,
				Collision = Guid.NewGuid()
			};
		}

		/// <summary>
		/// Allow for the problem report return content type
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
		public IActionResult ProblemDetailsRequest(RequiredDto dto)
		{
			return Ok();
		}

		/// <summary>
		/// handle model state errors
		/// </summary>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(IReadOnlyDictionary<string, IEnumerable<string>>), StatusCodes.Status400BadRequest)]
		public IActionResult ModelStateBadRequest()
		{
			ModelState.AddModelError("Test", "Something is right!");
			return BadRequest(ModelState);
		}


		/// <summary>
		/// Make sure encoding on the url is handled on the client
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet("[action]/{code}")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		public IActionResult UrlEncodingCheck(string code)
		{
			return Ok(code);
		}

		/// <summary>
		/// make sure query parameters are encoded properly
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		public IActionResult UrlEncodingQueryCheck(string code)
		{
			return Ok(code);
		}


		/// <summary>
		/// Need to make sure the token parameter isn't copied to the client
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		[HttpGet("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> CancellationTokenApi(CancellationToken token = default)
		{
			await Task.Delay(2500, token);

			return Ok();
		}

		[HttpGet("[action]")]
		public Task TestFromServicesAttributeAsync([FromServices] IFakeService fakeService)
		{
			return Task.CompletedTask;
		}

	}
}
