using Beffyman.AspNetCore.Client.Serializers;

namespace Beffyman.AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the AspnetCore.Client.BlazorJson library
	/// </summary>
	public static class MessagePackInstaller
	{
		/// <summary>
		/// Uses <see cref="MessagePackSerializer"/> to serialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseMessagePackSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<Beffyman.AspNetCore.Client.Serializers.MessagePackSerializer>();
		}

		/// <summary>
		/// Uses <see cref="MessagePackSerializer"/> to deserialize requests when MessagePack content-type is detected
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseMessagePackDeserializer(this ClientConfiguration config)
		{
			return config.UseDeserializer<Beffyman.AspNetCore.Client.Serializers.MessagePackSerializer>();
		}

		/// <summary>
		/// Adds an Accept of "application/x-msgpack" to every request
		/// </summary>
		/// <returns></returns>
		public static ClientConfiguration WithMessagePackBody(this ClientConfiguration config)
		{
			return config.WithPredefinedHeader("Accept", MessagePackSerializer.CONTENT_TYPE);
		}
	}
}
