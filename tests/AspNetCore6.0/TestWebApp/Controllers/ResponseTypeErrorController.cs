using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
