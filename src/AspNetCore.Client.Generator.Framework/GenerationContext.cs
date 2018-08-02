using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
		public IList<Client> Clients { get; set; } = new List<Client>();

		/// <summary>
		/// All of the endpoints inside the clients
		/// </summary>
		public IEnumerable<Endpoint> Endpoints => Clients.SelectMany(x => x.Endpoints);
	}
}
