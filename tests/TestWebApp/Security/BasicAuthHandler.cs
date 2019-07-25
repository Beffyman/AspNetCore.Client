using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestWebApp.Security
{
	public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public BasicAuthHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock)
			: base(options, logger, encoder, clock)
		{

		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			await Task.CompletedTask;

			if (!Request.Headers.ContainsKey("Authorization"))
				return AuthenticateResult.Fail("Missing Authorization Header");

			var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
			var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
			var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
			var username = credentials[0];
			var password = credentials[1];

			if (username == "Tester" && password == "Test123")
			{
				var claims = new[] {
					new Claim(ClaimTypes.Name, username),
				};
				var identity = new ClaimsIdentity(claims, Scheme.Name);
				var principal = new ClaimsPrincipal(identity);
				var ticket = new AuthenticationTicket(principal, Scheme.Name);
				return AuthenticateResult.Success(ticket);
			}
			else
			{
				return AuthenticateResult.Fail("Invalid Username or Password");
			}
		}
	}
}
