using Beffyman.AspNetCore.Client.Serializers;

namespace Beffyman.AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the Beffyman.AspNetCore.Client.JSInterop library
	/// </summary>
	public static class JSInteropInstaller
	{
		/// <summary>
		/// Uses <see cref="JSInteropJsonSerializer"/> to serialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseJSInteropJsonSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<JSInteropJsonSerializer>();
		}

		/// <summary>
		/// Uses <see cref="JSInteropJsonSerializer"/> to deserialize requests when Json is detected
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static ClientConfiguration UseJSInteropJsonDeserializer(this ClientConfiguration config)
		{
			return config.UseDeserializer<JSInteropJsonSerializer>();
		}
	}
}
