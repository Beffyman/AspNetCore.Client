using AspNetCore.Client.Serializers;
using System;
using Microsoft.AspNetCore.Blazor;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Uses Blazor's SimpleJson for serializing and deserializing the http content
	/// </summary>
	public class BlazorSimpleJsonSerializer : IHttpSerializer
	{
		/// <summary>
		/// Deserializes the request content which is assumed to be simpleJson into a object of <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public async ValueTask<T> Deserialize<T>(HttpContent content)
		{
			return JsonUtil.Deserialize<T>(await content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Serializes the request into a StringContent with a json media type, but serialized with SimpleJson
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpContent Serialize<T>(T request)
		{
			var json = JsonUtil.Serialize(request);
			return new StringContent(json, Encoding.UTF8, "application/json");
		}
	}
}
