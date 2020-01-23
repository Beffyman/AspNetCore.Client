using AspNetCore.Server.Attributes;
using AspNetCore.Server.Attributes.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TestWebApp.Hubs
{
	[Route("Test")]
	[NamespaceSuffix("FancySuffix")]
	[GenerateHub]
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
