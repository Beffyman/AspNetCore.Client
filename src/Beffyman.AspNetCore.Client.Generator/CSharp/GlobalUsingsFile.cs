using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Beffyman.AspNetCore.Client.Generator.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.CSharp
{
	internal class GlobalUsingsFile
	{
		public string Name { get; }
		public string FileName { get; }
		public SyntaxTree Syntax { get; set; }
		public CompilationUnitSyntax Root { get; }

		public GenerationContext Context { get; set; }

		public GlobalUsingsFile(string file)
		{
			Name = Path.GetFileNameWithoutExtension(file);
			FileName = file;

			var fileText = Helpers.SafelyReadFromFile(file);

			Syntax = CSharpSyntaxTree.ParseText(fileText, new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, SourceCodeKind.Regular));

			Context = new GenerationContext();
			Root = Syntax.GetRoot() as CompilationUnitSyntax;
			var usingStatements = Root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(x => (x.GlobalKeyword.Value as string) == "global").ToList();

			if (!usingStatements.Any())
			{
				return;
			}

			Regex allowedUsings;
			Regex unallowedUsings;

			if (Settings.AllowedNamespaces?.Any() ?? false)
			{
				allowedUsings = new Regex($"({string.Join("|", Settings.AllowedNamespaces)})");
			}
			else
			{
				allowedUsings = new Regex($"(.+)");
			}

			if (Settings.ExcludedNamespaces?.Any() ?? false)
			{
				unallowedUsings = new Regex($"({string.Join("|", Settings.ExcludedNamespaces)})");
			}
			else
			{
				unallowedUsings = new Regex($"(^[.]+)");
			}

			Context.UsingStatements = usingStatements.Select(x => x.WithoutLeadingTrivia().WithoutTrailingTrivia().ToFullString())
											.Where(x => allowedUsings.IsMatch(x)
													&& !unallowedUsings.IsMatch(x))
											.Select(x => x.Remove(0, 7))
											.ToList();

			if (Context.UsingStatements.Any())
			{
				Context.HasGlobalUsings = true;
			}
		}
	}
}
