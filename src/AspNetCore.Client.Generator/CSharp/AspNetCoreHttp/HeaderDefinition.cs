using AspNetCore.Client.Attributes.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.CSharp.AspNetCoreHttp
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
				Name = syntax.ArgumentList.Arguments[0].ToFullString()?.Replace("\"", "").Trim();
				Value = syntax.ArgumentList.Arguments[1].ToFullString()?.Replace("\"", "").Trim();
			}
			else
			{
				throw new Exception($"{nameof(HeaderParameterAttribute)} must have either 2 or 3 parameters.");
			}
		}

		public HeaderDefinition(string name, string value)
		{
			Name = name?.Replace("\"", "").Trim();
			Value = value?.Replace("\"", "").Trim();
		}
	}
}
