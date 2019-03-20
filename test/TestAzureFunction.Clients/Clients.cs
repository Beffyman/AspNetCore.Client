//------------------------------------------------------------------------------
// <auto-generated>
//		This code was generated from a template.
//		Manual changes to this file may cause unexpected behavior in your application.
//		Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using AspNetCore.Client.Authorization;
using AspNetCore.Client.Exceptions;
using AspNetCore.Client.GeneratorExtensions;
using AspNetCore.Client.Http;
using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using AspNetCore.Client;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System;
using TestAzureFunction.Contracts;

namespace TestAzureFunction.Clients
{
	public static class TestAzureFunctionClientInstaller
	{
		/// <summary>
		/// Register the autogenerated clients into the container with a lifecycle of scoped.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configure">Overrides for client configuration</param>
		/// <returns></returns>
		public static IServiceCollection AddTestFunctionClients(this IServiceCollection services, Action<ClientConfiguration> configure)
		{
			var configuration = new ClientConfiguration();
			configuration.RegisterClientWrapperCreator<ITestAzureFunctionClient>(TestAzureFunctionClientWrapper.Create);
			configuration.UseClientWrapper<ITestAzureFunctionClientWrapper, TestAzureFunctionClientWrapper>((provider) => new TestAzureFunctionClientWrapper(provider.GetService<Func<ITestAzureFunctionClient, IFlurlClient>>(), configuration.GetSettings(), provider));
			configure?.Invoke(configuration);
			services.AddScoped<IFunction1Client, Function1Client>();
			return configuration.ApplyConfiguration<ITestAzureFunctionClient>(services);
		}
	}

	public interface ITestAzureFunctionClientWrapper : IClientWrapper
	{
	}

	public class TestAzureFunctionClientWrapper : ITestAzureFunctionClientWrapper
	{
		public TimeSpan Timeout
		{
			get;
			internal set;
		}

		public IFlurlClient ClientWrapper
		{
			get;
			internal set;
		}

		public TestAzureFunctionClientWrapper(Func<ITestAzureFunctionClient, IFlurlClient> client, ClientSettings settings, IServiceProvider provider)
		{
			ClientWrapper = client(null);
			if (settings.BaseAddress != null)
			{
				ClientWrapper.BaseUrl = settings.BaseAddress(provider);
			}

			Timeout = settings.Timeout;
		}

		public static ITestAzureFunctionClientWrapper Create(Func<ITestAzureFunctionClient, IFlurlClient> client, ClientSettings settings, IServiceProvider provider)
		{
			return new TestAzureFunctionClientWrapper(client, settings, provider);
		}
	}

	public interface ITestAzureFunctionClient : IClient
	{
	}
}

namespace TestAzureFunction.Clients
{
	public interface IFunction1Client : ITestAzureFunctionClient
	{
		void Function1_GET(string name, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
		HttpResponseMessage Function1Raw_GET(string name, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
		Task Function1_GETAsync(string name, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
		ValueTask<HttpResponseMessage> Function1Raw_GETAsync(string name, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
		void Function1_POST(User body, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
		HttpResponseMessage Function1Raw_POST(User body, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
		Task Function1_POSTAsync(User body, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
		ValueTask<HttpResponseMessage> Function1Raw_POSTAsync(User body, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
	}

	internal class Function1Client : IFunction1Client
	{
		protected readonly ITestAzureFunctionClientWrapper Client;
		protected readonly IHttpOverride HttpOverride;
		protected readonly IHttpSerializer Serializer;
		protected readonly IHttpRequestModifier Modifier;
		public Function1Client(ITestAzureFunctionClientWrapper param_client, Func<ITestAzureFunctionClient, IHttpOverride> param_httpoverride, Func<ITestAzureFunctionClient, IHttpSerializer> param_serializer, Func<ITestAzureFunctionClient, IHttpRequestModifier> param_modifier)
		{
			Client = param_client;
			HttpOverride = param_httpoverride(this);
			Serializer = param_serializer(this);
			Modifier = param_modifier(this);
		}

		public void Function1_GET(string name, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe?{nameof(name)}={name.EncodeForUrl()}";
			HttpResponseMessage response = null;
			response = HttpOverride.GetResponseAsync(HttpMethod.Get, url, null, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().GetAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return;
				}

				HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Get, url, null, response, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			}

			if (OKCallback != null && OKCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for OKCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				if (OKCallback != null)
				{
					responseHandled = true;
					OKCallback.Invoke(Serializer.Deserialize<string>(response.Content).ConfigureAwait(false).GetAwaiter().GetResult());
				}
			}

			if (BadRequestCallback != null && BadRequestCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for BadRequestCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				if (BadRequestCallback != null)
				{
					responseHandled = true;
					BadRequestCallback.Invoke(Serializer.Deserialize<string>(response.Content).ConfigureAwait(false).GetAwaiter().GetResult());
				}
			}

			if (ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported.As they will run out of the scope of this call.");
			}

			if (ResponseCallback != null)
			{
				responseHandled = true;
				ResponseCallback.Invoke(response);
			}

			if (UnauthorizedCallback != null && UnauthorizedCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for UnauthorizedCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				if (UnauthorizedCallback != null)
				{
					responseHandled = true;
					UnauthorizedCallback.Invoke(Serializer.Deserialize<HttpResponseMessage>(response.Content).ConfigureAwait(false).GetAwaiter().GetResult());
				}
			}

			if (!responseHandled)
			{
				throw new System.InvalidOperationException($"Response Status of {response.StatusCode} was not handled properly.");
			}

			return;
		}

		public HttpResponseMessage Function1Raw_GET(string name, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe?{nameof(name)}={name.EncodeForUrl()}";
			HttpResponseMessage response = null;
			response = HttpOverride.GetResponseAsync(HttpMethod.Get, url, null, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().GetAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return null;
				}

				HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Get, url, null, response, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			}

			return response;
		}

		public async Task Function1_GETAsync(string name, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe?{nameof(name)}={name.EncodeForUrl()}";
			HttpResponseMessage response = null;
			response = await HttpOverride.GetResponseAsync(HttpMethod.Get, url, null, cancellationToken).ConfigureAwait(false);
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = await Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().GetAsync(cancellationToken).ConfigureAwait(false);
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return;
				}

				await HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Get, url, null, response, cancellationToken).ConfigureAwait(false);
			}

			if (OKCallback != null && OKCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for OKCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				if (OKCallback != null)
				{
					responseHandled = true;
					OKCallback.Invoke(await Serializer.Deserialize<string>(response.Content).ConfigureAwait(false));
				}
			}

			if (BadRequestCallback != null && BadRequestCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for BadRequestCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				if (BadRequestCallback != null)
				{
					responseHandled = true;
					BadRequestCallback.Invoke(await Serializer.Deserialize<string>(response.Content).ConfigureAwait(false));
				}
			}

			if (ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported.As they will run out of the scope of this call.");
			}

			if (ResponseCallback != null)
			{
				responseHandled = true;
				ResponseCallback.Invoke(response);
			}

			if (UnauthorizedCallback != null && UnauthorizedCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for UnauthorizedCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				if (UnauthorizedCallback != null)
				{
					responseHandled = true;
					UnauthorizedCallback.Invoke(await Serializer.Deserialize<HttpResponseMessage>(response.Content).ConfigureAwait(false));
				}
			}

			if (!responseHandled)
			{
				throw new System.InvalidOperationException($"Response Status of {response.StatusCode} was not handled properly.");
			}

			return;
		}

		public async ValueTask<HttpResponseMessage> Function1Raw_GETAsync(string name, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe?{nameof(name)}={name.EncodeForUrl()}";
			HttpResponseMessage response = null;
			response = await HttpOverride.GetResponseAsync(HttpMethod.Get, url, null, cancellationToken).ConfigureAwait(false);
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = await Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().GetAsync(cancellationToken).ConfigureAwait(false);
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return null;
				}

				await HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Get, url, null, response, cancellationToken).ConfigureAwait(false);
			}

			return response;
		}

		public void Function1_POST(User body, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe";
			HttpResponseMessage response = null;
			response = HttpOverride.GetResponseAsync(HttpMethod.Post, url, null, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().PostAsync(Serializer.Serialize(body), cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return;
				}

				HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Post, url, body, response, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			}

			if (OKCallback != null && OKCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for OKCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				if (OKCallback != null)
				{
					responseHandled = true;
					OKCallback.Invoke(Serializer.Deserialize<string>(response.Content).ConfigureAwait(false).GetAwaiter().GetResult());
				}
			}

			if (BadRequestCallback != null && BadRequestCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for BadRequestCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				if (BadRequestCallback != null)
				{
					responseHandled = true;
					BadRequestCallback.Invoke(Serializer.Deserialize<string>(response.Content).ConfigureAwait(false).GetAwaiter().GetResult());
				}
			}

			if (ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported.As they will run out of the scope of this call.");
			}

			if (ResponseCallback != null)
			{
				responseHandled = true;
				ResponseCallback.Invoke(response);
			}

			if (UnauthorizedCallback != null && UnauthorizedCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for UnauthorizedCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				if (UnauthorizedCallback != null)
				{
					responseHandled = true;
					UnauthorizedCallback.Invoke(Serializer.Deserialize<HttpResponseMessage>(response.Content).ConfigureAwait(false).GetAwaiter().GetResult());
				}
			}

			if (!responseHandled)
			{
				throw new System.InvalidOperationException($"Response Status of {response.StatusCode} was not handled properly.");
			}

			return;
		}

		public HttpResponseMessage Function1Raw_POST(User body, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe";
			HttpResponseMessage response = null;
			response = HttpOverride.GetResponseAsync(HttpMethod.Post, url, null, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().PostAsync(Serializer.Serialize(body), cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return null;
				}

				HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Post, url, body, response, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
			}

			return response;
		}

		public async Task Function1_POSTAsync(User body, Guid ID, String AuthKey, Action<string> OKCallback = null, Action<string> BadRequestCallback = null, Action<HttpResponseMessage> ResponseCallback = null, Action<FlurlHttpException> ExceptionCallback = null, Action<HttpResponseMessage> UnauthorizedCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe";
			HttpResponseMessage response = null;
			response = await HttpOverride.GetResponseAsync(HttpMethod.Post, url, null, cancellationToken).ConfigureAwait(false);
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = await Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().PostAsync(Serializer.Serialize(body), cancellationToken).ConfigureAwait(false);
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return;
				}

				await HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Post, url, body, response, cancellationToken).ConfigureAwait(false);
			}

			if (OKCallback != null && OKCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for OKCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				if (OKCallback != null)
				{
					responseHandled = true;
					OKCallback.Invoke(await Serializer.Deserialize<string>(response.Content).ConfigureAwait(false));
				}
			}

			if (BadRequestCallback != null && BadRequestCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for BadRequestCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				if (BadRequestCallback != null)
				{
					responseHandled = true;
					BadRequestCallback.Invoke(await Serializer.Deserialize<string>(response.Content).ConfigureAwait(false));
				}
			}

			if (ResponseCallback != null && ResponseCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for ResponseCallback are not supported.As they will run out of the scope of this call.");
			}

			if (ResponseCallback != null)
			{
				responseHandled = true;
				ResponseCallback.Invoke(response);
			}

			if (UnauthorizedCallback != null && UnauthorizedCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{
				throw new NotSupportedException("Async void action delegates for UnauthorizedCallback are not supported.As they will run out of the scope of this call.");
			}

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
			{
				if (UnauthorizedCallback != null)
				{
					responseHandled = true;
					UnauthorizedCallback.Invoke(await Serializer.Deserialize<HttpResponseMessage>(response.Content).ConfigureAwait(false));
				}
			}

			if (!responseHandled)
			{
				throw new System.InvalidOperationException($"Response Status of {response.StatusCode} was not handled properly.");
			}

			return;
		}

		public async ValueTask<HttpResponseMessage> Function1Raw_POSTAsync(User body, Guid ID, String AuthKey, Action<FlurlHttpException> ExceptionCallback = null, IDictionary<String, Object> headers = null, IEnumerable<Cookie> cookies = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			string url = $@"/api/helloMe";
			HttpResponseMessage response = null;
			response = await HttpOverride.GetResponseAsync(HttpMethod.Post, url, null, cancellationToken).ConfigureAwait(false);
			bool responseHandled = response != null;
			if (response == null)
			{
				try
				{
					response = await Client.ClientWrapper.Request(url).WithCookies(cookies).WithHeaders(headers).WithTimeout(timeout ?? Client.Timeout).WithFunctionAuthorizationKey(AuthKey).WithHeader("ID", ID).AllowAnyHttpStatus().PostAsync(Serializer.Serialize(body), cancellationToken).ConfigureAwait(false);
				}
				catch (FlurlHttpException fhex)
				{
					if (ExceptionCallback != null && ExceptionCallback.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
					{
						throw new NotSupportedException("Async void action delegates for ExceptionCallback are not supported.As they will run out of the scope of this call.");
					}

					if (ExceptionCallback != null)
					{
						responseHandled = true;
						ExceptionCallback?.Invoke(fhex);
					}
					else
					{
						throw fhex;
					}

					return null;
				}

				await HttpOverride.OnNonOverridedResponseAsync(HttpMethod.Post, url, body, response, cancellationToken).ConfigureAwait(false);
			}

			return response;
		}
	}
}