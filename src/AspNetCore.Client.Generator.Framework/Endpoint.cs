using AspNetCore.Client.Attributes;
using AspNetCore.Client.Generator.Framework.AttributeInterfaces;
using AspNetCore.Client.Generator.Framework.Headers;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using AspNetCore.Client.Generator.Framework.Routes;
using AspNetCore.Client.Generator.Framework.Parameters;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Client.Generator.Framework.Routes.Constraints;
using AspNetCore.Client.Generator.Framework.Dependencies;
using System.Reflection;

namespace AspNetCore.Client.Generator.Framework
{
	/// <summary>
	/// The information about an endpoint used for generation
	/// </summary>
	public class Endpoint : IResponseTypes, IHeaders, IIgnored, IObsolete, INavNode, IAuthorize
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
		/// Parameters that the endpoint has, can be placed in many different locations in a request
		/// </summary>
		public IList<IParameter> Parameters { get; set; } = new List<IParameter>();

		/// <summary>
		/// Parent of this endpoint
		/// </summary>
		public Controller Parent { get; set; }

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


		//IAuthoize

		/// <summary>
		/// Should this endpoint require credentials
		/// </summary>
		public bool IsSecured { get; set; }


		public Endpoint(Controller parent)
		{
			Parent = parent;
		}

		public IEnumerable<INavNode> GetChildren()
		{
			return new List<INavNode>() { Parent }
				.Union(Parent.GetChildren())
				.Union(ResponseTypes)
				.Union(ConstantHeader)
				.Union(Parameters)
				.Union(ParameterHeader)
				.Where(x => x != null);
		}

		public IEnumerable<IParameter> GetParameters()
		{
			return GetChildren().OfType<IParameter>().OrderBy(x => x.SortOrder).ThenBy(x => x.DefaultValue == null ? 0 : 1);
		}

		public IEnumerable<IParameter> GetParametersWithoutResponseTypes()
		{
			return GetChildren().Where(x => !(x is ResponseType)).OfType<IParameter>().OrderBy(x => x.SortOrder).ThenBy(x => x.DefaultValue == null ? 0 : 1);
		}

		public IEnumerable<RouteParameter> GetRouteParameters()
		{
			return GetChildren().OfType<RouteParameter>().OrderBy(x => x.SortOrder);
		}

		public IEnumerable<QueryParameter> GetQueryParameters()
		{
			return GetChildren().OfType<QueryParameter>().OrderBy(x => x.SortOrder);
		}

		public BodyParameter GetBodyParameter()
		{
			return GetChildren().OfType<BodyParameter>().SingleOrDefault();
		}

		public IEnumerable<Header> GetHeaders()
		{
			return GetChildren().OfType<Header>();
		}

		public IEnumerable<ResponseType> GetResponseTypes()
		{
			return GetChildren().OfType<ResponseType>().OrderBy(x => x.SortOrder);
		}

		public IEnumerable<IRequestModifier> GetRequestModifiers()
		{
			return GetChildren().OfType<IRequestModifier>();
		}

		/// <summary>
		/// Full route template for the endpoint
		/// </summary>
		public string FullRoute => Parent.Route.Merge(Route).Value;

		public IEnumerable<RouteConstraint> GetRouteConstraints()
		{
			return Parent.Route.Merge(Route).Constraints;
		}


		public bool RequiresAuthorization()
		{
			return GetChildren().OfType<IAuthorize>().Any(x => x.IsSecured);
		}

		public override string ToString()
		{
			string namespaceVersion = $@"{(Parent.NamespaceVersion != null ? $"{Parent.NamespaceVersion}." : "")}{(Parent.NamespaceSuffix != null ? $"{Parent.NamespaceSuffix}." : string.Empty)}";

			return $"{namespaceVersion}{Parent.Name}.{Name}";
		}
	}
}
