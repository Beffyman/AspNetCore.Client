using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreHttp
{
	public class ApiVersionDefinition
	{
		public string Version { get; set; }


		public ApiVersionDefinition(AttributeSyntax attribute)
		{
			Version = attribute.ArgumentList.Arguments.SingleOrDefault().ToFullString().TrimQuotes();
		}

	}
}
