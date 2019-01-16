using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Uses Newtonsoft.Json for serializing and deserializing the http content
	/// </summary>
	internal class TextHttpSerializer : IHttpContentSerializer
	{
		internal static readonly string CONTENT_TYPE = "text/plain";
		public string ContentType => CONTENT_TYPE;


		/// <summary>
		/// Deserializes the request content which is assumed to be json into a object of <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public async Task<T> Deserialize<T>(HttpContent content)
		{
			var str = await content.ReadAsStringAsync().ConfigureAwait(false);

			if (string.IsNullOrEmpty(str))
			{
				return default(T);
			}

			if (typeof(T).IsGenericType
				&& typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return (T)Convert.ChangeType(str, Nullable.GetUnderlyingType(typeof(T)));
			}
			else
			{
				return (T)Convert.ChangeType(await content.ReadAsStringAsync().ConfigureAwait(false), typeof(T));
			}
		}

		/// <summary>
		/// Serializes the request into a StringContent with a json media type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpContent Serialize<T>(T request)
		{
			return new StringContent(request?.ToString());
		}
	}
}
