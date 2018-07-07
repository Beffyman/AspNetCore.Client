using AspNetCore.Client.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client
{
	public static class CoreInstaller
	{
		public static void UseJsonClientSerializer(this ClientConfiguration config)
		{
			config.SerializeType = typeof(JsonHttpSerializer);
		}


	}
}
