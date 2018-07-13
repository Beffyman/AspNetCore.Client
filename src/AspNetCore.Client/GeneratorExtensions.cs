using AspNetCore.Client.Authorization;
using AspNetCore.Client.RequestModifiers;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client
{
	/// <summary>
	/// Extensions to be used inside the generated clients
	/// </summary>
	public static class GeneratorExtensions
	{
		/// <summary>
		/// Inserts the auth headers into the request based on the <see cref="SecurityHeader"/> implementation
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="security"></param>
		/// <returns></returns>
		public static T WithAuth<T>(this T clientOrRequest, SecurityHeader security) where T : IHttpSettingsContainer
		{
			if (security == null)
			{
				throw new ArgumentNullException($"{nameof(SecurityHeader)} provided is null");
			}

			return security.AddAuth(clientOrRequest);
		}

		/// <summary>
		/// Applies request modifiers
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="clientOrRequest"></param>
		/// <param name="requestModifier"></param>
		/// <returns></returns>
		public static T WithRequestModifiers<T>(this T clientOrRequest, IRequestModifier requestModifier) where T : IHttpSettingsContainer
		{
			return requestModifier.ApplyModifiers(clientOrRequest);
		}

	}
}
