using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace Beffyman.AspNetCore.Client.Http
{
	/// <summary>
	/// Overrides the request and allows for request interception before it even goes out
	/// </summary>
	public interface IHttpOverride
	{
		/// <summary>
		/// Intercepts the request before it goes out, if there is a non-null return from this method, it will be used instead of the request going out.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="url"></param>
		/// <param name="body"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<IFlurlResponse> GetResponseAsync(HttpMethod method, String url, object body, CancellationToken cancellationToken = default);

		/// <summary>
		/// Provides the response and the inputs used to achieve the result
		/// </summary>
		/// <param name="method"></param>
		/// <param name="url"></param>
		/// <param name="body"></param>
		/// <param name="response"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task OnNonOverridedResponseAsync(HttpMethod method, String url, object body, IFlurlResponse response, CancellationToken cancellationToken = default);
	}
}
