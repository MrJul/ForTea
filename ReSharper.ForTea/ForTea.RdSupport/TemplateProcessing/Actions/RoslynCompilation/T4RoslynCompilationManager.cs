using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
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
		private string Code { get; }

		[NotNull]
		public T4PsiFileInfo Info { get; }

		private Lifetime Lifetime { get; }

		public T4RoslynCompilationManager(Lifetime lifetime,
			[NotNull] string code,
			[NotNull] T4PsiFileInfo info
		)
		{
			Code = code;
			Info = info;
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

			var errors = compilation
				.GetDiagnostics()
				.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
				.Select(diagnostic => diagnostic.ToString()).ToList();
			if (!errors.IsEmpty()) return new T4RoslynCompilationFailure(errors);

			compilation.Emit(executablePath.FullPath);
			return new T4RoslynCompilationSuccess(executablePath, Info.Solution);
		}

		[NotNull]
		private FileSystemPath CreateTemporaryExecutable(Lifetime lifetime) =>
			FileSystemDefinition.CreateTemporaryFile(lifetime, extensionWithDot: DefaultExecutableExtensionWithDot);

		private IEnumerable<MetadataReference> ExtractReferences()
		{
			var psiModule = Info.PsiSourceFile.PsiModule;
			using (CompilationContextCookie.GetOrCreate(psiModule.GetResolveContextEx(Info.ProjectFile)))
			{
				return Info
					.Solution
					.GetComponent<IPsiModules>()
					.GetModuleReferences(psiModule)
					.Select(it => it.Module)
					.OfType<IAssemblyPsiModule>()
					.Select(it => it.Assembly)
					.SelectNotNull(it => it.Location)
					.Select(it => MetadataReference.CreateFromFile(it.FullPath));
			}
		}
	}
}
