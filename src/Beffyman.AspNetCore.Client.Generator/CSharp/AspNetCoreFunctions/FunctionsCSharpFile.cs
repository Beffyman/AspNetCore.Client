using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Beffyman.AspNetCore.Client.Generator.Framework;
using Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Functions;
using Beffyman.AspNetCore.Client.Generator.Output;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Beffyman.AspNetCore.Client.Generator.CSharp.AspNetCoreFunctions
{
	public class FunctionsCSharpFile
	{
		public string Name { get; }
		public string FileName { get; }
		public SyntaxTree Syntax { get; set; }
		public CompilationUnitSyntax Root { get; }

		public GenerationContext Context { get; set; }

		public FunctionsCSharpFile(string file, HostJson hostData)
		{
			Name = Path.GetFileNameWithoutExtension(file);
			FileName = file;

			var fileText = Helpers.SafelyReadFromFile(file);


			Syntax = CSharpSyntaxTree.ParseText(fileText, new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, SourceCodeKind.Regular));

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

			Context.UsingStatements = usingStatements.Select(x => x.WithoutLeadingTrivia().WithoutTrailingTrivia().ToFullString())
											.Where(x => allowedUsings.IsMatch(x)
													&& !unallowedUsings.IsMatch(x))
											.ToList();

			var namespaceDeclarations = Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

			foreach (var nsd in namespaceDeclarations)
			{
				var classDeclarations = nsd.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

				foreach (var cd in classDeclarations)
				{
					var methods = cd.DescendantNodes().OfType<MethodDeclarationSyntax>()
									.Where(x => x.Modifiers.Any(y => y.Text == "public") && x.Modifiers.Any(y => y.Text == "static"))
									.ToList();

					foreach (var method in methods)
					{
						Context.Functions.Add(ClassParser.ReadMethodAsFunction(method, hostData));
					}
				}
			}

		}
	}

}

