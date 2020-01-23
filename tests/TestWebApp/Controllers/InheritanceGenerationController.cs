using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Boilerplate;

namespace TestWebApp.Controllers
{
	[Route("api/[controller]")]
	public class InheritanceGenerationController : BaseInheritanceController
	{

		[HttpGet]
		public IActionResult Get()
		{
			return BadRequest("Woops");
		}


	}
}
