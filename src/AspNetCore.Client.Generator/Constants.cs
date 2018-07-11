using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator
{
	public static class Constants
	{
		public const string Route = "Route";
		public const string Authorize = "Authorize";
		public const string Obsolete = "Obsolete";
		public const string Http = "Http";
		public const string Attribute = "Attribute";
		public const string AllowAnonymous = "AllowAnonymous";
		public const string ProducesResponseType = "ProducesResponseType";

		public const string ClientInterfaceName = "Client";
		public const string FlurlClientVariable = "ClientWrapper";

		public const string HttpOverride = "IHttpOverride";
		public const string HttpOverrideClass = "DefaultHttpOverride";
		public const string HttpOverrideField = "HttpOverride";
		public const string HttpOverrideGetMethod = "GetResponseAsync";
		public const string HttpOverrideOnNonOverridedResponse = "OnNonOverridedResponseAsync";


		public const string Serializer = nameof(IHttpSerializer);
		public const string SerializerField = "Serializer";

		public const string RequestModifier = nameof(IRequestModifier);
		public const string RequestModifierField = "Modifier";


		public const string IActionResult = "IActionResult";

		public const string ControllerRouteReserved = "controller";
		public const string ActionRouteReserved = "action";

		public const string CancellationTokenParameter = "cancellationToken";

		public const string ResponseVariable = "response";
		public const string UrlVariable = "url";
		public const string AuthParameter = "auth";

		public const string ResponseCallback = "ResponseCallback";
	}
}
