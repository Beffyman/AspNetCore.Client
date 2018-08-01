using AspNetCore.Client.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.AttributeInterfaces
{
	/// <summary>
	/// Determines if the endpoint should not be generated because it has the <see cref="NoClientAttribute"/> on it
	/// </summary>
	public interface IIgnored
	{
		/// <summary>
		/// Should this endpoint be ignored because it has the <see cref="NoClientAttribute"/>
		/// </summary>
		bool Ignored { get; set; }
	}
}
