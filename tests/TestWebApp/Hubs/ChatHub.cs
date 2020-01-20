using AspNetCore.Server.Attributes.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TestWebApp.Contracts;

namespace TestWebApp.Hubs
{
	[Route("Chat")]
	[GenerateHub]
	public class ChatHub : Hub
	{
		public ChatHub()
		{

		}

		[ProducesMessage("ReceiveMessage", typeof(string), typeof(string))]
		public async Task SendMessage(string user, string message, CancellationToken token = default)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message, token);
		}

		[ProducesMessage("ReceiveMessage2", typeof(MyFancyDto))]
		public async Task DtoMessage(MyFancyDto dto)
		{
			await Clients.All.SendAsync("ReceiveMessage2", dto);
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
