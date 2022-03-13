using Microsoft.Extensions.DependencyInjection;
using System;
using TestWebApp.Clients;
using TestWebApp.Clients.FancySuffix;
using TestWebApp.Contracts;
using Beffyman.AspNetCore.Client;
using System.Net.Http;
using Flurl.Http;

namespace TestWebApp.Console
{
	public static class Program
	{

		public static void Main()
		{
			IServiceCollection services = new ServiceCollection();

			services.AddTestWebClients(config =>
			{
				config.UseJsonClientSerializer();
				config.UseJsonClientDeserializer();
				config.WithJsonBody();
				config.UseHttpClientFactory<ITestWebAppClient>();
				config.WithBaseAddress(_ => "http://localhost:62154");
				//config.WithBaseAddress("http://localhost:62154");
			});

			var provider = services.BuildServiceProvider();

			var client = provider.GetService<IValuesClient>();

			HttpResponseMessage msg = null;
			MyFancyDto val = client.MultiReturnParse(BadRequestCallback: () =>
			{
				throw new Exception("Client did not work!");
			},
			InternalServerErrorCallback: () =>
			{
				throw new Exception("Client did not work!");
			},
			CreatedCallback: dto =>
			 {

			 },
			ResponseCallback: response =>
			 {
				 msg = response;
			 });

			if (val.Id != 5)
			{
				throw new Exception("Client did not work!");
			}

			if (!msg.IsSuccessStatusCode)
			{
				throw new Exception("Client did not work!");
			}
		}

	}
}
