using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Boilerplate
{
	[ApiController]
	[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
	public abstract class BaseInheritanceController : ControllerBase
	{
	}
}
