using AspNetCore.Client.Serializers;

namespace AspNetCore.Client
{
	/// <summary>
	/// Static extension class for the AspNetCore.Client.RazorComponents library
	/// </summary>
	public static class RazorComponentsInstaller
	{
		/// <summary>
		/// Uses <see cref="RazorComponentsJsonSerializer"/> to serialize requests
		/// </summary>
		/// <param name="config"></param>
		public static ClientConfiguration UseRazorComponentsJsonSerializer(this ClientConfiguration config)
		{
			return config.UseSerializer<RazorComponentsJsonSerializer>();
		}

		/// <summary>
		/// Uses <see cref="RazorComponentsJsonSerializer"/> to deserialize requests when Json is detected
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static ClientConfiguration UseRazorComponentsJsonDeserializer(this ClientConfiguration config)
		{
			return config.UseDeserializer<RazorComponentsJsonSerializer>();
		}
	}
}
