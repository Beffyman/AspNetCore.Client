using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client
{
	/// <summary>
	/// Settings to be passed down into the generated client wrapper
	/// </summary>
	public class ClientSettings
	{
		/// <summary>
		/// BaseAddress for the HttpClient
		/// </summary>
		public string BaseAddress { get; set; }

		/// <summary>
		/// Timeout for the HttpRequest
		/// </summary>
		public TimeSpan Timeout { get; set; }

	}
}
