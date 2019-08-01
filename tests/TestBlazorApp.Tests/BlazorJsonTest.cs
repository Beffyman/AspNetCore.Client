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
		public void WeatherForecastsTest()
		{
			using (var endpoint = new BlazorJsonServerInfo())
			{
				var sampleDataClient = endpoint.Provider.GetService<ISampleDataClient>();
				var forecasts = sampleDataClient.WeatherForecasts();


				Assert.True(forecasts.Count() == 5);
			}
		}
	}
}
