using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestWebApp.Hubs;

namespace TestWebApp.Tests
{
	public static class CancellationTokenHelper
	{
		public static Task WhenCanceled(this CancellationToken cancellationToken)
		{
			var tcs = new TaskCompletionSource<bool>();
			cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
			return tcs.Task;
		}
	}

	[TestFixture]
	public class SignalRTests
	{

		[Test]
		public async Task SendReceiveMessageAsync()
		{
			var endpoint = new JsonServerInfo();

			var hub = new ChatHubConnectionBuilder(endpoint.Server.BaseAddress, null,
				config =>
				{
					config.HttpMessageHandlerFactory = _ => endpoint.Server.CreateHandler();
				})
				.Build();

			string user = null;
			string message = null;

			CancellationTokenSource tokenSource = new CancellationTokenSource(2000);
			var token = tokenSource.Token;

			hub.OnReceiveMessage((usr, msg) =>
			{
				user = usr;
				message = msg;
				tokenSource.Cancel();
			});

			await hub.StartAsync();

			await hub.SendMessageAsync("Test", "Hello World");

			await token.WhenCanceled();

			await hub.StopAsync();

			Assert.AreEqual("Test", user);
			Assert.AreEqual("Hello World", message);

		}


		[Test]
		public async Task CounterChannelTest()
		{
			var endpoint = new JsonServerInfo();

			var hub = new ChatHubConnectionBuilder(endpoint.Server.BaseAddress, null,
				config =>
				{
					config.HttpMessageHandlerFactory = _ => endpoint.Server.CreateHandler();
				})
				.Build();

			int count = 100;
			int delay = 20;

			IList<int> results = new List<int>();

			await hub.StartAsync();

			var channel = await hub.StreamCounterAsync(count, delay);

			while (await channel.WaitToReadAsync())
			{
				while (channel.TryRead(out int item))
				{
					results.Add(item);
				}
			}

			await hub.StopAsync();

			Assert.AreEqual(count, results.Count());

		}


		[Test]
		public async Task CounterBlockingTest()
		{
			var endpoint = new JsonServerInfo();

			var hub = new ChatHubConnectionBuilder(endpoint.Server.BaseAddress, null,
				config =>
				{
					config.HttpMessageHandlerFactory = _ => endpoint.Server.CreateHandler();
				})
				.Build();

			int count = 100;
			int delay = 20;

			await hub.StartAsync();

			IEnumerable<int> results = await hub.ReadCounterBlockingAsync(count, delay);

			await hub.StopAsync();

			Assert.AreEqual(count, results.Count());

		}


	}
}
