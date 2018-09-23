using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using AspNetCore.Client.Serializers;

namespace AspNetCore.Client.Generator.SignalR
{
	public class MessageDefinition
	{
		public string Name { get; }
		public IEnumerable<string> Types { get; }

		public MessageDefinition(AttributeSyntax attribute)
		{
			if (attribute.ArgumentList.Arguments.Count == 1)//Only HTTP value was provided, assumed to have no body
			{
				Name = attribute.ArgumentList.Arguments.SingleOrDefault().ToFullString().TrimStart('"').TrimEnd('"');
				Types = new List<string>();
			}
			else//Has 2 or more arguments, parse rest as types
			{
				Name = attribute.ArgumentList.Arguments.FirstOrDefault().ToFullString().TrimStart('"').TrimEnd('"');
				Types = attribute.ArgumentList.Arguments.Skip(1).Select(x => x.ToFullString().Replace("typeof", "").Trim().TrimStart('(').TrimEnd(')'));
			}
		}


		public override string ToString()
		{
			return $"{Name} {string.Join(", ", Types)}";
		}

	}
}
