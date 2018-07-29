﻿using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AspNetCore.Client.RequestModifiers
{
	/// <summary>
	/// Applies header modifications predefined inside the configuration
	/// </summary>
	public interface IRequestModifier
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
	/// Implementation of <see cref="IRequestModifier"/> that will apply header modifications defined inside the configuration
	/// </summary>
	internal class RequestModifier : IRequestModifier
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
		/// Applies header modifications to the request
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <returns></returns>
		public T ApplyModifiers<T>(T clientOrRequest) where T : IHttpSettingsContainer
		{
			foreach (var header in PredefinedHeaders)
			{
				clientOrRequest.WithHeader(header.Key, header.Value);
			}

			clientOrRequest.WithCookies(PredefinedCookies);

			return clientOrRequest;
		}
	}
}
