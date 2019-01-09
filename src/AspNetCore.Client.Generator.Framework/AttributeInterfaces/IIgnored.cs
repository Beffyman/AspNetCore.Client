using AspNetCore.Server.Attributes.Http;

namespace AspNetCore.Client.Generator.Framework.AttributeInterfaces
{
	/// <summary>
	/// Determines if the endpoint should not be generated because it has the <see cref="NotGeneratedAttribute"/> on it
	/// </summary>
	public interface IIgnored
	{
		/// <summary>
		/// Should this endpoint be ignored because it has the <see cref="NotGeneratedAttribute"/>
		/// </summary>
		bool Ignored { get; set; }
	}
}
