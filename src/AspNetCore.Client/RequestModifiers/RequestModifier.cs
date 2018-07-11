using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.RequestModifiers
{
	public interface IRequestModifier
	{
		IDictionary<string, string> PredefinedHeaders { get; set; }

		T ApplyModifiers<T>(T clientOrRequest) where T : IHttpSettingsContainer;
	}

	public class RequestModifier : IRequestModifier
	{
		public IDictionary<string, string> PredefinedHeaders { get; set; }

		public RequestModifier()
		{

		}

		public T ApplyModifiers<T>(T clientOrRequest) where T : IHttpSettingsContainer
		{
			foreach (var header in PredefinedHeaders)
			{
				clientOrRequest.WithHeader(header.Key, header.Value);
			}

			return clientOrRequest;
		}

	}
}
