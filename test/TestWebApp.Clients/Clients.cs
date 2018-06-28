//------------------------------------------------------------------------------
// <auto-generated>
//		This code was generated from a template.
//		Manual changes to this file may cause unexpected behavior in your application.
//		Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using Newtonsoft.Json;

namespace TestWebApp.Clients
{


	public static class TestWebAppClientInstaller
	{
		/// <summary>
		/// Register the autogenerated clients into the container with a lifecycle of scoped.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="baseAddress">Address to be used inside the HttpClient injected</param>
		/// <returns></returns>
		public static IServiceCollection InstallClients(this IServiceCollection services, string baseAddress = null)
		{
			services.AddScoped<TestWebAppClient>((provider) =>
			{
				var client = provider.GetService<HttpClient>();
				if(baseAddress != null)
				{
					client.BaseAddress = new Uri(baseAddress);
				}
				return new TestWebAppClient(client);
			});

			services.AddScoped<IValuesClient, ValuesClient>();
			return services;
		}
	}



	public class TestWebAppClient
	{
		public readonly FlurlClient ClientWrapper;

		public TestWebAppClient(HttpClient client)
		{
			ClientWrapper = new FlurlClient(client);
		}
	}

	public interface ITestWebAppClient : IClient { }



	public interface IValuesClient : ITestWebAppClient
	{
		
		IEnumerable<string> Get(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		HttpResponseMessage GetRaw();

		
		ValueTask<IEnumerable<string>> GetAsync(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		ValueTask<HttpResponseMessage> GetRawAsync();

		
		string Get(int id, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		HttpResponseMessage GetRaw(int id);

		
		ValueTask<string> GetAsync(int id, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		ValueTask<HttpResponseMessage> GetRawAsync(int id);

		
		void Post(string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		HttpResponseMessage PostRaw(string value, 
			int UserId);

		
		Task PostAsync(string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		ValueTask<HttpResponseMessage> PostRawAsync(string value, 
			int UserId);

		
		void Put(int id, 
			string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		HttpResponseMessage PutRaw(int id, 
			string value, 
			int UserId);

		
		Task PutAsync(int id, 
			string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		ValueTask<HttpResponseMessage> PutRawAsync(int id, 
			string value, 
			int UserId);

		
		void Delete(int id, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		HttpResponseMessage DeleteRaw(int id, 
			int UserId);

		
		Task DeleteAsync(int id, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null);

		
		ValueTask<HttpResponseMessage> DeleteRawAsync(int id, 
			int UserId);

	}


	public class ValuesClient : IValuesClient
	{
		public readonly TestWebAppClient Client;

		public ValuesClient(TestWebAppClient client)
		{
			Client = client;
		}


		public IEnumerable<string> Get(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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
				return JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
			}
			else
			{
				return null;
			}

		}


		public HttpResponseMessage GetRaw()
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult();

			return response;
		}


		public async ValueTask<IEnumerable<string>> GetAsync(Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false);

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
				return JsonConvert.DeserializeObject<IEnumerable<string>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
			}
			else
			{
				return null;
			}

		}


		public async ValueTask<HttpResponseMessage> GetRawAsync()
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false);

			return response;
		}


		public string Get(int id, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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
				return response.Content.ReadAsNonJsonAsync<string>().ConfigureAwait(false).GetAwaiter().GetResult();
			}
			else
			{
				return null;
			}

		}


		public HttpResponseMessage GetRaw(int id)
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult();

			return response;
		}


		public async ValueTask<string> GetAsync(int id, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false);

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
				return await response.Content.ReadAsNonJsonAsync<string>().ConfigureAwait(false);
			}
			else
			{
				return null;
			}

		}


		public async ValueTask<HttpResponseMessage> GetRawAsync(int id)
		{

			
			var controller = "Values";
			var action = "Get";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.AllowAnyHttpStatus()
				.GetAsync().ConfigureAwait(false);

			return response;
		}


		public void Post(string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Post";

			string url = $@"api/{controller}?value={value}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PostJsonAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();

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
			return;
		}


		public HttpResponseMessage PostRaw(string value, 
			int UserId)
		{

			
			var controller = "Values";
			var action = "Post";

			string url = $@"api/{controller}?value={value}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PostJsonAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();

			return response;
		}


		public async Task PostAsync(string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Post";

			string url = $@"api/{controller}?value={value}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PostJsonAsync(value).ConfigureAwait(false);

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
			return;
		}


		public async ValueTask<HttpResponseMessage> PostRawAsync(string value, 
			int UserId)
		{

			
			var controller = "Values";
			var action = "Post";

			string url = $@"api/{controller}?value={value}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PostJsonAsync(value).ConfigureAwait(false);

			return response;
		}


		public void Put(int id, 
			string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Put";

			string url = $@"api/{controller}/{id}?value={value}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PutJsonAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();

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
			return;
		}


		public HttpResponseMessage PutRaw(int id, 
			string value, 
			int UserId)
		{

			
			var controller = "Values";
			var action = "Put";

			string url = $@"api/{controller}/{id}?value={value}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PutJsonAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();

			return response;
		}


		public async Task PutAsync(int id, 
			string value, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Put";

			string url = $@"api/{controller}/{id}?value={value}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PutJsonAsync(value).ConfigureAwait(false);

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
			return;
		}


		public async ValueTask<HttpResponseMessage> PutRawAsync(int id, 
			string value, 
			int UserId)
		{

			
			var controller = "Values";
			var action = "Put";

			string url = $@"api/{controller}/{id}?value={value}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.PutJsonAsync(value).ConfigureAwait(false);

			return response;
		}


		public void Delete(int id, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Delete";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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
			return;
		}


		public HttpResponseMessage DeleteRaw(int id, 
			int UserId)
		{

			
			var controller = "Values";
			var action = "Delete";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();

			return response;
		}


		public async Task DeleteAsync(int id, 
			int UserId, 
			Action<string> BadRequestCallback = null, 
			Action InternalServerErrorCallback = null, 
			Action<HttpResponseMessage> ResponseCallback = null)
		{

			
			var controller = "Values";
			var action = "Delete";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.DeleteAsync().ConfigureAwait(false);

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
			return;
		}


		public async ValueTask<HttpResponseMessage> DeleteRawAsync(int id, 
			int UserId)
		{

			
			var controller = "Values";
			var action = "Delete";

			string url = $@"api/{controller}/{id}";
			HttpResponseMessage response = await Client.ClientWrapper
				.Request(url)
				.WithHeader("Accept", "application/json")
				.WithHeader("UserId", UserId)
				.AllowAnyHttpStatus()
				.DeleteAsync().ConfigureAwait(false);

			return response;
		}

	}


}