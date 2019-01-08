using Microsoft.AspNetCore.Mvc;

namespace TestWebApp.Controllers.V2
{
	[Route("api/v2/test")]
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
