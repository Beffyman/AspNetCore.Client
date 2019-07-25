using Beffyman.AspNetCore.Client.Serializers;

namespace Beffyman.AspNetCore.Client
{

	/// <summary>
	/// Static extension class for the AspnetCore.Client.Protobuf library
	/// </summary>
	public static class ProtobufInstaller
	{
		/// <summary>
		/// Uses <see cref="ProtobufSerializer"/> to serialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseProtobufSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<ProtobufSerializer>();
		}

		/// <summary>
		/// Uses <see cref="ProtobufSerializer"/> to deserialize requests when Protobuf content type is detected
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseProtobufDeserializer(this ClientConfiguration config)
		{
			return config.UseDeserializer<ProtobufSerializer>();
		}


		/// <summary>
		/// Adds an Accept of "application/x-protobuf" to every request
		/// </summary>
		/// <returns></returns>
		public static ClientConfiguration WithProtobufBody(this ClientConfiguration config)
		{
			return config.WithPredefinedHeader("Accept", ProtobufSerializer.CONTENT_TYPE);
		}
	}
}
