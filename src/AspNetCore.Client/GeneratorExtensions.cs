using AspNetCore.Client.Authorization;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client
{
	public static class GeneratorExtensions
	{

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
