using AspNetCore.Client.Attributes.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Controllers
{
	[Route("errorParameters")]
	public class DuplicateParameterErrorController : ControllerBase
	{

		[HeaderParameter("id",nameof(Int32))]
		[HttpGet]
		public void ErrorResponses(int id)
		{

		}
	}
}
