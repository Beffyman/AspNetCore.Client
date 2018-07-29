using AspNetCore.Client.Serializers;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Uses Google.Protobuf for serializing and deserializing the http content
	/// </summary>
	internal class ProtobufSerializer : IHttpSerializer
	{
		/// <summary>
		/// Deserializes the request content which is assumed to be protobuf into a object of <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public async ValueTask<T> Deserialize<T>(HttpContent content)
		{
			return Serializer.Deserialize<T>(await content.ReadAsStreamAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Serializes the request into a StringContent with a protobuf media type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpContent Serialize<T>(T request)
		{
			using (var stream = new MemoryStream())
			using (var reader = new StreamReader(stream))
			{
				Serializer.Serialize(stream, request);
				return new StringContent(reader.ReadToEnd(), Encoding.UTF8, "application/x-protobuf");
			}

		}
	}
}
