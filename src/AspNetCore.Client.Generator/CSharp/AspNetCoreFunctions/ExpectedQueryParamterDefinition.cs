using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCore.Client.Generator.CSharp.AspNetCoreFunctions
{
	public class ExpectedQueryParamterDefinition
	{
		public HttpMethod Method { get; }

		public string Name { get; }

		public string Type { get; }

		public ExpectedQueryParamterDefinition(AttributeSyntax syntax)
		{
			var arguments = syntax.ArgumentList?.Arguments.Select(x => x.ToFullString().Trim()).ToList();

			var methodParameter = arguments.First();
			var nameParameter = arguments.Skip(1).First();
			var typeParameter = arguments.Skip(2).First();

			if (methodParameter.Contains('"'))
			{
				var rawHttp = methodParameter.TrimQuotes().ToUpper();

				Method = new HttpMethod(rawHttp);
			}
			else
			{
				if (methodParameter?.Contains("nameof") ?? false)
				{
					var httpMethod = Regex.Replace(methodParameter, @"nameof\((.+)\)", "$1 ")?.Trim();
					httpMethod = httpMethod.Replace($"{nameof(HttpMethods)}.", string.Empty).ToUpper();
					Method = new HttpMethod(httpMethod);
				}
			}

			if (nameParameter?.Contains("nameof") ?? false)
			{
				var name = Regex.Replace(nameParameter, @"nameof\((.+)\)", "$1 ")?.Trim();
				Name = name;
			}
			else
			{
				Name = nameParameter.TrimQuotes();
			}

			if (typeParameter?.Contains("typeof") ?? false)
			{
				Type = Regex.Replace(typeParameter, @"typeof\((.+)\)", "$1 ");
			}
			else
			{
				Type = typeParameter;
			}

		}
	}
}
