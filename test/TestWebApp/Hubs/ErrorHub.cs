using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Client.Attributes.SignalR;

namespace TestWebApp.Hubs
{
	[Route("Error")]
	public class ErrorHub : Hub
	{
		public ErrorHub()
		{

		}

		[ProducesMessage("ReceiveMessage", typeof(string), typeof(string))]
		public async Task SendMessage1(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

		[ProducesMessage("ReceiveMessage", typeof(int), typeof(int))]
		public async Task SendMessage2(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

	}
}
