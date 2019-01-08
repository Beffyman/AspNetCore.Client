using AspNetCore.Server.Attributes;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Boilerplate;

namespace TestWebApp.Controllers
{
	[ApiController]
	[NamespaceSuffix("FancySuffix")]
	[Route("api/namespaced")]
	public class NamespacedController : BaseTestController
	{
		public NamespacedController()
		{

		}

		[HttpGet("test")]
		public void Test()
		{

		}

		[HttpGet("NewTest123")]
		public new void NewTest()
		{

		}

		[HttpGet("NewTest123/{id:int}")]
		public new void NewTest(int id)
		{

		}

		[HttpGet("overwritten")]
		public override void OverrideTest()
		{

		}

	}
}
