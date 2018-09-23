using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Client.Attributes.SignalR;
using System.Threading.Channels;

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


		public ChannelReader<int> Counter(int count, int delay)
		{
			var channel = Channel.CreateUnbounded<int>();

			// We don't want to await WriteItems, otherwise we'd end up waiting
			// for all the items to be written before returning the channel back to
			// the client.
			_ = WriteItems(channel.Writer, count, delay);

			return channel.Reader;
		}

		private async Task WriteItems(ChannelWriter<int> writer, int count, int delay)
		{
			for (var i = 0; i < count; i++)
			{
				await writer.WriteAsync(i);
				await Task.Delay(delay);
			}

			writer.TryComplete();
		}
	}
}
