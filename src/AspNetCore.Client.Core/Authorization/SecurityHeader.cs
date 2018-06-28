using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core.Authorization
{
	public abstract class SecurityHeader
	{
		public abstract T AddAuth<T>(T clientOrRequest) where T : IHttpSettingsContainer;
	}
}
