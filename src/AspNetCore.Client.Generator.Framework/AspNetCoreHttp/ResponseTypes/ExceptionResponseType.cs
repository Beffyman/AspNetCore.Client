using System;
using System.Net;
using Flurl.Http;

namespace AspNetCore.Client.Generator.Framework.AspNetCoreHttp.ResponseTypes
{
	/// <summary>
	/// Used for the callback for when the http call throws a FlurlHttpException meaning the request didn't get a proper response
	/// </summary>
	public class ExceptionResponseType : ResponseType
	{
		/// <summary>
		/// Display name of the parameter, based on the <see cref="Status"/>
		/// </summary>
		public override string Name
		{
			get
			{
				return $"ExceptionCallback";
			}
		}

		/// <summary>
		/// All Response Types are optional parameters, null
		/// </summary>
		public override string DefaultValue => "null";

		/// <summary>
		/// Type of the action
		/// </summary>
		public override string Type => $"{nameof(Action)}{(ActionType == null ? "" : "<")}{ActionType}{(ActionType == null ? "" : ">")}";

		/// <summary>
		/// Type of the action parameter
		/// </summary>
		public override string ActionType { get; } = nameof(FlurlHttpException);

		/// <summary>
		/// Order in which the parameter is inside the generated file
		/// </summary>
		public override int SortOrder => 6;

		/// <summary>
		/// What status does the action trigger on
		/// </summary>
		public override HttpStatusCode? Status { get; set; } = null;



	}
}
