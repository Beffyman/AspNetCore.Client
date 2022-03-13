using Microsoft.AspNetCore.Mvc;

namespace TestWebApp.Controllers.V1
{
	[Route("api/v1/test")]
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
