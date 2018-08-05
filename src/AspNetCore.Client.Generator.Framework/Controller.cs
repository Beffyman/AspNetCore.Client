using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Framework.AttributeInterfaces;
using AspNetCore.Client.Generator.Framework.Dependencies;
using AspNetCore.Client.Generator.Framework.Headers;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.ResponseTypes;
using AspNetCore.Client.Generator.Framework.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCore.Client.Generator.Framework
{
	/// <summary>
	/// Information about a group of endpoints used for generation
	/// </summary>
	public class Controller : IResponseTypes, IHeaders, IIgnored, INamespaceSuffix, IObsolete, INavNode, IAuthorize
	{
		/// <summary>
		/// Name of the endpoint/controller generated from
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Name of the client generated
		/// </summary>
		public string ClientName => $@"{Name}Client";

		/// <summary>
		/// Version of the route, will group controllers with similar versions together
		/// </summary>
		public string NamespaceVersion { get; set; }

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

		//IAuthoize

		/// <summary>
		/// Should this endpoint require credentials
		/// </summary>
		public bool IsSecured { get; set; }

		public IEnumerable<INavNode> GetChildren()
		{
			return ResponseTypes.Cast<INavNode>()
				.Union(ConstantHeader)
				.Union(ParameterHeader)
				.Union(GetInjectionDependencies().OfType<INavNode>());
		}



		private static IEnumerable<Type> _allDependencies = typeof(IDependency).GetTypeInfo().Assembly
											.GetTypes()
											.Where(x => typeof(IDependency).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract)
											.ToList();
		public IEnumerable<IDependency> GetInjectionDependencies()
		{
			return _allDependencies.Where(x => x != typeof(ClientDependency)).Select(x => Activator.CreateInstance(x) as IDependency);
		}

		public override string ToString()
		{
			string namespaceVersion = $@"{(NamespaceVersion != null ? $"{NamespaceVersion}." : "")}{(NamespaceSuffix != null ? $"{NamespaceSuffix}." : string.Empty)}";

			return $"{namespaceVersion}{Name}";
		}
	}
}
