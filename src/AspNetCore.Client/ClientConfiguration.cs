using AspNetCore.Client.Http;
using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AspNetCore.Client
{
	/// <summary>
	/// Configuration for the clients
	/// </summary>
	public class ClientConfiguration
	{
		/// <summary>
		/// Base address to be used for a HttpClient being injected
		/// </summary>
		private string HttpBaseAddress { get; set; }

		/// <summary>
		/// What IHttpSerializer to use, defaults to json, allows for custom serialization of requests
		/// </summary>
		private Type SerializeType { get; set; } = typeof(JsonHttpSerializer);

		/// <summary>
		/// What IHttpOverride to use, allows for pre-post request calls
		/// </summary>
		private Type HttpOverrideType { get; set; } = typeof(DefaultHttpOverride);

		/// <summary>
		/// Headers that will always be included with every request
		/// </summary>
		private IDictionary<string, string> PredefinedHeaders { get; } = new Dictionary<string, string>();

		/// <summary>
		/// Headers that will always be included with every request
		/// </summary>
		private List<Cookie> PredefinedCookies { get; } = new List<Cookie>();

		/// <summary>
		/// Override the default timeout, which is 60
		/// </summary>
		private TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

		/// <summary>
		/// Registration for the clients
		/// </summary>
		private Action<IServiceCollection> _clientRegister = null;

		/// <summary>
		/// Func that creates the client wrapper, comes from generated files
		/// </summary>
		private Func<HttpClient, ClientSettings, IClientWrapper> _clientCreator = null;

		/// <summary>
		/// Applies the configurations to the <see cref="IServiceCollection"/>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public IServiceCollection ApplyConfiguration(IServiceCollection services)
		{
			if (SerializeType == null)
			{
				SerializeType = typeof(JsonHttpSerializer);
			}

			if (HttpOverrideType == null)
			{
				HttpOverrideType = typeof(DefaultHttpOverride);
			}

			if(_clientRegister != null)
			{
				_clientRegister(services);
			}
			else
			{
				throw new Exception("Error setting up client dependnecies register.");
			}

			services.AddScoped(typeof(IHttpSerializer), SerializeType);
			services.AddScoped(typeof(IHttpOverride), HttpOverrideType);

			services.AddScoped<IRequestModifier, RequestModifier>((_) =>
			{
				return new RequestModifier
				{
					PredefinedHeaders = PredefinedHeaders,
					PredefinedCookies = PredefinedCookies
				};
			});

			return services;
		}

		/// <summary>
		/// Sets the base address to be used for the injected HttpClients.
		/// </summary>
		/// <param name="baseAddress"></param>
		/// <returns></returns>
		public ClientConfiguration WithBaseAddress(string baseAddress)
		{
			HttpBaseAddress = baseAddress;

			return this;
		}

		/// <summary>
		/// Overrides the default timeout of 60 seconds with the one provided
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public ClientConfiguration WithTimeout(TimeSpan timeout)
		{
			Timeout = timeout;

			return this;
		}

		/// <summary>
		/// Adds a predefined header to the configuration
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ClientConfiguration WithPredefinedHeader(string name, string value)
		{
			PredefinedHeaders.Add(name, value);
			return this;
		}

		/// <summary>
		/// Adds a predefined cookie to the configuration
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ClientConfiguration WithPredefinedCookie(string name, string value)
		{
			PredefinedCookies.Add(new Cookie(name, value));
			return this;
		}

		/// <summary>
		/// Adds the predefined cookies to the configuration
		/// </summary>
		/// <param name="cookies"></param>
		/// <returns></returns>
		public ClientConfiguration WithPredefinedCookies(IEnumerable<Cookie> cookies)
		{
			PredefinedCookies.AddRange(cookies);
			return this;
		}

		/// <summary>
		/// Adds an Accept of "application/json" to every request
		/// </summary>
		/// <returns></returns>
		public ClientConfiguration WithJsonBody()
		{
			return WithPredefinedHeader("Accept", "application/json");
		}

		/// <summary>
		/// Uses <see cref="JsonHttpSerializer"/> to serialize and deserialize requests
		/// </summary>
		public ClientConfiguration UseJsonClientSerializer()
		{
			SerializeType = typeof(JsonHttpSerializer);

			return this;
		}

		/// <summary>
		/// Overrides the default <see cref="IHttpOverride"/> and uses the one provided
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ClientConfiguration UseHttpOverride<T>() where T : IHttpOverride
		{
			HttpOverrideType = typeof(T);
			return this;
		}

		/// <summary>
		/// Overrides the default <see cref="JsonHttpSerializer"/> with the one provided
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ClientConfiguration UseSerializer<T>() where T : IHttpSerializer
		{
			SerializeType = typeof(T);
			return this;
		}

		/// <summary>
		/// Delays registration of the client wrapper into the container, in case we need to override with a test server
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImplementation"></typeparam>
		/// <param name="registrationFunc"></param>
		/// <returns></returns>
		public ClientConfiguration UseClientWrapper<TService, TImplementation>(Func<IServiceProvider, TImplementation> registrationFunc)
			where TImplementation : class, TService
			where TService : class, IClientWrapper
		{
			_clientRegister = (IServiceCollection services) =>
			{
				services.AddScoped<TService, TImplementation>(registrationFunc);
			};

			return this;
		}

		/// <summary>
		/// Registers the client creation for the current clients project into this configuration
		/// </summary>
		/// <param name="registrationFunc"></param>
		/// <returns></returns>
		public ClientConfiguration RegisterClientWrapperCreator(Func<HttpClient, ClientSettings, IClientWrapper> registrationFunc)
		{
			_clientCreator = registrationFunc;

			return this;
		}

		/// <summary>
		/// Overrides this client's configuration to use a specific <see cref="HttpClient"/> instead.  Useful for test servers
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImplementation"></typeparam>
		/// <param name="client"></param>
		/// <returns></returns>
		public ClientConfiguration UseTestServerClient<TService, TImplementation>(HttpClient client)
			where TImplementation : class, TService
			where TService : class, IClientWrapper
		{
			_clientRegister = (IServiceCollection services) =>
			{
				services.AddScoped<TService, TImplementation>((_) => (TImplementation)_clientCreator(client, this.GetSettings()));
			};

			return this;
		}

		/// <summary>
		/// Gets the global settings to be passed into each client
		/// </summary>
		/// <returns></returns>
		public ClientSettings GetSettings()
		{
			return new ClientSettings
			{
				BaseAddress = HttpBaseAddress,
				Timeout = Timeout
			};
		}
	}
}
