using Beffyman.AspNetCore.Client.Http;
using Beffyman.AspNetCore.Client.RequestModifiers;
using Beffyman.AspNetCore.Client.Serializers;

namespace Beffyman.AspNetCore.Client.Generator
{
	public static class Constants
	{
		public const string Http = "Http";
		public const string Attribute = "Attribute";

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

		public const string ApiVersionAttribute = "ApiVersionAttribute";
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

		public const string ResponseHandledVariable = "responseHandled";
	}
}
