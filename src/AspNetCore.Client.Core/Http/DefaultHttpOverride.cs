using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Client.Core.Http
{
	public class DefaultHttpOverride : IHttpOverride
	{
		public async ValueTask<HttpResponseMessage> GetResponseAsync(String url, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await Task.FromResult<HttpResponseMessage>(null);
		}

		public async Task OnNonOverridedResponseAsync(String url, HttpResponseMessage response, CancellationToken cancellationToken = default(CancellationToken))
		{
			await Task.CompletedTask;
		}
	}
}
