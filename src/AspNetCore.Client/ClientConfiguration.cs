using AspNetCore.Client.Http;
using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
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
		private Func<IServiceProvider, string> HttpBaseAddress { get; set; }

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
		private Func<IFlurlClient, ClientSettings, IServiceProvider, IClientWrapper> _clientCreator = null;

		/// <summary>
		/// Whether or not to inject HttpClient
		/// </summary>
		private bool HttpPool = false;

		/// <summary>
		/// Applies the configurations to the <see cref="IServiceCollection"/>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public IServiceCollection ApplyConfiguration<T>(IServiceCollection services) where T : IClient
		{
			if (SerializeType == null)
			{
				SerializeType = typeof(JsonHttpSerializer);
			}

			if (HttpOverrideType == null)
			{
				HttpOverrideType = typeof(DefaultHttpOverride);
			}

			if (_clientRegister != null)
			{
				_clientRegister(services);
			}
			else
			{
				throw new Exception("Error setting up client dependencies register.");
			}

			services.AddScoped(SerializeType);
			services.AddScoped(HttpOverrideType);
			services.AddScoped<Func<T, IHttpSerializer>>(provider => (_ => (IHttpSerializer)provider.GetService(SerializeType)));
			services.AddScoped<Func<T, IHttpOverride>>(provider => (_ => (IHttpOverride)provider.GetService(HttpOverrideType)));

			if (HttpPool)
			{
				services.AddHttpClient(typeof(T).Name);

				services.AddTransient<HttpClient>(provider =>
				{
					return provider.GetService<IHttpClientFactory>().CreateClient(typeof(T).Name);
				});

				services.AddTransient<IFlurlClient>(provider =>
				{
					return new FlurlClient(provider.GetService<HttpClient>());
				});
			}
			else
			{
				services.AddTransient<IFlurlClient>(provider =>
				{
					return new FlurlClient();
				});
			}



			services.AddScoped<IHttpRequestModifier, HttpRequestModifier>((_) =>
			{
				return new HttpRequestModifier
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
		public ClientConfiguration WithBaseAddress(Func<IServiceProvider, string> baseAddress)
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
		/// Enables the use of Microsoft.Extensions.Http for injecting HttpClients that the IFlurlClient will use.
		/// </summary>
		/// <returns></returns>
		public ClientConfiguration UseHttpClientFactory()
		{
			HttpPool = true;
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
		public ClientConfiguration RegisterClientWrapperCreator(Func<IFlurlClient, ClientSettings, IServiceProvider, IClientWrapper> registrationFunc)
		{
			_clientCreator = registrationFunc;

			return this;
		}

		/// <summary>
		/// Overrides this client's configuration to use a specific <see cref="HttpClient"/> instead.  Useful for test servers
		/// </summary>
		/// <typeparam name="TService">Autogenerated interface that inherits of <see cref="IClientWrapper"/> based on the ClientInterfaceName property</typeparam>
		/// <param name="client"></param>
		/// <returns></returns>
		public ClientConfiguration UseTestServerClient<TService>(HttpClient client)
			where TService : class, IClientWrapper
		{
			_clientRegister = (IServiceCollection services) =>
			{
				services.AddScoped((provider) => (TService)_clientCreator(new FlurlClient(client), this.GetSettings(), provider));
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
