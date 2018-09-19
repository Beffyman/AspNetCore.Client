using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.SignalR
{
	public class Message
	{
		public string Name { get; }
		public IEnumerable<string> Types { get; }

		public Message(string name, IEnumerable<string> types)
		{
			Name = name;
			Types = types;
		}
	}
}
