using System;
using System.Text;
using Flurl.Http;

namespace AspNetCore.Client.Authorization
{
	/// <summary>
	/// OAuth Authentication protocol
	/// </summary>
	public class OAuthHeader : SecurityHeader
	{
		/// <summary>
		/// The Encoded string for the token, use <see cref="OAuthHeader.Encode(string, Encoding)"/> if you just want to pass a unencoded string in.
		/// </summary>
		/// <param name="token"></param>
		public OAuthHeader(string token)
		{
			Token = token;
		}

		/// <summary>
		/// Token to be used in the header
		/// </summary>
		private string Token { get; }

		/// <summary>
		///Implementation of <see cref="SecurityHeader.AddAuth{T}(T)"/> which uses <see cref="HeaderExtensions.WithOAuthBearerToken{T}(T, string)"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <returns></returns>
		public override T AddAuth<T>(T clientOrRequest)
		{
			return clientOrRequest.WithOAuthBearerToken(Token);
		}


		/// <summary>
		/// Runs the <see cref="Encoding.GetBytes(string)"/> and <see cref="Convert.ToBase64String(byte[])"/> for you
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
