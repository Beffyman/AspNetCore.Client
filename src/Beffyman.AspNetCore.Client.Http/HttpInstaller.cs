using System;
using System.Net.Http;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beffyman.AspNetCore.Client
{
	public static class HttpInstaller
	{
		/// <summary>
		/// Enables the use of Microsoft.Extensions.Http for injecting HttpClients that the IFlurlClient will use.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static ClientConfiguration UseHttpClientFactory<T>(this ClientConfiguration config) where T : IClient
		{
			return config.UseCustomHttpClient<T>((services, constantBaseAddress, httpBaseAddress) =>
			{
				if (constantBaseAddress)
				{
					services.AddSingleton<IFlurlClientFactory, PerHostFlurlClientFactory>();

					services.AddScoped<Func<T, IFlurlClient>>(provider =>
					{
						var factory = provider.GetService<IFlurlClientFactory>();
						return _ => factory.Get(new Flurl.Url(httpBaseAddress(provider)));
					});
				}
				else
				{
					services.AddHttpClient(typeof(T).Name);

					services.AddTransient<HttpClient>(provider =>
					{
						return provider.GetService<System.Net.Http.IHttpClientFactory>().CreateClient(typeof(T).Name);
					});


					services.AddTransient<Func<T, IFlurlClient>>(provider =>
					{
						return _ => new FlurlClient(provider.GetService<HttpClient>());
					});
				}
			});
		}
	}
}
