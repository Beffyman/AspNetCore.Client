using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Attributes.Http
{
	/// <summary>
	/// Used when you don't want a client generated for the endpoint, can be placed on classes and methods
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class NotGeneratedAttribute : Attribute
	{
	}
}
