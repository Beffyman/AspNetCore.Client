using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Server.Attributes.SignalR
{
	/// <summary>
	/// Indicates to the generator that this is a SignalR hub
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class GenerateHubAttribute : Attribute
	{
	}
}
