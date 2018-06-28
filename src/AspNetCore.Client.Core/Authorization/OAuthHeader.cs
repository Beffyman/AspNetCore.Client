using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core.Authorization
{
	public class OAuthHeader : SecurityHeader
	{
		public OAuthHeader() { }
		public OAuthHeader(string token)
		{
			Token = token;
		}

		public string Token { get; set; }

		public override T AddAuth<T>(T clientOrRequest)
		{
			return clientOrRequest.WithOAuthBearerToken(Token);
		}
	}
}
