using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Core.AttributeInterfaces;
using AspNetCore.Client.Generator.Core.Headers;
using AspNetCore.Client.Generator.Core.Navigation;
using AspNetCore.Client.Generator.Core.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using AspNetCore.Client.Generator.Core.Routes;

namespace AspNetCore.Client.Generator.Core
{
	/// <summary>
	/// The information about an endpoint used for generation
	/// </summary>
	public class Endpoint : IResponseTypes, IHeaders, IIgnored, IObsolete, INavNode
	{
		/// <summary>
		/// Name of the endpoint/controller generated from
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// What HTTP method is required to hit this endpoint
		/// </summary>
		public HttpMethod HttpType { get; set; }

		/// <summary>
		/// Determines whether or not to have a void method depending on if it is a ActionResult return
		/// </summary>
		public string ReturnType { get; set; }

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
		/// Parent of this endpoint
		/// </summary>
		public Client Parent { get; set; }

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


		//IObsolete

		/// <summary>
		/// Whether or not the endpoint is obsolete
		/// </summary>
		public bool Obsolete { get; set; }

		/// <summary>
		/// Message
		/// </summary>
		public string ObsoleteMessage { get; set; }

		public Endpoint(Client parent)
		{
			Parent = parent;
		}

		public IEnumerable<INavNode> GetChildren()
		{
			return new List<INavNode>() { Parent }
				.Union(ResponseTypes)
				.Union(ConstantHeader)
				.Union(ParameterHeader);
		}
	}
}
