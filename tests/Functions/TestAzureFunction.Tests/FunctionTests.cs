using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TestAzureFunction.Clients;
using System;

namespace TestAzureFunction.Tests
{
	public class FunctionTests
	{
		[Fact]
		public void TestJson_Get()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var client = endpoint.Provider.GetService<IFunction1Client>();

				string response = null;

				client.Function1_GET("Asp Client", Guid.NewGuid(), "Test",
				OKCallback: _ =>
				{
					response = _;
				});


				Assert.Equal("Hello, Asp Client", response);
			}
		}

		[Fact]
		public void TestJson_Post()
		{
			using (var endpoint = new JsonServerInfo())
			{
				var client = endpoint.Provider.GetService<IFunction1Client>();

				string response = null;

				client.Function1_POST(new Contracts.User
				{
					Name = "Asp Client"
				}, Guid.NewGuid(), "Test",
				OKCallback: _ =>
				{
					response = _;
				});


				Assert.Equal("Hello, Asp Client", response);
			}
		}


		[Fact]
		public void TestProtobuf_Get()
		{
			using (var endpoint = new ProtobufServerInfo())
			{
				var client = endpoint.Provider.GetService<IFunction1Client>();

				string response = null;

				client.Function1_GET("Asp Client", Guid.NewGuid(), "Test",
				OKCallback: _ =>
				{
					response = _;
				});


				Assert.Equal("Hello, Asp Client", response);
			}
		}

		[Fact]
		public void TestProtobuf_Post()
		{
			using (var endpoint = new ProtobufServerInfo())
			{
				var client = endpoint.Provider.GetService<IFunction1Client>();

				string response = null;

				client.Function1_POST(new Contracts.User
				{
					Name = "Asp Client"
				}, Guid.NewGuid(), "Test",
				OKCallback: _ =>
				{
					response = _;
				});


				Assert.Equal("Hello, Asp Client", response);
			}
		}

		[Fact]
		public void TestMessagePack_Get()
		{
			using (var endpoint = new MessagePackServerInfo())
			{
				var client = endpoint.Provider.GetService<IFunction1Client>();

				string response = null;

				client.Function1_GET("Asp Client", Guid.NewGuid(), "Test",
				OKCallback: _ =>
				{
					response = _;
				});


				Assert.Equal("Hello, Asp Client", response);
			}
		}

		[Fact]
		public void TestMessagePack_Post()
		{
			using (var endpoint = new MessagePackServerInfo())
			{
				var client = endpoint.Provider.GetService<IFunction1Client>();

				string response = null;

				client.Function1_POST(new Contracts.User
				{
					Name = "Asp Client"
				}, Guid.NewGuid(), "Test",
				OKCallback: _ =>
				{
					response = _;
				});

				Assert.Equal("Hello, Asp Client", response);
			}
		}
	}
}
