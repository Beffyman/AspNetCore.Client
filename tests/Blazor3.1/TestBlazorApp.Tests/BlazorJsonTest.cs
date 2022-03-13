using Xunit;
using System;
using System.Threading.Tasks;
using TestBlazorApp.Clients;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace TestBlazorApp.Tests
{
	public class BlazorJsonTest
	{
		[Fact]
		public async Task WeatherForecastsTest()
		{
			using (var endpoint = new BlazorJsonServerInfo())
			{
				var sampleDataClient = endpoint.Provider.GetService<IWeatherForecastClient>();
				var forecasts = await sampleDataClient.GetAsync();


				Assert.True(forecasts.Count() == 5);
			}
		}
	}
}
