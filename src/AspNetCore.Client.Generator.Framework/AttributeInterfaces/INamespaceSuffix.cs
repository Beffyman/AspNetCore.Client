using AspNetCore.Server.Attributes;

namespace AspNetCore.Client.Generator.Framework.AttributeInterfaces
{
	/// <summary>
	/// This value is populated by the <see cref="NamespaceSuffixAttribute"/>
	/// </summary>
	public interface INamespaceSuffix
	{
		/// <summary>
		/// Suffix added onto the client's namespace
		/// </summary>
		string NamespaceSuffix { get; set; }
	}
}
