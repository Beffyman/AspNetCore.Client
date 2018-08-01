using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.AttributeInterfaces
{
	/// <summary>
	/// This value is populated by the Http attributes or the Route attribute
	/// </summary>
	public interface IRoute
	{
		/// <summary>
		/// Route required to hit the endpoint
		/// </summary>
		string Route { get; set; }
	}
}
