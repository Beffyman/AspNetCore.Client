using Microsoft.Extensions.DependencyInjection;
using System;
using TestWebApp.Clients;
using TestWebApp.Clients.FancySuffix;
using TestWebApp.Contracts;
using Beffyman.AspNetCore.Client;

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

			MyFancyDto dto = null;

			client.FancyDtoReturn(50, OKCallback: _ =>
			  {
				  dto = _;
			  });

			if (dto == null)
			{
				throw new Exception("Client did not work!");
			}

		}

	}
}
