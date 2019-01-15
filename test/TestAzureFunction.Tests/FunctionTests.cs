using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using TestAzureFunction.Clients;
using System;

namespace TestAzureFunction.Tests
{
	[TestFixture]
	public class FunctionTests
	{
		[Test]
		public void TestJson_Get()
		{
			var endpoint = new JsonServerInfo();

			var client = endpoint.Provider.GetService<IFunction1Client>();

			string response = null;

			client.Function1_GET("Asp Client", Guid.NewGuid(), "Test",
			OKCallback: _ =>
			{
				response = _;
			});


			Assert.AreEqual("Hello, Asp Client", response);
		}

		[Test]
		public void TestJson_Post()
		{
			var endpoint = new JsonServerInfo();

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


			Assert.AreEqual("Hello, Asp Client", response);
		}


		[Test]
		public void TestProtobuf_Get()
		{
			var endpoint = new ProtobufServerInfo();

			var client = endpoint.Provider.GetService<IFunction1Client>();

			string response = null;

			client.Function1_GET("Asp Client", Guid.NewGuid(), "Test",
			OKCallback: _ =>
			{
				response = _;
			});


			Assert.AreEqual("Hello, Asp Client", response);
		}

		[Test]
		public void TestProtobuf_Post()
		{
			var endpoint = new ProtobufServerInfo();

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


			Assert.AreEqual("Hello, Asp Client", response);
		}

		[Test]
		public void TestMessagePack_Get()
		{
			var endpoint = new MessagePackServerInfo();

			var client = endpoint.Provider.GetService<IFunction1Client>();

			string response = null;

			client.Function1_GET("Asp Client", Guid.NewGuid(), "Test",
			OKCallback: _ =>
			{
				response = _;
			});


			Assert.AreEqual("Hello, Asp Client", response);
		}

		[Test]
		public void TestMessagePack_Post()
		{
			var endpoint = new MessagePackServerInfo();

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


			Assert.AreEqual("Hello, Asp Client", response);
		}
	}
}
