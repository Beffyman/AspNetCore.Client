using System;
using System.Collections.Generic;
using System.Net;
using Flurl.Http;

namespace Beffyman.AspNetCore.Client.RequestModifiers
{
	/// <summary>
	/// Applies header modifications predefined inside the configuration
	/// </summary>
	public interface IHttpRequestModifier
	{
		/// <summary>
		/// Header modifications that are defined inside the configuration
		/// </summary>
		IDictionary<string, string> PredefinedHeaders { get; set; }

		/// <summary>
		/// Cookie modifications that are defined inside the configuration
		/// </summary>
		IEnumerable<Cookie> PredefinedCookies { get; set; }

		/// <summary>
		/// Applies header modifications to the request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <returns></returns>
		T ApplyModifiers<T>(T clientOrRequest) where T : IHttpSettingsContainer;
	}

	/// <summary>
	/// Implementation of <see cref="IHttpRequestModifier"/> that will apply header modifications defined inside the configuration
	/// </summary>
	internal class HttpRequestModifier : IHttpRequestModifier
	{
		/// <summary>
		/// Header modifications that are defined inside the configuration
		/// </summary>
		public IDictionary<string, string> PredefinedHeaders { get; set; }

		/// <summary>
		/// Cookie modifications that are defined inside the configuration
		/// </summary>
		public IEnumerable<Cookie> PredefinedCookies { get; set; }

		/// <summary>
		/// Functions that will always be ran on a request
		/// </summary>
		public ICollection<Func<IHttpSettingsContainer, IHttpSettingsContainer>> PredefinedFunctions { get; set; }


		/// <summary>
		/// Applies header modifications to the request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <returns></returns>
		public T ApplyModifiers<T>(T clientOrRequest) where T : IHttpSettingsContainer
		{
			var request = clientOrRequest.WithHeaders(PredefinedHeaders)
											.WithCookies(PredefinedCookies);

			if (PredefinedFunctions != null)
			{
				foreach (var func in PredefinedFunctions)
				{
					request = (T)func(request);
				}
			}

			return request;
		}
	}
}
