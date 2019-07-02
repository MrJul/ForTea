using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JetBrains.ForTea.RiderSupport.TemplateProcessing.Actions.RoslynCompilation
{
	public class T4RoslynCompilationManager
	{
		[NotNull] private const string DefaultExecutableExtension = "exe";
		[NotNull] private const string DefaultExecutableExtensionWithDot = "." + DefaultExecutableExtension;

		[NotNull]
		private string Code { get; }

		[NotNull]
		private ISolution Solution { get; }

		private Lifetime Lifetime { get; }

		public T4RoslynCompilationManager(
			Lifetime lifetime,
			[NotNull] string code,
			[NotNull] ISolution solution
		)
		{
			Lifetime = lifetime;
			Code = code;
			Solution = solution;
		}

		[NotNull]
		public IT4RoslynCompilationResult Compile(IEnumerable<MetadataReference> references)
		{
			var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
				.WithOptimizationLevel(OptimizationLevel.Debug)
				.WithMetadataImportOptions(MetadataImportOptions.Public);

			var syntaxTree = SyntaxFactory.ParseSyntaxTree(
				Code,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));

			// TODO: use actual name
			var compilation = CSharpCompilation.Create(
				"T4CompilationAssemblyName.exe",
				new[] {syntaxTree},
				options: options,
				references: references);

			// TODO: also generate pdb
			var executablePath = CreateTemporaryExecutable(Lifetime);

			var cancellationToken = Lifetime.ToCancellationToken();
			
			var errors = compilation
				.GetDiagnostics(cancellationToken)
				.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				.Select(diagnostic => diagnostic.ToString()).ToList();
			if (!errors.IsEmpty()) return new T4RoslynCompilationFailure(errors);

			compilation.Emit(executablePath.FullPath, cancellationToken: cancellationToken);
			return new T4RoslynCompilationSuccess(executablePath, Solution);
		}

		[NotNull]
		private FileSystemPath CreateTemporaryExecutable(Lifetime lifetime) =>
			FileSystemDefinition.CreateTemporaryFile(lifetime, extensionWithDot: DefaultExecutableExtensionWithDot);
	}
}
