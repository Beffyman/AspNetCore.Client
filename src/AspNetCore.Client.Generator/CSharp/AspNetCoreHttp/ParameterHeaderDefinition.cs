using System;
using System.Text.RegularExpressions;
using AspNetCore.Server.Attributes.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AspNetCore.Client.Generator.CSharp.AspNetCoreHttp
{
	public class ParameterHeaderDefinition
	{
		public string Name { get; }
		public string Type { get; }
		public string DefaultValue { get; }

		public int SortOrder
		{
			get
			{
				if (string.IsNullOrEmpty(DefaultValue))
				{
					return 0;
				}
				return 1;
			}
		}

		public ParameterHeaderDefinition(AttributeSyntax syntax)
		{
			if (syntax.ArgumentList.Arguments.Count == 2)
			{
				Name = syntax.ArgumentList.Arguments[0].ToFullString()?.Replace("\"", "").Trim();
				Type = syntax.ArgumentList.Arguments[1].ToFullString()?.Replace("\"", "").Trim();
			}
			else if (syntax.ArgumentList.Arguments.Count == 3)
			{
				Name = syntax.ArgumentList.Arguments[0].ToFullString()?.Replace("\"", "").Trim();
				Type = syntax.ArgumentList.Arguments[1].ToFullString()?.Replace("\"", "").Trim();
				DefaultValue = syntax.ArgumentList.Arguments[2].ToFullString()?.Replace("\"", "").Trim();
			}
			else
			{
				throw new Exception($"{nameof(HeaderParameterAttribute)} must have either 2 or 3 parameters.");
			}

			if (Type?.Contains("typeof") ?? false)
			{
				Type = Regex.Replace(Type, @"typeof\((.+)\)", "$1 ")?.Trim();
			}
			else if (Type?.Contains("nameof") ?? false)
			{
				Type = Regex.Replace(Type, @"nameof\((.+)\)", "$1 ")?.Trim();
			}

		}

		public ParameterHeaderDefinition(string name, string type, string defaultValue)
		{
			Name = name?.Replace("\"", "").Trim();
			Type = type?.Replace("\"", "").Trim();
			DefaultValue = defaultValue?.Replace("\"", "").Trim();


			if (Type?.Contains("typeof") ?? false)
			{
				Type = Regex.Replace(Type, @"typeof\((.+)\)", "$1 ");
			}
			else if (Type?.Contains("nameof") ?? false)
			{
				Type = Regex.Replace(Type, @"nameof\((.+)\)", "$1 ")?.Trim();
			}
		}
	}
}
