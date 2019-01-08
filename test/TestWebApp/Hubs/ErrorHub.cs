using AspNetCore.Server.Attributes.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

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
