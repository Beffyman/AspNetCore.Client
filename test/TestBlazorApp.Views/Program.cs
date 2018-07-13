using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TestBlazorApp.Views
{
	public class Program
	{
		public static void Main()
		{
			var serviceProvider = new BrowserServiceProvider(services =>
			{
				// Add any custom services here
			});

			new BrowserRenderer(serviceProvider).AddComponent<App>("app");
		}
	}
}
