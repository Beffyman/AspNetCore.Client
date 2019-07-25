using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreFunctions
{
	public class HttpTriggerParameter
	{
		public IList<HttpMethod> Methods { get; }
		public AuthorizationLevel? AuthLevel { get; }
		public string Route { get; }

		public HttpTriggerParameter(ParameterSyntax syntax)
		{
			var triggerAttribute = syntax.AttributeLists.SelectMany(x => x.Attributes).GetAttribute<HttpTriggerAttribute>();

			var arguments = triggerAttribute.ArgumentList?.Arguments.Select(x => x.ToFullString().Trim()).ToList();
			if (!arguments.Any())
			{
				Methods = new List<HttpMethod>();
				return;
			}

			int parameterStart = 0;

			#region AuthLevel

			string authLevelString = null;

			var authLevelAssignment = arguments.SingleOrDefault(x => x.Contains(nameof(HttpTriggerAttribute.AuthLevel)));

			if (arguments.First().Contains(nameof(AuthorizationLevel)))
			{
				parameterStart = 1;
				var authLevel = arguments.First();
				authLevelString = authLevel.Substring(authLevel.IndexOf(nameof(AuthorizationLevel)) + nameof(AuthorizationLevel).Length + 1);
			}

			if (authLevelAssignment != null)
			{
				authLevelString = authLevelAssignment.Substring(authLevelAssignment.IndexOf(nameof(AuthorizationLevel)) + nameof(AuthorizationLevel).Length + 1);
			}

			if (authLevelString != null)
			{
				AuthLevel = (AuthorizationLevel)Enum.Parse(typeof(AuthorizationLevel), authLevelString);
			}
			else
			{
				AuthLevel = AuthorizationLevel.Anonymous;
			}

			#endregion

			var methods = arguments.Skip(parameterStart).Where(x => !x.Contains('='));
			var assignments = arguments.Skip(parameterStart).Where(x => x.Contains('='));

			var routeAssignment = assignments.SingleOrDefault(x => x.Contains(nameof(HttpTriggerAttribute.Route)));
			if (routeAssignment != null)
			{
				Route = string.Join(string.Empty, routeAssignment.Split('=').Skip(1)).TrimQuotes();
				if (Route == "null")
				{
					Route = null;
				}
			}

			Methods = new List<HttpMethod>();

			foreach (var method in methods)
			{
				if (method.Contains('"'))
				{
					var rawHttp = method.TrimQuotes().ToUpper();

					Methods.Add(new HttpMethod(rawHttp));
				}
				else
				{
					if (method?.Contains("nameof") ?? false)
					{
						var httpMethod = Regex.Replace(method, @"nameof\((.+)\)", "$1 ")?.Trim();
						httpMethod = httpMethod.Replace($"{nameof(HttpMethods)}.", string.Empty).ToUpper();
						Methods.Add(new HttpMethod(httpMethod));
					}
				}
			}

		}
	}
}
