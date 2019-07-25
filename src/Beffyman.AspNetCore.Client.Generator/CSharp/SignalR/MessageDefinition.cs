using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.SignalR
{
	public class MessageDefinition
	{
		public string Name { get; }
		public IEnumerable<string> Types { get; }

		public MessageDefinition(AttributeSyntax attribute)
		{
			if (attribute.ArgumentList.Arguments.Count == 1)//Only HTTP value was provided, assumed to have no body
			{
				Name = attribute.ArgumentList.Arguments.SingleOrDefault().ToFullString().TrimQuotes();
				Types = Enumerable.Empty<string>();
			}
			else//Has 2 or more arguments, parse rest as types
			{
				Name = attribute.ArgumentList.Arguments.FirstOrDefault().ToFullString().TrimQuotes();
				Types = attribute.ArgumentList.Arguments.Skip(1).Select(x => x.ToFullString().Replace("typeof", "").Trim().TrimStart('(').TrimEnd(')'));
			}
		}


		public override string ToString()
		{
			return $"{Name} {string.Join(", ", Types)}";
		}

	}
}
