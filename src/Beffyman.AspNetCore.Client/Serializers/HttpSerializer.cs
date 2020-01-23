using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Beffyman.AspNetCore.Client.Serializers
{
	/// <summary>
	/// This is used to register all deserializers and the serializer that the endpoint uses
	/// </summary>
	public interface IHttpSerializer
	{
		/// <summary>
		/// Deserializes the HttpContent using a deserializer if it has it registered, else it throws an exception
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <exception cref="FormatException" />
		/// <returns></returns>
		Task<T> Deserialize<T>(HttpContent content);

		/// <summary>
		/// Uses the configured serializer to serialize the object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		HttpContent Serialize<T>(T request);

	}

	internal class HttpSerializer : IHttpSerializer
	{
		private readonly ClientConfiguration _config;
		private readonly IServiceProvider _provider;
		private readonly IHttpContentSerializer Serializer;
		private readonly IDictionary<string, IHttpContentSerializer> Deserializers;


		public HttpSerializer(IServiceProvider provider, ClientConfiguration config)
		{
			_provider = provider;
			_config = config;

			Serializer = (IHttpContentSerializer)_provider.GetService(_config.Serializer);

			Deserializers = new Dictionary<string, IHttpContentSerializer>();
			foreach (var serType in _config.Deserializers)
			{
				var ser = (IHttpContentSerializer)_provider.GetService(serType);
				foreach (var contentType in ser.ContentTypes ?? Enumerable.Empty<string>())
				{
					if (Deserializers.ContainsKey(contentType))
					{
						Deserializers[contentType] = ser;
					}
					else
					{
						Deserializers.Add(contentType, ser);
					}
				}
			}
		}


		/// <summary>
		/// Deserializes the response into the correct format
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <exception cref="FormatException" />
		/// <returns></returns>
		public async Task<T> Deserialize<T>(HttpContent content)
		{
			if (content.Headers.ContentType == null)
			{
				return await Deserializers[TextHttpSerializer.CONTENT_TYPE].Deserialize<T>(content).ConfigureAwait(false);
			}

			if (Deserializers.ContainsKey(content.Headers.ContentType.MediaType))
			{
				var serializer = Deserializers[content.Headers.ContentType.MediaType];
				return await serializer.Deserialize<T>(content).ConfigureAwait(false);
			}

			throw new FormatException($"Deserialize Content-Type of {content.Headers.ContentType.MediaType} was unexpected.");
		}

		/// <summary>
		/// Serializes the request into the correct format
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpContent Serialize<T>(T request)
		{
			return Serializer.Serialize<T>(request);
		}

	}
}
