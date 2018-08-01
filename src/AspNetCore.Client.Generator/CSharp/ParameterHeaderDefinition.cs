using AspNetCore.Client.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.CSharp
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
				throw new Exception($"{HeaderParameterAttribute.AttributeName} must have either 2 or 3 parameters.");
			}

			if (Type?.Contains("typeof") ?? false)
			{
				Type = Regex.Replace(Type, @"typeof\((.+)\)", "$1 ")?.Trim();
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
		}


		public string ParameterOutput()
		{
			if (string.IsNullOrEmpty(DefaultValue))
			{
				return $@"{Type} {Name}";
			}
			else
			{
				if (Type.Equals("string", StringComparison.CurrentCultureIgnoreCase))
				{
					return $@"{Type} {Name} = ""{DefaultValue}""";
				}
				else
				{
					return $@"{Type} {Name} = {DefaultValue}";
				}
			}
		}

		public string MethodOutput()
		{
			return GetMethodOutput($@"""{Name}""", Name);
		}

		public static string GetMethodOutput(string key, string value)
		{
			return $@".WithHeader({key}, {value})";
		}
	}
}
