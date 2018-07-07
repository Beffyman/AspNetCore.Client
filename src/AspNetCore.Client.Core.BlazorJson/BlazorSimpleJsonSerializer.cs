using AspNetCore.Client.Core.Serializers;
using System;
using Microsoft.AspNetCore.Blazor;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Core.Serializers
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

		public string Serialize<T>(T request)
		{
			return JsonUtil.Serialize(request);
		}
	}
}
