using AspNetCore.Client.Authorization;
using AspNetCore.Client.RequestModifiers;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client
{
	/// <summary>
	/// Extensions to be used inside the generated clients
	/// </summary>
	public static class GeneratorExtensions
	{
		/// <summary>
		/// Inserts the auth headers into the request based on the <see cref="SecurityHeader"/> implementation
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="security"></param>
		/// <returns></returns>
		public static T WithAuth<T>(this T clientOrRequest, SecurityHeader security) where T : IHttpSettingsContainer
		{
			if (security == null)
			{
				throw new ArgumentNullException($"{nameof(SecurityHeader)} provided is null");
			}

			return security.AddAuth(clientOrRequest);
		}

		/// <summary>
		/// Applies request modifiers
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="requestModifier"></param>
		/// <returns></returns>
		public static T WithRequestModifiers<T>(this T clientOrRequest, IHttpRequestModifier requestModifier) where T : IHttpSettingsContainer
		{
			return requestModifier.ApplyModifiers(clientOrRequest);
		}

		/// <summary>
		/// Adds all of the cookies inside the enumerable into the request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="cookies"></param>
		/// <returns></returns>
		public static T WithCookies<T>(this T clientOrRequest, IEnumerable<Cookie> cookies) where T : IHttpSettingsContainer
		{
			if (cookies != null && cookies.Any())
			{
				foreach (var cookie in cookies)
				{
					clientOrRequest.WithCookie(cookie);
				}
			}
			return clientOrRequest;
		}

		/// <summary>
		/// Adds all the headers inside the dictionary into the request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		public static T WithHeaders<T>(this T clientOrRequest, IDictionary<string, object> headers) where T : IHttpSettingsContainer
		{
			if (headers != null && headers.Any())
			{
				foreach (var header in headers)
				{
					clientOrRequest.WithHeader(header.Key, header.Value);
				}
			}
			return clientOrRequest;
		}

	}
}
