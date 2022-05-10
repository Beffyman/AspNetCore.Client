using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Controllers.V3
{
	[ApiVersion("3")]
	[Route("api/[controller]")]
	[ApiController]
	public class TestQueryController : ControllerBase
	{
		public TestQueryController()
		{

		}

		[HttpGet("endpoint/{index:int}")]
		public int Endpoint(int index)
		{
			return index + 1;
		}

	}
}
