using AspNetCore.Client.Http;
using AspNetCore.Client.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client
{
	public class ClientConfiguration
	{
		public string HttpBaseAddress { get; set; }
		public Type SerializeType { get; set; } = typeof(JsonHttpSerializer);
		public Type HttpOverrideType { get; set; } = typeof(DefaultHttpOverride);

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

			return services;
		}
	}
}
