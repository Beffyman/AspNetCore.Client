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
			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType(await content.ReadAsStringAsync().ConfigureAwait(false), typeof(string));
			}

			throw new InvalidCastException($"Cannot convert string to type {typeof(T).Name}");
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
