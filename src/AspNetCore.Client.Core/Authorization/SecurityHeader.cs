using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core.Authorization
{
	/// <summary>
	/// Abstraction around security modifier to http request
	/// </summary>
	public abstract class SecurityHeader
	{
		/// <summary>
		/// Adds auth headers depending on the implementation
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <returns></returns>
		public abstract T AddAuth<T>(T clientOrRequest) where T : IHttpSettingsContainer;
	}
}
