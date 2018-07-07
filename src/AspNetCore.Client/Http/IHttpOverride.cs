using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Client.Http
{
	public interface IHttpOverride
	{
		ValueTask<HttpResponseMessage> GetResponseAsync(String url, CancellationToken cancellationToken = default(CancellationToken));
		Task OnNonOverridedResponseAsync(String url, HttpResponseMessage response, CancellationToken cancellationToken = default(CancellationToken));
	}
}
