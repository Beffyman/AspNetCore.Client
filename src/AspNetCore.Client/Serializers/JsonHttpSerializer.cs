using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Uses Newtonsoft.Json for serializing and deserializing the http content
	/// </summary>
	public class JsonHttpSerializer : IHttpSerializer
	{
		private static readonly HashSet<Type> _knownJsonPrimitives = new HashSet<Type>
		{
			typeof(char),
			typeof(byte),
			typeof(sbyte),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(string),
			typeof(bool),
			typeof(DateTime),
			typeof(Guid),
		};

		/// <summary>
		/// Reads non-json primitives back as a string using Convert.ChangeType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		private async ValueTask<T> ReadAsNonJsonAsync<T>(HttpContent content)
		{
			string data = await content.ReadAsStringAsync().ConfigureAwait(false);
			if (typeof(T) == typeof(string))
			{
				data = data.TrimStart('"').TrimEnd('"');
			}
			return (T)Convert.ChangeType(data, typeof(T));
		}


		public async ValueTask<T> Deserialize<T>(HttpContent content)
		{
			if (_knownJsonPrimitives.Contains(typeof(T)))
			{
				return await ReadAsNonJsonAsync<T>(content).ConfigureAwait(false);
			}
			else
			{
				var str = await content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonConvert.DeserializeObject<T>(str);
			}
		}

		public string Serialize<T>(T request)
		{
			return JsonConvert.SerializeObject(request);
		}
	}
}
