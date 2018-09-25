using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AspNetCore.Client.Generator.Framework.Http;
using AspNetCore.Client.Generator.Framework.SignalR;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.Framework
{
	/// <summary>
	/// Context used to keep track of generation details
	/// </summary>
	public class GenerationContext
	{

		/// <summary>
		/// Using statements that were in the files
		/// </summary>
		public IList<string> UsingStatements { get; set; } = new List<string>();

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
			if ((other.HttpClients?.Any() ?? false) || (other.HubClients?.Any() ?? false))
			{
				return new GenerationContext
				{
					HttpClients = this.HttpClients.Union(other.HttpClients).ToList(),
					HubClients = this.HubClients.Union(other.HubClients).ToList(),
					UsingStatements = this.UsingStatements.Union(other.UsingStatements).ToList()
				};
			}
			else
			{
				return this;
			}
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

		/// <summary>
		/// Validates the context for anything that might lead to a compile or runtime error
		/// </summary>
		public void Validate(string[] allowedNamespaces, string[] excludedNamespaces)
		{
			foreach (var client in HttpClients)
			{
				client.Validate();
			}

			foreach (var client in HubClients)
			{
				client.Validate();
			}

			Regex allowedUsings;
			Regex unallowedUsings;

			if (allowedNamespaces?.Any() ?? false)
			{
				allowedUsings = new Regex($"({string.Join("|", allowedNamespaces)})");
			}
			else
			{
				allowedUsings = new Regex($"(.+)");
			}

			if (excludedNamespaces?.Any() ?? false)
			{
				unallowedUsings = new Regex($"({string.Join("|", excludedNamespaces)})");
			}
			else
			{
				unallowedUsings = new Regex($"(^[.]+)");
			}

			this.UsingStatements = UsingStatements.Where(x => allowedUsings.IsMatch(x)
															&& !unallowedUsings.IsMatch(x))
														.ToList();
		}
	}
}
