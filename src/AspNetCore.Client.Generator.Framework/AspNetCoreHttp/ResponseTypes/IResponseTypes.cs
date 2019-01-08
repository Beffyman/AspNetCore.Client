using System.Collections.Generic;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.ResponseTypes
{
	/// <summary>
	/// Detailing that this component contains response types that can be added
	/// </summary>
	public interface IResponseTypes
	{

		/// <summary>
		/// List of response types that can be added to the context
		/// </summary>
		IList<ResponseType> ResponseTypes { get; set; }

	}
}
