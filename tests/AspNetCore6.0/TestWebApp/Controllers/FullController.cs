using Microsoft.AspNetCore.Mvc;
using TestWebApp.Contracts;

namespace TestWebApp.Controllers
{
	/// <summary>
	/// Model binding only works in the full Controller as it is a "View" function
	/// </summary>
	[Route("api/[controller]")]
	public class FullController : Controller
	{
		[HttpGet("[action]")]
		public MyFancyDto GetQueryObject([FromQuery]MyFancyDto dto)
		{
			return dto;
		}

	}
}
