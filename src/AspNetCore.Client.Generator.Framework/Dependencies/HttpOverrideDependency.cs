using AspNetCore.Client.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class HttpOverrideDependency : IDependency
	{
		public string Type => nameof(IHttpOverride);
		public string Name => "HttpOverride";
	}
}
