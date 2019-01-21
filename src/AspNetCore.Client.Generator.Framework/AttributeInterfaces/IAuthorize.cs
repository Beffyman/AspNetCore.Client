namespace AspNetCore.Client.Generator.Framework.AttributeInterfaces
{
	/// <summary>
	/// Determines if the endpoint requires credentials because AuthorizeAttribute on it
	/// </summary>
	public interface IAuthorize
	{
		/// <summary>
		/// Should this endpoint require credentials
		/// </summary>
		bool IsSecured { get; set; }
	}
}
