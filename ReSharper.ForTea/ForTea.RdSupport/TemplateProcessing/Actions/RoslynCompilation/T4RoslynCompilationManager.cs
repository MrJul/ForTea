using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.Actions.RoslynCompilation
{
	public class T4RoslynCompilationManager
	{
		[NotNull] private const string DefaultExecutableExtension = "exe";
		[NotNull] private const string DefaultExecutableExtensionWithDot = "." + DefaultExecutableExtension;

		[NotNull]
		private ISolution Solution { get; }

		[NotNull]
		private string Code { get; }

		[NotNull]
		private IPsiSourceFile PsiSourceFile { get; }

		private Lifetime Lifetime { get; }

		public T4RoslynCompilationManager(Lifetime lifetime,
			[NotNull] ISolution solution,
			[NotNull] string code,
			[NotNull] IPsiSourceFile psiSourceFile
		)
		{
			Solution = solution;
			Code = code;
			PsiSourceFile = psiSourceFile;
			Lifetime = lifetime;
		}

		// TODO: use progress indicator
		[NotNull]
		public IT4RoslynCompilationResult Compile()
		{
			var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
				.WithOptimizationLevel(OptimizationLevel.Debug)
				.WithMetadataImportOptions(MetadataImportOptions.Public);

			var syntaxTree = SyntaxFactory.ParseSyntaxTree(
				Code,
				CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));


			var compilation =
				CSharpCompilation.Create("T4CompilationAssemblyName.exe", // TODO: use actual name
					new[] {syntaxTree},
					options: options,
					references: ExtractReferences());

			// TODO: also generate pdb
			var executablePath = CreateTemporaryExecutable(Lifetime);
			var emitResult = compilation.Emit(executablePath.FullPath);
			if (emitResult.Success) return new T4RoslynCompilationSuccess(executablePath, Solution);
			return new T4RoslynCompilationFailure(emitResult
				.Diagnostics
				.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				.Select(diagnostic => diagnostic.ToString()).ToList());
		}

		[NotNull]
		private FileSystemPath CreateTemporaryExecutable(Lifetime lifetime) =>
			FileSystemDefinition.CreateTemporaryFile(lifetime, extensionWithDot: DefaultExecutableExtensionWithDot);

		private IEnumerable<MetadataReference> ExtractReferences() =>
			Solution
				.GetComponent<IPsiModules>()
				.GetModuleReferences(PsiSourceFile.PsiModule)
				.Select(it => it.Module)
				.OfType<IAssemblyPsiModule>()
				.Select(it => it.Assembly)
				.SelectNotNull(it => it.Location)
				.Select(it => MetadataReference.CreateFromFile(it.FullPath));
	}
}
