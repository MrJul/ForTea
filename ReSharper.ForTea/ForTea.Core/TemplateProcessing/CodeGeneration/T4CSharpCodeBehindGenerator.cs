using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Helpers;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>
	/// This class generates a code-behind file
	/// from C# embedded statements and directives in the T4 file.
	/// That file is used for providing code highlighting and other code insights
	/// in T4 source file.
	/// That code is not intended to be compiled and run.
	/// </summary>
	internal sealed class T4CSharpCodeBehindGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpCodeBehindGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file) => Collector = new T4CSharpCodeBehindGenerationInfoCollector(file, manager);

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		protected override T4CSharpCodeGenerationIntermediateResultsConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpCodeBehindGenerationIntermediateResultsConverter(intermediateResult, File);
	}
}
