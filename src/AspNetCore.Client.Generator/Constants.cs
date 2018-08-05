using AspNetCore.Client.Http;
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
		public const string FromQuery = "FromQuery";
		public const string FromRoute = "FromRoute";
		public const string FromBody = "FromBody";

		public const string ClientInterfaceName = "Client";
		public const string FlurlClientVariable = "ClientWrapper";

		public const string HttpOverride = "IHttpOverride";
		public const string HttpOverrideClass = "DefaultHttpOverride";
		public const string HttpOverrideField = "HttpOverride";
		public const string HttpOverrideGetMethod = nameof(IHttpOverride.GetResponseAsync);
		public const string HttpOverrideOnNonOverridedResponse = nameof(IHttpOverride.OnNonOverridedResponseAsync);


		public const string Serializer = nameof(IHttpSerializer);
		public const string SerializerField = "Serializer";

		public const string RequestModifier = nameof(IHttpRequestModifier);
		public const string RequestModifierField = "Modifier";


		public const string IActionResult = "IActionResult";

		public const string ControllerRouteReserved = "controller";
		public const string ActionRouteReserved = "action";

		public const string TimeoutParameter = "timeout";
		public const string CancellationTokenParameter = "cancellationToken";

		public const string ResponseVariable = "response";
		public const string UrlVariable = "url";
		public const string AuthParameter = "auth";
		public const string CookiesParameter = "cookies";
		public const string HeadersParameter = "headers";

		public const string ResponseCallback = "ResponseCallback";
	}
}
