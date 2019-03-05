using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ProtoBuf;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Uses Google.Protobuf for serializing and deserializing the http content
	/// </summary>
	internal class ProtobufSerializer : IHttpContentSerializer
	{
		internal static readonly string CONTENT_TYPE = "application/x-protobuf";
		public string[] ContentTypes => new string[] { CONTENT_TYPE };

		/// <summary>
		/// Deserializes the request content which is assumed to be protobuf into a object of <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public async Task<T> Deserialize<T>(HttpContent content)
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
			var stream = new MemoryStream();
			Serializer.Serialize(stream, request);
			var content = new StreamContent(stream);
			content.Headers.ContentType = new MediaTypeHeaderValue(CONTENT_TYPE);
			return content;
		}
	}
}
