using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.CSharp.SignalR
{
	public class HubParameterDefinition
	{
		public string Name { get; }
		public string Type { get; }
		public string Default { get; }


		public HubParameterDefinition(ParameterSyntax parameter)
		{
			Name = parameter.Identifier.ValueText.Trim();
			Type = parameter.Type.ToFullString().Trim();
			Default = parameter.Default?.Value.ToFullString().Trim();
		}

		public override string ToString()
		{
			if (Default != null)
			{
				return $"{Type} {Name} = {Default}";
			}
			else
			{
				return $"{Type} {Name}";
			}
		}
	}
}
