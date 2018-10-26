using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
