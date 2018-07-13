using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Client.Http
{
	/// <summary>
	/// Default implementation for <see cref="IHttpOverride"/> which doesn't do anything
	/// </summary>
	public class DefaultHttpOverride : IHttpOverride
	{
		/// <summary>
		/// Empty implementation of <see cref="IHttpOverride.GetResponseAsync(HttpMethod, string, object, CancellationToken)"/>
		/// </summary>
		/// <param name="method"></param>
		/// <param name="url"></param>
		/// <param name="body"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async ValueTask<HttpResponseMessage> GetResponseAsync(HttpMethod method, String url, object body, CancellationToken cancellationToken = default)
		{
			return await Task.FromResult<HttpResponseMessage>(null).ConfigureAwait(false);
		}

		/// <summary>
		/// Empty implementation of <see cref="IHttpOverride.OnNonOverridedResponseAsync(HttpMethod, string, object, HttpResponseMessage, CancellationToken)"/>
		/// </summary>
		/// <param name="method"></param>
		/// <param name="url"></param>
		/// <param name="body"></param>
		/// <param name="response"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task OnNonOverridedResponseAsync(HttpMethod method, String url, object body, HttpResponseMessage response, CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask.ConfigureAwait(false);
		}
	}
}
