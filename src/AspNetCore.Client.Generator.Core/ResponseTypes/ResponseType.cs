using AspNetCore.Client.Generator.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AspNetCore.Client.Generator.Core.ResponseTypes
{
	/// <summary>
	/// Handles mapping ProducesResponseType to a suitable call
	/// </summary>
	public class ResponseType : IParameter
	{

		/// <summary>
		/// Display name of the parameter, based on the <see cref="Status"/>
		/// </summary>
		public string Name => $"{(Status == null ? "Response" : Status.ToString())}Callback";

		/// <summary>
		/// All Response Types are optional parameters, null
		/// </summary>
		public string DefaultValue => "null";

		/// <summary>
		/// Type of the action
		/// </summary>
		public string Type => $"{nameof(Action)}{(ActionType == null ? "" : "<")}{ActionType}{(ActionType == null ? "" : ">")}";

		/// <summary>
		/// Type of the action parameter
		/// </summary>
		public string ActionType { get; } = nameof(HttpResponseMessage);


		/// <summary>
		/// What status does the action trigger on
		/// </summary>
		public HttpStatusCode? Status { get; set; } = null;

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
			ActionType = type;
			Status = status;
		}

		/// <summary>
		/// An action with no parameter that is only hit during the provided status
		/// </summary>
		/// <param name="status"></param>
		public ResponseType(HttpStatusCode status)
		{
			ActionType = null;
			Status = status;
		}
	}
}
