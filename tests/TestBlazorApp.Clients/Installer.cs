//------------------------------------------------------------------------------
// <auto-generated>
//		This code was generated from a template.
//		Manual changes to this file may cause unexpected behavior in your application.
//		Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//Requires nuget Beffyman.AspNetCore.Client
using Beffyman.AspNetCore.Client.Authorization;
using Beffyman.AspNetCore.Client.Exceptions;
using Beffyman.AspNetCore.Client.GeneratorExtensions;
using Beffyman.AspNetCore.Client.Http;
using Beffyman.AspNetCore.Client.RequestModifiers;
using Beffyman.AspNetCore.Client.Serializers;
using Beffyman.AspNetCore.Client;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System;
using TestBlazorApp.Shared;

namespace TestBlazorApp.Clients
{
	public static class TestBlazorAppClientInstaller
	{
		/// <summary>
		/// Register the autogenerated clients into the container with a lifecycle of scoped.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configure">Overrides for client configuration</param>
		/// <returns></returns>
		public static IServiceCollection AddTestBlazorClients(this IServiceCollection services, Action<ClientConfiguration> configure)
		{
			var configuration = new ClientConfiguration();
			configuration.RegisterClientWrapperCreator<ITestBlazorAppClient>(TestBlazorAppClientWrapper.Create);
			configuration.UseClientWrapper<ITestBlazorAppClientWrapper, TestBlazorAppClientWrapper>((provider) => new TestBlazorAppClientWrapper(provider.GetService<Func<ITestBlazorAppClient, IFlurlClient>>(), configuration.GetSettings(), provider));
			configure?.Invoke(configuration);
			services.AddScoped<IWeatherForecastClient, WeatherForecastClient>();
			return configuration.ApplyConfiguration<ITestBlazorAppClient>(services);
		}
	}

	public interface ITestBlazorAppClientWrapper : IClientWrapper
	{
	}

	public class TestBlazorAppClientWrapper : ITestBlazorAppClientWrapper
	{
		public TimeSpan Timeout
		{
			get;
			internal set;
		}

		public IFlurlClient ClientWrapper
		{
			get;
			internal set;
		}

		public TestBlazorAppClientWrapper(Func<ITestBlazorAppClient, IFlurlClient> client, ClientSettings settings, IServiceProvider provider)
		{
			ClientWrapper = client(null);
			if (settings.BaseAddress != null)
			{
				ClientWrapper.BaseUrl = settings.BaseAddress(provider);
			}

			Timeout = settings.Timeout;
		}

		public static ITestBlazorAppClientWrapper Create(Func<ITestBlazorAppClient, IFlurlClient> client, ClientSettings settings, IServiceProvider provider)
		{
			return new TestBlazorAppClientWrapper(client, settings, provider);
		}
	}

	public interface ITestBlazorAppClient : IClient
	{
	}
}