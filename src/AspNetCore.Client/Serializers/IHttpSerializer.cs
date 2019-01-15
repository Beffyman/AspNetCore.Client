using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Client.Serializers
{
	public interface IHttpSerializer<T> where T : IClient
	{
		Task<T> Deserialize(HttpContent content);
		HttpContent Serialize(T request, string contentType);

	}

	internal class HttpSerializer<T> : IHttpSerializer<T> where T : IClient
	{
		private readonly ClientConfiguration _config;
		private readonly IServiceProvider _provider;
		private readonly IDictionary<string, IHttpContentSerializer> Serializers;


		public HttpSerializer(IServiceProvider provider, ClientConfiguration config)
		{
			_provider = provider;
			_config = config;

			Serializers = new Dictionary<string, IHttpContentSerializer>();
			foreach (var serType in _config.SerializeTypes)
			{
				var ser = (IHttpContentSerializer)_provider.GetService(serType);
				Serializers.Add(ser.ContentType, ser);
			}
		}


		/// <summary>
		/// Deserializes the response into the correct format
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public async Task<T> Deserialize(HttpContent content)
		{
			if (Serializers.ContainsKey(content.Headers.ContentType.MediaType))
			{
				var serializer = Serializers[content.Headers.ContentType.MediaType];
				return await serializer.Deserialize<T>(content).ConfigureAwait(false);
			}

			throw new FormatException($"Deserialize Content-Type of {content.Headers.ContentType.MediaType} was unexpected.");
		}

		/// <summary>
		/// Serializes the request into the correct format
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public HttpContent Serialize(T request, string contentType)
		{
			if (Serializers.ContainsKey(contentType))
			{
				var serializer = Serializers[contentType];
				return serializer.Serialize<T>(request);
			}

			throw new FormatException($"Serialize Content-Type of {contentType} was unexpected.");
		}

	}
}
