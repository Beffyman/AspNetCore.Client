using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Headers;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Parameters;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.ResponseTypes;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Routes.Constraints;
using AspNetCore.Client.Generator.Framework.AttributeInterfaces;
using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Server.Attributes.Http;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Functions
{
	/// <summary>
	/// The information about an endpoint used for generation
	/// </summary>
	public class FunctionEndpoint : IResponseTypes, IHeaders, IIgnored, IObsolete, INavNode
	{
		/// <summary>
		/// Name of the endpoint/controller generated from
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Determines whether or not to have a void method depending on if it is a ActionResult return
		/// </summary>
		public string ReturnType { get; set; }

		/// <summary>
		/// Indicates that the endpoint returns a stream and to not deserialize it
		/// </summary>
		public bool ReturnsStream { get; set; }

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
		/// Parameters specific to a specific http method that is expected
		/// </summary>
		public IDictionary<HttpMethod, IList<IParameter>> HttpParameters { get; set; } = new Dictionary<HttpMethod, IList<IParameter>>();

		/// <summary>
		/// Supported HttpMethods listed out
		/// </summary>
		public IList<HttpMethod> SupportedMethods { get; set; } = new List<HttpMethod>();

		//IRoute

		/// <summary>
		/// Route required to hit the function, can be null
		/// </summary>
		public HttpRoute Route { get; set; }

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
		/// Creates an function
		/// </summary>
		public FunctionEndpoint()
		{
		}

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INavNode> GetChildren()
		{
			return new List<INavNode>() { }
				.Union(ResponseTypes)
				.Union(ConstantHeader)
				.Union(Parameters)
				.Union(ParameterHeader)
				.Where(x => x != null);
		}

		/// <summary>
		/// Gets all of the parameters for this function that is sorted
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IParameter> GetParameters()
		{
			return GetChildren().OfType<IParameter>().OrderBy(x => x.DefaultValue == null ? 0 : 1).ThenBy(x => x.SortOrder);
		}

		/// <summary>
		/// Gets all the parameters of the function that are not response types, used for creating a Raw request
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IParameter> GetParametersWithoutResponseTypes()
		{
			return GetChildren().Where(x => !(x is ResponseType) || (x is ExceptionResponseType)).OfType<IParameter>().OrderBy(x => x.DefaultValue == null ? 0 : 1).ThenBy(x => x.SortOrder);
		}

		/// <summary>
		/// Gets all of the parameters that exist inside the route
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RouteParameter> GetRouteParameters()
		{
			return GetChildren().OfType<RouteParameter>().OrderBy(x => x.SortOrder);
		}

		/// <summary>
		/// Gets all the parameters that exist as query string in the uri
		/// </summary>
		/// <returns></returns>
		public IEnumerable<QueryParameter> GetQueryParameters()
		{
			return GetChildren().OfType<QueryParameter>().OrderBy(x => x.SortOrder);
		}

		/// <summary>
		/// Gets the parameter that may exist inside the body of the request
		/// </summary>
		/// <returns></returns>
		public BodyParameter GetBodyParameter()
		{
			return GetChildren().OfType<BodyParameter>().SingleOrDefault();
		}

		/// <summary>
		/// Gets all headers that exist as children for the request
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Header> GetHeaders()
		{
			return GetChildren().OfType<Header>();
		}

		/// <summary>
		/// Gets all response types that are associated with the endpoint
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ResponseType> GetResponseTypes()
		{
			return GetChildren().OfType<ResponseType>().OrderBy(x => x.SortOrder);
		}

		/// <summary>
		/// Gets all request modifiers that affect this endpoint
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IRequestModifier> GetRequestModifiers()
		{
			return GetChildren().OfType<IRequestModifier>();
		}

		/// <summary>
		/// Full route template for the endpoint
		/// </summary>
		public string GetFullRoute()
		{
			return Route?.Value;
		}

		/// <summary>
		/// Get all route constraints associated with the endpoint, with the caller being accounted for
		/// </summary>
		/// <param name="caller"></param>
		/// <returns></returns>
		public IEnumerable<RouteConstraint> GetRouteConstraints(AspNetCoreHttpController caller)
		{
			return caller.Route.Merge(Route).Constraints;
		}

		/// <summary>
		/// Returns a string that represents the current object under the context of the caller
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}";
		}

		/// <summary>
		/// Get the signature of the endpoint, for equality/grouping purposes
		/// </summary>
		/// <returns></returns>
		public string GetSignature()
		{
			return $"{ToString()}(${string.Join(", ", GetParameters().Select(x => x.ToString()))}";
		}


		/// <summary>
		/// Validates the endpoint for anything that might lead to a compile or runtime error
		/// </summary>
		public void Validate()
		{
			var duplicateResponseTypes = this.GetResponseTypes().Where(x => x.Status != null).GroupBy(x => x.Status).Where(x => x.Count() > 1).ToList();

			if (duplicateResponseTypes.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple response types of the same status defined. {string.Join(", ", duplicateResponseTypes.Select(x => x.Key?.ToString()))}");
			}


			var duplicateParameters = this.GetParametersWithoutResponseTypes().GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();

			if (duplicateParameters.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple parameters of the same name defined. {string.Join(", ", duplicateParameters.Select(x => x.Key?.ToString()))}");
			}

			var duplicateHeaders = this.GetHeaders().GroupBy(x => x.Key).Where(x => x.Count() > 1).ToList();

			if (duplicateHeaders.Any())
			{
				throw new NotSupportedException($"Endpoint has multiple headers of the same key defined. {string.Join(", ", duplicateHeaders.Select(x => x.Key?.ToString()))}");
			}
		}
	}
}
