namespace Beffyman.AspNetCore.Client.Generator.Framework.AttributeInterfaces
{
	/// <summary>
	/// This value is populated by the Obsolete attribute
	/// </summary>
	public interface IObsolete
	{
		/// <summary>
		/// Whether or not the endpoint is obsolete
		/// </summary>
		bool Obsolete { get; set; }

		/// <summary>
		/// Message
		/// </summary>
		string ObsoleteMessage { get; set; }
	}
}
