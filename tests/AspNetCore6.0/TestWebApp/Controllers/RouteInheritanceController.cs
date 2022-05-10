using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class RouteInheritanceController : ControllerBase
	{

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IActionResult NoRoute()
		{
			return Ok();
		}

	}
}
