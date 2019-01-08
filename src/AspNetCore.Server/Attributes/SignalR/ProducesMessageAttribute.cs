using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Server.Attributes.SignalR
{
	/// <summary>
	/// Indicates that the method sends out a message with the types provided
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ProducesMessageAttribute : Attribute
	{
		/// <summary>
		/// Name of the message sent out
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Types to be used for the message sent out
		/// </summary>
		public Type[] Types { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="types"></param>
		public ProducesMessageAttribute(string name, params Type[] types)
		{
			Name = name;
			Types = types;
		}

	}
}
