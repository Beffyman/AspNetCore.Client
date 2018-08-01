using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Core.AttributeInterfaces;
using AspNetCore.Client.Generator.Core.Headers;
using AspNetCore.Client.Generator.Core.Navigation;
using AspNetCore.Client.Generator.Core.ResponseTypes;
using AspNetCore.Client.Generator.Core.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCore.Client.Generator.Core
{
	/// <summary>
	/// Information about a group of endpoints used for generation
	/// </summary>
	public class Client : IResponseTypes, IHeaders, IIgnored, INamespaceSuffix, IObsolete, INavNode
	{
		/// <summary>
		/// Name of the endpoint/controller generated from
		/// </summary>
		public string Name { get; set; }



		/// <summary>
		/// List of response types that can be added to the context
		/// </summary>
		public IList<ResponseType> ResponseTypes { get; set; } = new List<ResponseType>();

		/// <summary>
		/// List of headers that can be added to the context that never change in value
		/// </summary>
		public IList<ConstantHeader> ConstantHeader { get; set; } = new List<ConstantHeader>();

		/// <summary>
		/// List of headers that can be added to the context that are used as a parameter
		/// </summary>
		public IList<ParameterHeader> ParameterHeader { get; set; } = new List<ParameterHeader>();

		/// <summary>
		/// List of endpoints that are under this client
		/// </summary>
		public IList<Endpoint> Endpoints { get; set; } = new List<Endpoint>();

		//IRoute

		/// <summary>
		/// Route required to hit the endpoint
		/// </summary>
		public Route Route { get; set; }

		//IIgnored

		/// <summary>
		/// Should this endpoint be ignored because it has the <see cref="NoClientAttribute" />
		/// </summary>
		public bool Ignored { get; set; }


		//INamespaceSuffix

		/// <summary>
		/// Suffix added onto the client's namespace
		/// </summary>
		public string NamespaceSuffix { get; set; }


		//IObsolete

		/// <summary>
		/// Whether or not the endpoint is obsolete
		/// </summary>
		public bool Obsolete { get; set; }

		/// <summary>
		/// Message
		/// </summary>
		public string ObsoleteMessage { get; set; }

		public IEnumerable<INavNode> GetChildren()
		{
			return ResponseTypes.Cast<INavNode>().Union(ConstantHeader).Union(ParameterHeader);
		}
	}
}
