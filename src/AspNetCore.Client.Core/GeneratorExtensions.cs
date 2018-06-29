using AspNetCore.Client.Core.Authorization;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Core
{
	public static class GeneratorExtensions
	{

		/// <summary>
		/// Reads non-json back as a string using Convert.ChangeType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public static async ValueTask<T> ReadAsNonJsonAsync<T>(this HttpContent content)
		{
			string data = await content.ReadAsStringAsync().ConfigureAwait(false);
			return (T)Convert.ChangeType(data, typeof(T));
		}

		/// <summary>
		/// Inserts the auth headers into the request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="security"></param>
		/// <returns></returns>
		public static T WithAuth<T>(this T clientOrRequest, SecurityHeader security) where T: IHttpSettingsContainer
		{
			if(security == null)
			{
				throw new ArgumentNullException($"{nameof(SecurityHeader)} provided is null");
			}

			return security.AddAuth(clientOrRequest);
		}
	}
}
