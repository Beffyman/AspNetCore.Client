using System.Collections.Generic;

namespace AspNetCore.Client.Generator.Framework.SignalR
{
	/// <summary>
	/// Represents a message that can be fired by a HubEndpoint
	/// </summary>
	public class Message
	{
		/// <summary>
		/// Name of the message
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Types the message contains
		/// </summary>
		public IEnumerable<string> Types { get; }

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="types"></param>
		public Message(string name, IEnumerable<string> types)
		{
			Name = name;
			Types = types;
		}
	}
}
