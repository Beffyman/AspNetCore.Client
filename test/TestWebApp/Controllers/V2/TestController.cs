using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Controllers.V2
{
	[Route("api/v2/test")]
	[ApiController]
	public class TestController : ControllerBase
	{
		public TestController()
		{

		}

		[HttpPost("endpoint")]
		public void Endpoint()
		{

		}

	}
}
