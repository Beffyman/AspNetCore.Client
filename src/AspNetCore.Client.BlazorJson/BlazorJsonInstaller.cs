using AspNetCore.Client.Serializers;

namespace AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the AspnetCore.Client.BlazorJson library
	/// </summary>
	public static class BlazorJsonInstaller
	{
		/// <summary>
		/// Uses <see cref="BlazorSimpleJsonSerializer"/> to serialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseBlazorSimpleJsonSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<BlazorSimpleJsonSerializer>();
		}

		/// <summary>
		/// Uses <see cref="BlazorSimpleJsonSerializer"/> to deserialize requests when Json is detected
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static ClientConfiguration UseBlazorSimpleJsonDeserializer(this ClientConfiguration config)
		{
			return config.UseDeserializer<BlazorSimpleJsonSerializer>();
		}
	}
}
