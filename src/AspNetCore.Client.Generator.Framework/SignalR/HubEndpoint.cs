using AspNetCore.Client.Attributes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using AspNetCore.Client.Generator.Framework.Http.Dependencies;
using System.Reflection;
using AspNetCore.Client.Attributes.Http;
using AspNetCore.Client.Generator.Framework.AttributeInterfaces;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.RequestModifiers;

namespace AspNetCore.Client.Generator.Framework.SignalR
{
	/// <summary>
	/// The information about an endpoint used for generation
	/// </summary>
	public class HubEndpoint : IIgnored, IObsolete, INavNode
	{
		/// <summary>
		/// Name of the endpoint/controller generated from
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// List of response types that can be added to the context
		/// </summary>
		public IList<Message> Messages { get; set; } = new List<Message>();

		/// <summary>
		/// Parameters that the endpoint has, can be placed in many different locations in a request
		/// </summary>
		public IList<IParameter> Parameters { get; set; } = new List<IParameter>();

		/// <summary>
		/// Parent of this endpoint
		/// </summary>
		public HubController Parent { get; set; }

		//IIgnored

		/// <summary>
		/// Should this endpoint be ignored because it has the <see cref="NotGeneratedAttribute" />
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


		/// <summary>
		/// Method has the virtual modifier
		/// </summary>
		public bool Virtual { get; set; }

		/// <summary>
		/// Method has the override modifier
		/// </summary>
		public bool Override { get; set; }

		/// <summary>
		/// Method has the new modifier
		/// </summary>
		public bool New { get; set; }

		/// <summary>
		/// Creates an endpoint assosicated with the controller
		/// </summary>
		/// <param name="parent"></param>
		public HubEndpoint(HubController parent)
		{
			Parent = parent;
		}

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INavNode> GetChildren()
		{
			return new List<INavNode>() { Parent }
				.Union(Parent.GetChildren())
				.Union(Parameters)
				.Where(x => x != null);
		}

		/// <summary>
		/// Gets all of the parameters for this endpoint that is sorted
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IParameter> GetParameters()
		{
			return GetChildren()
				.OfType<IParameter>()
				.Union(new List<IParameter> { new CancellationTokenModifier()})
				.OrderBy(x => x.SortOrder)
				.ThenBy(x => x.DefaultValue == null ? 0 : 1);
		}

		/// <summary>
		/// Returns a string that represents the current object under the context of the caller
		/// </summary>
		/// <param name="caller"></param>
		/// <returns></returns>
		public string ToString(HubController caller)
		{
			string namespaceVersion = $@"{(caller.NamespaceVersion != null ? $"{caller.NamespaceVersion}." : "")}{(caller.NamespaceSuffix != null ? $"{caller.NamespaceSuffix}." : string.Empty)}";

			return $"{namespaceVersion}{caller.Name}.{Name}";
		}

		/// <summary>
		/// Returns a string that represents the current object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(Parent);
		}

		/// <summary>
		/// Get the signature of the endpoint, for equality/grouping purposes
		/// </summary>
		/// <param name="caller"></param>
		/// <returns></returns>
		public string GetSignature(HubController caller)
		{
			return $"{ToString(caller)}(${string.Join(", ", GetParameters().Select(x => x.ToString()))}";
		}
	}
}
