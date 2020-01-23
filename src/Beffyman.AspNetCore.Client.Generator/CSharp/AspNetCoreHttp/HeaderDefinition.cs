using System;
using AspNetCore.Server.Attributes.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreHttp
{
	public class HeaderDefinition
	{
		public string Name { get; }
		public string Value { get; }

		public int SortOrder
		{
			get
			{
				if (string.IsNullOrEmpty(Value))
				{
					return 0;
				}
				return 1;
			}
		}

		public HeaderDefinition(AttributeSyntax syntax)
		{
			if (syntax.ArgumentList.Arguments.Count == 2)
			{
				Name = syntax.ArgumentList.Arguments[0].ToFullString()?.TrimQuotes();
				Value = syntax.ArgumentList.Arguments[1].ToFullString()?.TrimQuotes();
			}
			else
			{
				throw new Exception($"{nameof(HeaderParameterAttribute)} must have either 2 or 3 parameters.");
			}
		}

		public HeaderDefinition(string name, string value)
		{
			Name = name?.TrimQuotes();
			Value = value?.TrimQuotes();
		}
	}
}
