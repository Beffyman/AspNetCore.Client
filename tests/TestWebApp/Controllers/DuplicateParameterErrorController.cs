using AspNetCore.Server.Attributes.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TestWebApp.Controllers
{
	[Route("errorParameters")]
	public class DuplicateParameterErrorController : ControllerBase
	{

		[HeaderParameter("id", nameof(Int32))]
		[HttpGet]
		public void ErrorResponses(int id)
		{

		}
	}
}
