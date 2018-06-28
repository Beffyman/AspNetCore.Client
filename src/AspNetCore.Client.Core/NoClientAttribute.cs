using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Core
{
	/// <summary>
	/// Used when you don't want a client generated for the endpoint
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class NoClientAttribute : Attribute
	{
		public const string AttributeName = "NoClient";
	}
}
