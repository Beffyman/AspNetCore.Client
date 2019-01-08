﻿using AspNetCore.Client.Serializers;

namespace AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the AspnetCore.Client.BlazorJson library
	/// </summary>
	public static class MessagePackInstaller
	{
		/// <summary>
		/// Uses <see cref="MessagePackSerializer"/> to serialize and deserialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseMessagePackSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<AspNetCore.Client.Serializers.MessagePackSerializer>();
		}

		/// <summary>
		/// Adds an Accept of "application/x-msgpack" to every request
		/// </summary>
		/// <returns></returns>
		public static ClientConfiguration WithMessagePackBody(this ClientConfiguration config)
		{
			return config.WithPredefinedHeader("Accept", "application/x-msgpack");
		}
	}
}
