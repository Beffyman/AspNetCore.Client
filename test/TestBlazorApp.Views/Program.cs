using Microsoft.AspNetCore.Blazor.Hosting;

namespace TestBlazorApp.Views
{
	public class Program
	{
		public static void Main()
		{
			BlazorWebAssemblyHost.CreateDefaultBuilder()
							.UseBlazorStartup<Startup>()
							.Build()
							.Run();
		}
	}
}
