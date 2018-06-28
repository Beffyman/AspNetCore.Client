using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Data
{
	public class HeaderDefinition
	{
		public string Name { get; }
		public string Type { get; }
		public string DefaultValue { get; }

		public HeaderDefinition(AttributeSyntax syntax)
		{
			if (syntax.ArgumentList.Arguments.Count == 2)
			{
				Name = syntax.ArgumentList.Arguments[0].ToFullString();
				Type = syntax.ArgumentList.Arguments[1].ToFullString();
			}
			else if (syntax.ArgumentList.Arguments.Count == 3)
			{
				Name = syntax.ArgumentList.Arguments[0].ToFullString();
				Type = syntax.ArgumentList.Arguments[1].ToFullString();
				DefaultValue = syntax.ArgumentList.Arguments[2].ToFullString();
			}
			else
			{
				throw new Exception($"{AspNetCore.Client.Core.IncludesHeaderAttribute.AttributeName} must have either 2 or 3 parameters.");
			}
		}

		public HeaderDefinition(string name, string type, string defaultValue)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}


		public string ParameterOutput()
		{
			if (string.IsNullOrEmpty(DefaultValue))
			{
				return $@"{Type} {Name}";
			}
			else
			{
				return $@"{Type} {Name} = {DefaultValue}";
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
