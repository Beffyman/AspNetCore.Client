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
	internal class JsonHttpSerializer : IHttpSerializer
	{
		private static readonly IDictionary<Type, Func<string, object>> _knownJsonPrimitives = new Dictionary<Type, Func<string, object>>
		{
			{ typeof(char), (_)=> char.Parse(_) },
			{ typeof(byte), (_)=> byte.Parse(_) },
			{ typeof(sbyte), (_)=> sbyte.Parse(_) },
			{ typeof(ushort), (_)=> ushort.Parse(_) },
			{ typeof(int), (_)=> int.Parse(_) },
			{ typeof(uint), (_)=> uint.Parse(_) },
			{ typeof(long), (_)=> long.Parse(_) },
			{ typeof(ulong), (_)=> ulong.Parse(_) },
			{ typeof(float), (_)=> float.Parse(_) },
			{ typeof(double), (_)=> double.Parse(_) },
			{ typeof(string), (_)=> _.TrimStart('"').TrimEnd('"') },
			{ typeof(bool), (_)=> bool.Parse(_) },
			{ typeof(DateTime), (_)=> DateTime.Parse(_.TrimStart('"').TrimEnd('"')) },
			{ typeof(Guid), (_)=> Guid.Parse(_.TrimStart('"').TrimEnd('"')) },
		};

		/// <summary>
		/// Deserializes the request content which is assumed to be json into a object of <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public async ValueTask<T> Deserialize<T>(HttpContent content)
		{
			if (_knownJsonPrimitives.ContainsKey(typeof(T)))
			{
				return (T)_knownJsonPrimitives[typeof(T)](await content.ReadAsStringAsync().ConfigureAwait(false));
			}
			else
			{
				var str = await content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonConvert.DeserializeObject<T>(str);
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
			var json = JsonConvert.SerializeObject(request);
			return new StringContent(json, Encoding.UTF8, "application/json");
		}
	}
}
