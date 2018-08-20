using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Controllers
{
	[Route("errorResponseTypes")]
	public class ResponseTypeErrorController : ControllerBase
	{
		[ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
		[HttpGet]
		public void ErrorResponses()
		{

		}
	}
}
