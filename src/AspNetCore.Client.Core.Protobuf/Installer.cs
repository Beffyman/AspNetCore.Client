using AspNetCore.Client.Core.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core
{
	public static class ProtobufInstaller
	{

		public static void UseProtobufSerlaizer(this ClientConfiguration config)
		{
			config.SerializeType = typeof(ProtobufSerializer);
		}
	}
}
