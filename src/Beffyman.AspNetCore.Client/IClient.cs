using System;
using Flurl.Http;

namespace Beffyman.AspNetCore.Client
{
	/// <summary>
	/// Client interface used by all clients generated, if you want to use reflection over all the clients, use this as the lookup
	/// </summary>
	public interface IClient { }

	/// <summary>
	/// Base interface for client wrapper
	/// </summary>
	public interface IClientWrapper
	{
		/// <summary>
		/// Base timeout to be used in requests
		/// </summary>
		TimeSpan Timeout { get; }
		/// <summary>
		/// Client to be used by requests
		/// </summary>
		IFlurlClient ClientWrapper { get; }
	}
}
