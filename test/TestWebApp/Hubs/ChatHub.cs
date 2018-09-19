using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Client.Attributes.SignalR;

namespace TestWebApp.Hubs
{
	[Route("Chat")]
	public class ChatHub : Hub
	{
		public ChatHub()
		{

		}

		[ProducesMessage("ReceiveMessage", typeof(string), typeof(string))]
		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

	}
}
