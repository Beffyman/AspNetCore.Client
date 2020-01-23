using AspNetCore.Server.Attributes.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestWebApp.Boilerplate
{
	[ApiController]
	[IncludeHeader("TestInheritance", "WORKS")]
	public abstract class BaseTestController : ControllerBase
	{


		[HttpGet("InheritTest")]
		public void InheritTest()
		{


		}

		[HttpGet("NewTest")]
		public void NewTest()
		{

		}

		[HttpGet("OverrideTest")]
		public virtual void OverrideTest()
		{

		}


		public void TestNonMethod()
		{

		}

	}
}
