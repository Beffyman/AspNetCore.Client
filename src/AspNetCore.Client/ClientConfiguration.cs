using AspNetCore.Client.Http;
using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client
{
	public class ClientConfiguration
	{
		/// <summary>
		/// Base address to be used for a HttpClient being injected
		/// </summary>
		public string HttpBaseAddress { get; set; }
		/// <summary>
		/// What IHttpSerializer to use, defaults to json, allows for custom serialization of requests
		/// </summary>
		public Type SerializeType { get; set; } = typeof(JsonHttpSerializer);
		/// <summary>
		/// What IHttpOverride to use, allows for pre-post request calls
		/// </summary>
		public Type HttpOverrideType { get; set; } = typeof(DefaultHttpOverride);

		/// <summary>
		/// Headers that will always be included with every request
		/// </summary>
		public IDictionary<string, string> PredefinedHeaders { get; private set; } = new Dictionary<string, string>();

		/// <summary>
		/// Override the default timeout, which is 60
		/// </summary>
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

		public ClientConfiguration() { }

		public IServiceCollection ApplyConfiguration(IServiceCollection services)
		{
			if (SerializeType == null)
			{
				SerializeType = typeof(JsonHttpSerializer);
			}

			if (HttpOverrideType == null)
			{
				HttpOverrideType = typeof(DefaultHttpOverride);
			}

			services.AddScoped(typeof(IHttpSerializer), SerializeType);
			services.AddScoped(typeof(IHttpOverride), HttpOverrideType);

			services.AddScoped<IRequestModifier, RequestModifier>((provider) =>
			{
				var rm = new RequestModifier();
				rm.PredefinedHeaders = PredefinedHeaders;
				return rm;
			});

			return services;
		}


		public ClientConfiguration WithJsonBody()
		{
			PredefinedHeaders.Add("Accept", "application/json");

			return this;
		}
	}
}
