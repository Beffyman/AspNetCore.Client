using AspNetCore.Server.Attributes.Http;
using Microsoft.AspNetCore.Mvc;
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
