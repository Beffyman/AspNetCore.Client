using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Authorization
{
	/// <summary>
	/// OAuth Authentication protocol
	/// </summary>
	public class OAuthHeader : SecurityHeader
	{
		public OAuthHeader() { }
		/// <summary>
		/// The Encoded string for the token, use OAuthHeader.Encode if you just want to pass a unencoded string in.
		/// </summary>
		/// <param name="token"></param>
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


		/// <summary>
		/// Runs the ASCII.GetBytes & Convert.ToBase64String for you
		/// </summary>
		/// <param name="token"></param>
		/// <param name="encoding">Defaults to ASCIIEncoding.ASCII</param>
		/// <returns></returns>
		public static OAuthHeader Encode(string token, Encoding encoding = default)
		{
			return new OAuthHeader(Convert.ToBase64String((encoding ?? System.Text.ASCIIEncoding.ASCII).GetBytes(token)));
		}
	}
}
