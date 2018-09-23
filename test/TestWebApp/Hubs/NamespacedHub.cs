using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Client.Attributes.SignalR;
using System.Threading.Channels;
using AspNetCore.Client.Attributes;

namespace TestWebApp.Hubs
{
	[Route("Test")]
	[NamespaceSuffix("FancySuffix")]
	public class NamespacedHub : Hub
	{
		public NamespacedHub()
		{

		}

		[ProducesMessage("TestMessage", typeof(string))]
		public async Task TestMessage()
		{
			await Clients.All.SendAsync("TestMessage", "Hello");
		}
	}
}
