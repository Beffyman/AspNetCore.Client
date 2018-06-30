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
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Flurl.Http;
using Flurl;
using System.Runtime.CompilerServices;
using AspNetCore.Client.Core;
using AspNetCore.Client.Core.Authorization;
using AspNetCore.Client.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Microsoft.AspNetCore.Blazor;

namespace TestBlazorApp.Clients
{


	public static class TestBlazorAppClientInstaller
	{
		/// <summary>
		/// Register the autogenerated clients into the container with a lifecycle of scoped.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="baseAddress">Address to be used inside the HttpClient injected</param>
		/// <returns></returns>
		public static IServiceCollection InstallClients(this IServiceCollection services, string baseAddress = null)
		{
			services.AddScoped<TestBlazorAppClient>((provider) =>
			{
				var client = provider.GetService<HttpClient>();
				if(baseAddress != null)
				{
					client.BaseAddress = new Uri(baseAddress);
				}
				return new TestBlazorAppClient(client);
			});

			services.AddScoped<ISampleDataClient, SampleDataClient>();
			return services;
		}
	}




	public class TestBlazorAppClient
	{
		public readonly FlurlClient ClientWrapper;

		public TestBlazorAppClient(HttpClient client)
		{
			ClientWrapper = new FlurlClient(client);
		}
	}

	public interface ITestBlazorAppClient : IClient { }



	public interface ISampleDataClient : ITestBlazorAppClient
	{
		
		IEnumerable<WeatherForecast> WeatherForecasts(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken));

		
		HttpResponseMessage WeatherForecastsRaw(CancellationToken cancellationToken = default(CancellationToken));

		
		ValueTask<IEnumerable<WeatherForecast>> WeatherForecastsAsync(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken));

		
		ValueTask<HttpResponseMessage> WeatherForecastsRawAsync(CancellationToken cancellationToken = default(CancellationToken));

	}


	public class SampleDataClient : ISampleDataClient
	{
		public readonly TestBlazorAppClient Client;

		public SampleDataClient(TestBlazorAppClient client)
		{
			Client = client;
		}


		public IEnumerable<WeatherForecast> WeatherForecasts(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken))
		{

			
			var controller = "SampleData";
			var action = "WeatherForecasts";

			string url = $@"api/{controller}/{action}";
			HttpResponseMessage response = null;
			
			if(response == null)
			{
				response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				
			}

			if(BadRequestCallback != null && BadRequestCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for BadRequestCallback are not supported. As they will run out of the scope of this call.");
			}
			if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				BadRequestCallback?.Invoke(response.Content.ReadAsNonJsonAsync<string>().ConfigureAwait(false).GetAwaiter().GetResult());
			}
			if(InternalServerErrorCallback != null && InternalServerErrorCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for InternalServerErrorCallback are not supported. As they will run out of the scope of this call.");
			}
			if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
			{
				InternalServerErrorCallback?.Invoke();
			}
			if(ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported. As they will run out of the scope of this call.");
			}
			ResponseCallback?.Invoke(response);
			
			if(response.IsSuccessStatusCode)
			{
				return JsonUtil.Deserialize<IEnumerable<WeatherForecast>>(response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
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
			
			if(response == null)
			{
				response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				
			}

			return response;
		}


		public async ValueTask<IEnumerable<WeatherForecast>> WeatherForecastsAsync(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null, 
			CancellationToken cancellationToken = default(CancellationToken))
		{

			
			var controller = "SampleData";
			var action = "WeatherForecasts";

			string url = $@"api/{controller}/{action}";
			HttpResponseMessage response = null;
			
			if(response == null)
			{
				response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync(cancellationToken).ConfigureAwait(false);
				
			}

			if(BadRequestCallback != null && BadRequestCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for BadRequestCallback are not supported. As they will run out of the scope of this call.");
			}
			if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				BadRequestCallback?.Invoke(await response.Content.ReadAsNonJsonAsync<string>().ConfigureAwait(false));
			}
			if(InternalServerErrorCallback != null && InternalServerErrorCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for InternalServerErrorCallback are not supported. As they will run out of the scope of this call.");
			}
			if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
			{
				InternalServerErrorCallback?.Invoke();
			}
			if(ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported. As they will run out of the scope of this call.");
			}
			ResponseCallback?.Invoke(response);
			
			if(response.IsSuccessStatusCode)
			{
				return JsonUtil.Deserialize<IEnumerable<WeatherForecast>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
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
			
			if(response == null)
			{
				response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync(cancellationToken).ConfigureAwait(false);
				
			}

			return response;
		}

	}


}
