using AspNetCore.Client.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client
{

	/// <summary>
	/// Static extension class for the AspnetCore.Client.Protobuf library
	/// </summary>
	public static class ProtobufInstaller
	{
		/// <summary>
		/// Uses <see cref="ProtobufSerializer"/> to serialize and deserialize requests
		/// </summary>
		/// <param name="config"></param>
		public static void UseProtobufSerlaizer(this ClientConfiguration config)
		{
			config.SerializeType = typeof(ProtobufSerializer);
		}


		/// <summary>
		/// Adds an Accept of "application/x-protobuf" to every request
		/// </summary>
		/// <returns></returns>
		public static ClientConfiguration WithProtobufBody(this ClientConfiguration config)
		{
			config.PredefinedHeaders.Add("Accept", "application/x-protobuf");

			return config;
		}
	}
}
