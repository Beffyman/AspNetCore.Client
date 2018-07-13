using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Authorization
{
	/// <summary>
	/// Basic Authentication protocol
	/// </summary>
	public class BasicAuthHeader : SecurityHeader
	{
		/// <summary>
		/// Provides a BasicAuth header with the username and password provided
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public BasicAuthHeader(string username, string password)
		{
			Username = username;
			Password = password;
		}

		private string Username { get; }

		private string Password { get; }

		/// <summary>
		/// Implementation of <see cref="SecurityHeader.AddAuth{T}(T)"/> which uses <see cref="HeaderExtensions.WithBasicAuth{T}(T, string, string)"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <returns></returns>
		public override T AddAuth<T>(T clientOrRequest)
		{
			return clientOrRequest.WithBasicAuth(Username, Password);
		}
	}
}
