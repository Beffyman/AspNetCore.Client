using AspNetCore.Client.Attributes.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Boilerplate;

namespace TestWebApp.Controllers
{
	[Route("api/inheritance")]
	public class InheritanceErrorsController : BaseTestController
	{

		[HttpGet("OverrideTest")]
		[IncludeHeader("TestInheritance", "WORKS")]
		public override void OverrideTest()
		{

		}

	}
}
