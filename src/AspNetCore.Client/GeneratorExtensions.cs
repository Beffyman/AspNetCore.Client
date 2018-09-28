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
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace AspNetCore.Client.GeneratorExtensions
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

		/// <summary>
		/// Converts the object into a query data string that can be parsed by the client
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public static string GetQueryObjectString(this object obj, string parameterName)
		{
			var keyValueContent = obj.ToKeyValue(parameterName);
			var formUrlEncodedContent = new FormUrlEncodedContent(keyValueContent);
			return formUrlEncodedContent.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
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
