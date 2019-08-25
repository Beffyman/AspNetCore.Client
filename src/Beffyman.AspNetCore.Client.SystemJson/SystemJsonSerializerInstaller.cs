using Beffyman.AspNetCore.Client.Serializers;

namespace Beffyman.AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the Beffyman.AspNetCore.Client.SystemJson library
	/// </summary>
	public static class SystemJsonSerializerInstaller
	{
		/// <summary>
		/// Uses <see cref="SystemJsonSerializer"/> to serialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseSystemJsonSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<SystemJsonSerializer>();
		}

		/// <summary>
		/// Uses <see cref="SystemJsonSerializer"/> to deserialize requests when Json is detected
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static ClientConfiguration UseSystemJsonDeserializer(this ClientConfiguration config)
		{
			return config.UseDeserializer<SystemJsonSerializer>();
		}
	}
}
