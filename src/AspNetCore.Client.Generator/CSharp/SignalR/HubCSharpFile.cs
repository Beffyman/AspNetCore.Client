using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Output;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.Client.Generator.CSharp.SignalR
{
	public class HubCSharpFile
	{
		public string Name { get; }
		public string FileName { get; }
		public string FileText { get; }
		public SyntaxTree Syntax { get; set; }
		public CompilationUnitSyntax Root { get; }

		public List<string> UsingStatements { get; }

		public GenerationContext Context { get; set; }

		public HubCSharpFile(string file)
		{
			Name = Path.GetFileNameWithoutExtension(file);
			FileName = file;

			FileText = Helpers.SafelyReadFromFile(file);


			Syntax = CSharpSyntaxTree.ParseText(FileText, new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, SourceCodeKind.Regular));

			Context = new GenerationContext();
			Root = Syntax.GetRoot() as CompilationUnitSyntax;
			var usingStatements = Root.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

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

			UsingStatements = usingStatements.Select(x => x.WithoutLeadingTrivia().WithoutTrailingTrivia().ToFullString())
											.Where(x => allowedUsings.IsMatch(x)
													&& !unallowedUsings.IsMatch(x))
											.ToList();

			var namespaceDeclarations = Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

			foreach (var nsd in namespaceDeclarations)
			{
				var classDeclarations = nsd.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

				foreach (var cd in classDeclarations)
				{
					Context.HubClients.Add(ClassParser.ReadClassAsHubController(cd));
				}
			}

		}

	}
}
