using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestBlazorApp.Clients;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace TestBlazorApp.Tests
{
	[TestFixture]
	public class BlazorJsonTest
	{
		[Test]
		public void WeatherForecastsTest()
		{
			var endpoint = new BlazorJsonServerInfo();

			var sampleDataClient = endpoint.Provider.GetService<ISampleDataClient>();
			var forecasts = sampleDataClient.WeatherForecasts();


			Assert.IsTrue(forecasts.Count() == 5);
		}
	}
}
