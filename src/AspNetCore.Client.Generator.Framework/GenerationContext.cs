using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AspNetCore.Client.Generator.Framework.Http;
using AspNetCore.Client.Generator.Framework.SignalR;

namespace AspNetCore.Client.Generator.Framework
{
	/// <summary>
	/// Context used to keep track of generation details
	/// </summary>
	public class GenerationContext
	{

		/// <summary>
		/// Clients that will be generated
		/// </summary>
		public IList<HttpController> HttpClients { get; set; } = new List<HttpController>();

		/// <summary>
		/// All of the endpoints inside the clients
		/// </summary>
		public IEnumerable<HttpEndpoint> HttpEndpoints => HttpClients.SelectMany(x => x.Endpoints);




		/// <summary>
		/// Clients that will be generated
		/// </summary>
		public IList<HubController> HubClients { get; set; } = new List<HubController>();

		/// <summary>
		/// All of the endpoints inside the clients
		/// </summary>
		public IEnumerable<HubEndpoint> HubEndpoints => HubClients.SelectMany(x => x.Endpoints);










		/// <summary>
		/// Merge this and another context into a new one
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public GenerationContext Merge(GenerationContext other)
		{
			return new GenerationContext
			{
				HttpClients = this.HttpClients.Union(other.HttpClients).ToList(),
				HubClients = this.HubClients.Union(other.HubClients).ToList()
			};
		}

		/// <summary>
		/// Maps the related information together, like base controllers
		/// </summary>
		public void MapRelatedInfo()
		{
			foreach (var client in HttpClients)
			{
				if (!string.IsNullOrEmpty(client.BaseClass))
				{
					client.BaseController = HttpClients.SingleOrDefault(x => x.Name == client.BaseClass);
				}
			}

			foreach (var client in HubClients)
			{
				if (!string.IsNullOrEmpty(client.BaseClass))
				{
					client.BaseController = HubClients.SingleOrDefault(x => x.Name == client.BaseClass);
				}
			}
		}
	}
}
