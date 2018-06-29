using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core.Authorization
{
	/// <summary>
	/// OAuth Authentication protocol
	/// </summary>
	public class OAuthHeader : SecurityHeader
	{
		public OAuthHeader() { }
		public OAuthHeader(string token)
		{
			Token = token;
		}

		/// <summary>
		/// Token to be used in the header
		/// </summary>
		public string Token { get; set; }

		public override T AddAuth<T>(T clientOrRequest)
		{
			return clientOrRequest.WithOAuthBearerToken(Token);
		}
	}
}
