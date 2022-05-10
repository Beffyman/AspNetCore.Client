using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TestWebApp.Contracts;
using TestWebApp.Hubs;
using Xunit;

namespace TestWebApp.Tests
{
	public static class CancellationTokenHelper
	{
		public static Task WhenCanceled(this ref CancellationToken cancellationToken)
		{
			var tcs = new TaskCompletionSource<bool>();
			cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
			return tcs.Task;
		}
	}

	public class SignalRTests
	{
		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task SendReceiveMessageAsync()
		{
			using var endpoint = new JsonServerInfo();
			var hub = new ChatHubConnectionBuilder(endpoint.Server.BaseAddress, null,
				config =>
				{
					config.HttpMessageHandlerFactory = _ => endpoint.Server.CreateHandler();
				})
				.Build();

			string user = null;
			string message = null;

			using CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
			var token = tokenSource.Token;

			hub.OnReceiveMessage((usr, msg) =>
			{
				user = usr;
				message = msg;
				tokenSource.Cancel();
			});

			await hub.StartAsync(endpoint.TimeoutToken);

			await hub.SendMessageAsync("Test", "Hello World", endpoint.TimeoutToken);

			await token.WhenCanceled();

			await hub.StopAsync();

			Assert.Equal("Test", user);
			Assert.Equal("Hello World", message);
		}


		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task CounterChannelTest()
		{
			using var endpoint = new JsonServerInfo();
			var hub = new ChatHubConnectionBuilder(endpoint.Server.BaseAddress, null,
				config =>
				{
					config.HttpMessageHandlerFactory = _ => endpoint.Server.CreateHandler();
				})
				.Build();

			int count = 100;
			int delay = 20;

			IList<int> results = new List<int>();

			await hub.StartAsync(endpoint.TimeoutToken);

			var channel = await hub.StreamCounterAsync(count, delay, endpoint.TimeoutToken);

			while (await channel.WaitToReadAsync(endpoint.TimeoutToken))
			{
				while (channel.TryRead(out int item))
				{
					results.Add(item);
				}
			}

			await hub.StopAsync(endpoint.TimeoutToken);

			Assert.Equal(count, results.Count());
		}

		[Fact(Timeout = Constants.TEST_TIMEOUT)]
		public async Task CounterBlockingTest()
		{
			using var endpoint = new JsonServerInfo();
			var hub = new ChatHubConnectionBuilder(endpoint.Server.BaseAddress, null,
				config =>
				{
					config.HttpMessageHandlerFactory = _ => endpoint.Server.CreateHandler();
				})
				.Build();

			int count = 100;
			int delay = 20;

			await hub.StartAsync(endpoint.TimeoutToken);

			IEnumerable<int> results = await hub.ReadCounterBlockingAsync(count, delay, endpoint.TimeoutToken);

			await hub.StopAsync(endpoint.TimeoutToken);

			Assert.Equal(count, results.Count());
		}


		//[Fact(Timeout = Constants.TEST_TIMEOUT)]
		//public async Task MessagePackTest()
		//{
		//	using (var endpoint = new MessagePackServerInfo())
		//	{
		//		var hub = new ChatHubConnectionBuilder(endpoint.Server.BaseAddress, null,
		//			config =>
		//			{
		//				config.HttpMessageHandlerFactory = _ => endpoint.Server.CreateHandler();
		//			})
		//			.AddMessagePackProtocol()
		//			.Build();

		//		MyFancyDto actual = null;
		//		MyFancyDto expected = new MyFancyDto
		//		{
		//			Collision = Guid.NewGuid(),
		//			Description = "I am a test",
		//			Id = 15,
		//			When = DateTime.Now
		//		};

		//		using CancellationTokenSource tokenSource = new CancellationTokenSource(2000);
		//		var token = tokenSource.Token;

		//		hub.OnReceiveMessage2((dto) =>
		//		{
		//			actual = dto;
		//			tokenSource.Cancel();
		//		});

		//		await hub.StartAsync(endpoint.TimeoutToken);

		//		await hub.DtoMessageAsync(expected, endpoint.TimeoutToken);

		//		await token.WhenCanceled();

		//		await hub.StopAsync(endpoint.TimeoutToken);

		//		Assert.Equal(expected.Collision, actual.Collision);
		//		Assert.Equal(expected.Description, actual.Description);
		//		Assert.Equal(expected.Id, actual.Id);
		//		Assert.Equal(expected.When.ToLocalTime(), actual.When.ToLocalTime());
		//	}
		//}

	}
}
