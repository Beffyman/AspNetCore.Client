using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Uses Blazor's SimpleJson for serializing and deserializing the http content
	/// </summary>
	internal class BlazorSimpleJsonSerializer : IHttpSerializer
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
		/// Deserializes the request content which is assumed to be simpleJson into a object of <typeparamref name="T"/>
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
				return Json.Deserialize<T>(str);
			}
		}

		/// <summary>
		/// Serializes the request into a StringContent with a json media type, but serialized with SimpleJson
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpContent Serialize<T>(T request)
		{
			var json = Json.Serialize(request);
			return new StringContent(json, Encoding.UTF8, "application/json");
		}
	}
}
