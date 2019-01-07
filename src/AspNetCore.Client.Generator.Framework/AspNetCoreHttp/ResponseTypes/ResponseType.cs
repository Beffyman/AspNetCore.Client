using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Parameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.ResponseTypes
{
	/// <summary>
	/// Handles mapping ProducesResponseType to a suitable call
	/// </summary>
	public class ResponseType : IParameter
	{

		/// <summary>
		/// Display name of the parameter, based on the <see cref="Status"/>
		/// </summary>
		public virtual string Name
		{
			get
			{
				if (Status == HttpStatusCode.RedirectMethod)
				{
					return $"{nameof(HttpStatusCode.SeeOther)}Callback";
				}
				else
				{
					return $"{(Status == null ? "Response" : Status.ToString())}Callback";
				}
			}
		}

		/// <summary>
		/// All Response Types are optional parameters, null
		/// </summary>
		public virtual string DefaultValue => "null";

		/// <summary>
		/// Type of the action
		/// </summary>
		public virtual string Type => $"{nameof(Action)}{(ActionType == null ? "" : "<")}{ActionType}{(ActionType == null ? "" : ">")}";

		/// <summary>
		/// Type of the action parameter
		/// </summary>
		public virtual string ActionType { get; } = nameof(HttpResponseMessage);

		/// <summary>
		/// Order in which the parameter is inside the generated file
		/// </summary>
		public virtual int SortOrder => 6;

		/// <summary>
		/// What status does the action trigger on
		/// </summary>
		public virtual HttpStatusCode? Status { get; set; } = null;

		/// <summary>
		/// A Empty ResponseType means it is the generic handler of <see cref="Action"/>&lt;<see cref="HttpResponseMessage"/>&gt; ResponseCallback = null
		/// </summary>
		public ResponseType()
		{

		}

		/// <summary>
		/// A handler that has a parameter of type that is only hit during the provided status
		/// </summary>
		/// <param name="type"></param>
		/// <param name="status"></param>
		public ResponseType(string type, HttpStatusCode status)
		{
			Status = status;

			if (Status == 0)
			{
				Status = null;
			}
			else if (Status == HttpStatusCode.RedirectMethod)
			{
				Status = HttpStatusCode.SeeOther;
			}

			if (Status != 0 && Status != null)
			{
				ActionType = type;
			}
		}

		/// <summary>
		/// An action with no parameter that is only hit during the provided status
		/// </summary>
		/// <param name="status"></param>
		public ResponseType(HttpStatusCode status)
		{
			Status = status;

			if (Status == 0)
			{
				Status = null;
			}
			else if (Status == HttpStatusCode.RedirectMethod)
			{
				Status = HttpStatusCode.SeeOther;
			}
		}

		/// <summary>
		/// Retrieve all the <see cref="INavNode"/> implemented children of this node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}

		/// <summary>
		/// Returns a string that represents the current object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Type} {Name} = {DefaultValue}";
		}
	}
}
