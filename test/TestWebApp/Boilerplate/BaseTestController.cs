using AspNetCore.Client.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Boilerplate
{
	[ApiController]
	[IncludeHeader("TestInheritance","WORKS")]
	public abstract class BaseTestController : ControllerBase
	{



	}
}
