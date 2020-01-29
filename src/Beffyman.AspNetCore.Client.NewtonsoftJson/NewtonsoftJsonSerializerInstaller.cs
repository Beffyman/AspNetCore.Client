using Beffyman.AspNetCore.Client.Serializers;

namespace Beffyman.AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the Beffyman.AspNetCore.Client.NewtonsoftJson library
	/// </summary>
	public static class NewtonsoftJsonSerializerInstaller
	{
		/// <summary>
		/// Uses <see cref="NewtonsoftJsonHttpSerializer"/> to serialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseNewtonsoftJsonHttpSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<NewtonsoftJsonHttpSerializer>();
		}

		/// <summary>
		/// Uses <see cref="NewtonsoftJsonHttpSerializer"/> to deserialize requests when Json is detected
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static ClientConfiguration UseNewtonsoftJsonHttpDeserializer(this ClientConfiguration config)
		{
			return config.UseDeserializer<NewtonsoftJsonHttpSerializer>();
		}
	}
}
