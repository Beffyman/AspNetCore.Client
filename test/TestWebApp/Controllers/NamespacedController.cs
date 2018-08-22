using AspNetCore.Client.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
	}
}
