using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWebApp.Clients;

namespace TestWebApp.Tests
{
	/// <summary>
	/// Could implement redis caching, etc
	/// </summary>
	public class FakeHttpOverride : IHttpOverride
	{
		private IDictionary<string, HttpResponseMessage> memCachedResponses = new ConcurrentDictionary<string, HttpResponseMessage>();

		public async ValueTask<HttpResponseMessage> GetResponseAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (memCachedResponses.ContainsKey(url))
			{
				return await Task.FromResult(memCachedResponses[url]);
			}
			return null;
		}

		public async Task OnNonOverridedResponseAsync(string url, HttpResponseMessage response, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (memCachedResponses.ContainsKey(url))
			{
				memCachedResponses[url] = response;
			}
			else
			{
				memCachedResponses.Add(url, response);
			}
			await Task.CompletedTask;
		}
	}
}
