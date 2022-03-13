using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Beffyman.AspNetCore.Client.Authorization;
using Beffyman.AspNetCore.Client.RequestModifiers;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace Beffyman.AspNetCore.Client.GeneratorExtensions
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
		public static T WithAuth<T>(this T clientOrRequest, SecurityHeader security) where T : IFlurlRequest
		{
			if (security == null)
			{
				throw new ArgumentNullException($"{nameof(SecurityHeader)} provided is null");
			}

			return security.AddAuth(clientOrRequest);
		}

		/// <summary>
		/// Adds the x-functions-key header to the request with the provided authkey
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="AuthKey"></param>
		/// <returns></returns>
		public static T WithFunctionAuthorizationKey<T>(this T clientOrRequest, string AuthKey) where T : IFlurlRequest
		{
			return clientOrRequest.WithHeader("x-functions-key", AuthKey);
		}

		/// <summary>
		/// Applies request modifiers
		/// </summary>
		/// <param name="clientOrRequest"></param>
		/// <param name="requestModifier"></param>
		/// <returns></returns>
		public static IFlurlRequest WithRequestModifiers(this IFlurlRequest clientOrRequest, IHttpRequestModifier requestModifier)
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
		public static T WithCookies<T>(this T clientOrRequest, IEnumerable<Cookie> cookies) where T : IFlurlRequest
		{
			if (cookies != null && cookies.Any())
			{
				foreach (var cookie in cookies)
				{
					clientOrRequest.WithCookie(cookie.Name, cookie.Value);
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
		public static T WithHeaders<T>(this T clientOrRequest, IDictionary<string, object> headers) where T : IFlurlRequest
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

		/// <summary>
		/// Converts the object into a query data string that can be parsed by the client
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public static async Task<string> GetQueryObjectString(this object obj, string parameterName)
		{
			var keyValueContent = obj.ToKeyValue(parameterName);
			var formUrlEncodedContent = new FormUrlEncodedContent(keyValueContent);
			return await formUrlEncodedContent.ReadAsStringAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Converts the object into a URL encoded string
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string EncodeForUrl<T>(this T obj)
		{
			return Uri.EscapeDataString(obj?.ToString() ?? string.Empty);
		}

		/// <summary>
		/// https://geeklearning.io/serialize-an-object-to-an-url-encoded-string-in-csharp/
		/// </summary>
		/// <param name="metaToken"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		private static IDictionary<string, string> ToKeyValue(this object metaToken, string parameterName)
		{
			if (metaToken == null)
			{
				return null;
			}

			JToken token = metaToken as JToken;
			if (token == null)
			{
				return ToKeyValue(JObject.FromObject(metaToken), parameterName);
			}

			if (token.HasValues)
			{
				var contentData = new Dictionary<string, string>();
				foreach (var child in token.Children().ToList())
				{
					var childContent = child.ToKeyValue(null);
					if (childContent != null)
					{
						contentData = contentData.Concat(childContent)
												 .ToDictionary(k => k.Key, v => v.Value);
					}
				}

				return contentData.ToDictionary(x => $"{parameterName}{(!string.IsNullOrEmpty(parameterName) ? "." : string.Empty)}{x.Key}", y => y.Value);
			}

			var jValue = token as JValue;
			if (jValue?.Value == null)
			{
				return null;
			}

			var value = jValue?.Type == JTokenType.Date ?
							jValue?.ToString("o", CultureInfo.InvariantCulture) :
							jValue?.ToString(CultureInfo.InvariantCulture);

			return new Dictionary<string, string> { { token.Path, value } };
		}
	}
}
