using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Controllers.V3
{
	[ApiVersion("3.0")]
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class TestRouteController : ControllerBase
	{
		public TestRouteController()
		{

		}

		[HttpGet("endpoint/{index:int}")]
		public int Endpoint(int index)
		{
			return index + 1;
		}

	}
}
