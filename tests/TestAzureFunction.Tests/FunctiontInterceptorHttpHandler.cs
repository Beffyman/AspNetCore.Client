using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Net;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;

namespace TestAzureFunction.Tests
{
	public class FunctiontInterceptorHttpHandler : HttpMessageHandler
	{
		private readonly IServiceProvider _provider;

		public FunctiontInterceptorHttpHandler(IServiceProvider provider) : base()
		{
			_provider = provider;
		}


		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var httpContext = GetHttpContext();
			var builtRequest = await GetRequest(httpContext, request);

			var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

			var result = await TestAzureFunction.Function1.Run(builtRequest, _provider.GetService<ILogger>());
			try
			{
				await result.ExecuteResultAsync(actionContext);
			}
			catch (Exception ex)
			{
				throw;
			}

			return GetResponse(httpContext, request);
		}

		private HttpContext GetHttpContext()
		{
			var httpContext = new DefaultHttpContext();
			httpContext.RequestServices = _provider;
			httpContext.Response.Body = new MemoryStream();
			return httpContext;
		}

		private HttpResponseMessage GetResponse(HttpContext context, HttpRequestMessage request)
		{

			var response = new HttpResponseMessage();
			response.StatusCode = (HttpStatusCode)context.Response.StatusCode;
			response.RequestMessage = request;

			var data = ((MemoryStream)context.Response.Body).ToArray();
			response.Content = new ByteArrayContent(data);
			response.Content.Headers.ContentLength = data.Length;

			foreach (var header in context.Response.Headers)
			{
				if (!response.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value))
				{
					bool success = response.Content.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
				}
			}

			return response;
		}


		private async Task<HttpRequest> GetRequest(HttpContext context, HttpRequestMessage request)
		{
			var requestContent = request.Content ?? new StreamContent(Stream.Null);
			var body = await requestContent.ReadAsStreamAsync();

			PathString pathBase = new PathString("");

			var req = context.Request;

			req.Protocol = "HTTP/" + request.Version.ToString(fieldCount: 2);
			req.Method = request.Method.ToString();

			req.Scheme = request.RequestUri.Scheme;

			foreach (var header in request.Headers)
			{
				req.Headers.Append(header.Key, header.Value.ToArray());
			}

			if (req.Host == null || !req.Host.HasValue)
			{
				// If Host wasn't explicitly set as a header, let's infer it from the Uri
				req.Host = HostString.FromUriComponent(request.RequestUri);
				if (request.RequestUri.IsDefaultPort)
				{
					req.Host = new HostString(req.Host.Host);
				}
			}

			req.Path = PathString.FromUriComponent(request.RequestUri);
			req.PathBase = PathString.Empty;
			if (req.Path.StartsWithSegments(pathBase, out var remainder))
			{
				req.Path = remainder;
				req.PathBase = pathBase;
			}
			req.QueryString = QueryString.FromUriComponent(request.RequestUri);

			if (requestContent != null)
			{
				foreach (var header in requestContent.Headers)
				{
					req.Headers.Append(header.Key, header.Value.ToArray());
				}
			}

			if (body.CanSeek)
			{
				// This body may have been consumed before, rewind it.
				body.Seek(0, SeekOrigin.Begin);
			}
			req.Body = body;


			return req;
		}

	}
}
