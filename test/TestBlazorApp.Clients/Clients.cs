//------------------------------------------------------------------------------
// <auto-generated>
//		This code was generated from a template.
//		Manual changes to this file may cause unexpected behavior in your application.
//		Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using TestBlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Flurl.Http;
using Flurl;
using System.Runtime.CompilerServices;
using AspNetCore.Client;
using AspNetCore.Client.Authorization;
using AspNetCore.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using AspNetCore.Client.Serializers;
using AspNetCore.Client.Http;

namespace TestBlazorApp.Clients
{


	public static class TestBlazorAppClientInstaller
	{
		/// <summary>
		/// Register the autogenerated clients into the container with a lifecycle of scoped.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configure">Overrides for client configuration</param>
		/// <returns></returns>
		public static IServiceCollection InstallClients(this IServiceCollection services, Action<ClientConfiguration> configure = null)
		{
			var configuration = new ClientConfiguration();
			configure?.Invoke(configuration);

			services.AddScoped<TestBlazorAppClient>((provider) => new TestBlazorAppClient(provider.GetService<HttpClient>(), configuration.HttpBaseAddress, configuration.Timeout));

			services.AddScoped<ISampleDataClient, SampleDataClient>();

			return configuration.ApplyConfiguration(services);;
		}
	}



	public class TestBlazorAppClient
	{
		public TimeSpan Timeout { get; internal set; }
		public readonly FlurlClient ClientWrapper;

		public TestBlazorAppClient(HttpClient client, string baseAddress, TimeSpan timeout)
		{
			if (!string.IsNullOrEmpty(baseAddress))
			{
				client.BaseAddress = new Uri(baseAddress);
			}
			ClientWrapper = new FlurlClient(client);
			Timeout = timeout;
		}

	}

	public interface ITestBlazorAppClient : IClient { }


	public interface ISampleDataClient : ITestBlazorAppClient
	{
		
		IEnumerable<WeatherForecast> WeatherForecasts(Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken));

		
		HttpResponseMessage WeatherForecastsRaw(CancellationToken cancellationToken = default(CancellationToken));

		
		ValueTask<IEnumerable<WeatherForecast>> WeatherForecastsAsync(Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken));

		
		ValueTask<HttpResponseMessage> WeatherForecastsRawAsync(CancellationToken cancellationToken = default(CancellationToken));

	}


	public class SampleDataClient : ISampleDataClient
	{
		public readonly TestBlazorAppClient Client;
		public readonly IHttpOverride HttpOverride;
		public readonly IHttpSerializer Serializer;

		public SampleDataClient(TestBlazorAppClient client, IHttpOverride httpOverride, IHttpSerializer serializer)
		{
			Client = client;
			HttpOverride = httpOverride;
			Serializer = serializer;
		}


		public IEnumerable<WeatherForecast> WeatherForecasts(Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken))
		{

			
			var controller = "SampleData";
			var action = "WeatherForecasts";

			string url = $@"api/{controller}/{action}";
			HttpResponseMessage response = null;
			response = HttpOverride.GetResponseAsync(url, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			if(response == null)
			{
				response = Client.ClientWrapper
				.Request(url)
				.AllowAnyHttpStatus()
				.WithTimeout(Client.Timeout)
				.GetAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				
				HttpOverride.OnNonOverridedResponseAsync(url, response, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			}

			if(ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported. As they will run out of the scope of this call.");
			}
			ResponseCallback?.Invoke(response);
			
			if(response.IsSuccessStatusCode)
			{
				return Serializer.Deserialize<IEnumerable<WeatherForecast>>(response.Content).ConfigureAwait(false).GetAwaiter().GetResult();
			}
			else
			{
				return default(IEnumerable<WeatherForecast>);
			}

		}


		public HttpResponseMessage WeatherForecastsRaw(CancellationToken cancellationToken = default(CancellationToken))
		{

			
			var controller = "SampleData";
			var action = "WeatherForecasts";

			string url = $@"api/{controller}/{action}";
			HttpResponseMessage response = null;
			response = HttpOverride.GetResponseAsync(url, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			if(response == null)
			{
				response = Client.ClientWrapper
				.Request(url)
				.AllowAnyHttpStatus()
				.WithTimeout(Client.Timeout)
				.GetAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				
				HttpOverride.OnNonOverridedResponseAsync(url, response, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			}

			return response;
		}


		public async ValueTask<IEnumerable<WeatherForecast>> WeatherForecastsAsync(Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken))
		{

			
			var controller = "SampleData";
			var action = "WeatherForecasts";

			string url = $@"api/{controller}/{action}";
			HttpResponseMessage response = null;
			response = await HttpOverride.GetResponseAsync(url, cancellationToken).ConfigureAwait(false);
			if(response == null)
			{
				response = await Client.ClientWrapper
				.Request(url)
				.AllowAnyHttpStatus()
				.WithTimeout(Client.Timeout)
				.GetAsync(cancellationToken).ConfigureAwait(false);
				
				await HttpOverride.OnNonOverridedResponseAsync(url, response, cancellationToken).ConfigureAwait(false);
			}

			if(ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported. As they will run out of the scope of this call.");
			}
			ResponseCallback?.Invoke(response);
			
			if(response.IsSuccessStatusCode)
			{
				return await Serializer.Deserialize<IEnumerable<WeatherForecast>>(response.Content).ConfigureAwait(false);
			}
			else
			{
				return default(IEnumerable<WeatherForecast>);
			}

		}


		public async ValueTask<HttpResponseMessage> WeatherForecastsRawAsync(CancellationToken cancellationToken = default(CancellationToken))
		{

			
			var controller = "SampleData";
			var action = "WeatherForecasts";

			string url = $@"api/{controller}/{action}";
			HttpResponseMessage response = null;
			response = await HttpOverride.GetResponseAsync(url, cancellationToken).ConfigureAwait(false);
			if(response == null)
			{
				response = await Client.ClientWrapper
				.Request(url)
				.AllowAnyHttpStatus()
				.WithTimeout(Client.Timeout)
				.GetAsync(cancellationToken).ConfigureAwait(false);
				
				await HttpOverride.OnNonOverridedResponseAsync(url, response, cancellationToken).ConfigureAwait(false);
			}

			return response;
		}

	}

}
