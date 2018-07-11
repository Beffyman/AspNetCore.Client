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
		public async ValueTask<T> Deserialize<T>(HttpContent content)
		{
			return JsonUtil.Deserialize<T>(await content.ReadAsStringAsync().ConfigureAwait(false));
		}

		public HttpContent Serialize<T>(T request)
		{
			var json = JsonUtil.Serialize(request);
			return new StringContent(json, Encoding.UTF8, "application/json");
		}
	}
}
