using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core.Authorization
{
	/// <summary>
	/// Basic Authentication protocol
	/// </summary>
	public class BasicAuthHeader : SecurityHeader
	{
		public BasicAuthHeader() { }
		public BasicAuthHeader(string username, string password)
		{
			Username = username;
			Password = password;
		}

		public string Username { get; set; }

		public string Password { get; set; }

		public override T AddAuth<T>(T clientOrRequest)
		{
			return clientOrRequest.WithBasicAuth(Username, Password);
		}
	}
}
