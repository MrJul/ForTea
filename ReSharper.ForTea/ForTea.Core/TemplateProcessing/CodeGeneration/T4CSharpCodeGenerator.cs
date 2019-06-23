using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Helpers;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>
	/// This class preprocesses T4 file
	/// to produce C# file that can be compiled and run correctly.
	/// </summary>
	internal sealed class T4CSharpCodeGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file) => Collector = new T4CSharpCodeGenerationInfoCollector(file, manager);

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		protected override T4CSharpCodeGenerationIntermediateResultsConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpCodeGenerationIntermediateResultsConverter(intermediateResult, File);
	}
}
