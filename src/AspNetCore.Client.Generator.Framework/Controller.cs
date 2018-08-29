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
		/// Whether the controller should be generated or not.
		/// </summary>
		public bool Abstract { get; set; }

		/// <summary>
		/// Name of the base class of the controller
		/// </summary>
		public string BaseClass { get; set; }


		/// <summary>
		/// The base class of the controller, may contain methods/attributes
		/// </summary>
		public Controller BaseController { get; set; }

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

		/// <summary>
		/// Whether to generate the client or not
		/// </summary>
		public bool Generated => !Ignored && !Abstract && !Failed;


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

		/// <summary>
		/// Whether of not the controller loaded correctly
		/// </summary>
		public bool Failed { get; set; }

		/// <summary>
		/// Unexpected error found
		/// </summary>
		public bool UnexpectedFailure { get; set; }

		/// <summary>
		/// Expected error found
		/// </summary>
		public string Error { get; set; }



		/// <summary>
		/// Gets all the children of this controller and any it inherits.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INavNode> GetChildren()
		{
			return ResponseTypes.Cast<INavNode>()
				.Union(BaseController?.GetChildren() ?? new List<INavNode>())
				.Union(ConstantHeader)
				.Union(ParameterHeader)
				.Union(GetInjectionDependencies().OfType<INavNode>())
				.Where(x => x != null)
				.ToList();
		}



		private static IEnumerable<Type> _allDependencies = typeof(IDependency).GetTypeInfo().Assembly
											.GetTypes()
											.Where(x => typeof(IDependency).IsAssignableFrom(x) && !x.GetTypeInfo().IsAbstract)
											.ToList();

		/// <summary>
		/// Gets all of the injectable properties for the clients generated
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDependency> GetInjectionDependencies()
		{
			if (Abstract)
			{
				return new List<IDependency>();
			}

			return _allDependencies.Where(x => x != typeof(ClientDependency)).Select(x => Activator.CreateInstance(x) as IDependency);
		}

		/// <summary>
		/// Gets all the endpoints under this controller that will filter based on overrides/virtual/new and inherit from base classes.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Endpoint> GetEndpoints()
		{
			var resolvedEndpoints = Endpoints.Union(BaseController?.GetEndpoints() ?? new List<Endpoint>())
											.ToList();

			var overwrittenEndpoints = resolvedEndpoints.GroupBy(x => x.Name)
														.Where(x => x.Count() > 1
																&& x.Any(y => y.Virtual)
																&& x.Any(y => y.Override))
														.ToList();

			//Any duplicates of name w/ a virtual and override
			if (overwrittenEndpoints.Any())
			{
				foreach (var group in overwrittenEndpoints)
				{
					resolvedEndpoints.RemoveAll(x => x.Name == group.Key && x.Virtual);
				}
			}

			var newEndpoints = resolvedEndpoints.GroupBy(x => x.Name)
												.Where(x => x.Any(y => y.New))
												.ToList();

			if (newEndpoints.Any())
			{
				foreach (var group in newEndpoints)
				{
					if (group.Count() == 1)
					{
						//do nothing, new with no inherited method...OK, warning
					}
					else if (group.Count() == 2)
					{
						resolvedEndpoints.RemoveAll(x => x.Name == group.Key && !x.New);
					}
					else if (group.Count() > 2)
					{
						var duplicateSignatures = group.GroupBy(x => x.GetSignature(this))
														.Where(x => x.Count() > 1 && x.Any(y => y.New))
														.ToList();

						foreach (var dupGroup in duplicateSignatures)
						{
							foreach (var ep in dupGroup.Skip(1))
							{
								resolvedEndpoints.Remove(ep);
							}
						}
					}
				}
			}



			return resolvedEndpoints.Where(x => !x.Ignored).ToList();
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string namespaceVersion = $@"{(NamespaceVersion != null ? $"{NamespaceVersion}." : "")}{(NamespaceSuffix != null ? $"{NamespaceSuffix}." : string.Empty)}";

			return $"{namespaceVersion}{Name}";
		}
	}
}
