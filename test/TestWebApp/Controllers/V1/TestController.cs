using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Controllers.V1
{
	[Route("api/v1/test")]
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
