using AspNetCore.Client.Serializers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the AspnetCore.Client.BlazorJson library
	/// </summary>
	public static class BlazorJsonInstaller
	{
		/// <summary>
		/// Uses <see cref="BlazorSimpleJsonSerializer"/> to serialize and deserialize requests
		/// </summary>
		/// <param name="config"></param>
		public static void UseBlazorSimpleJsonSerlaizer(this ClientConfiguration config)
		{
			config.SerializeType = typeof(BlazorSimpleJsonSerializer);
		}
	}
}
