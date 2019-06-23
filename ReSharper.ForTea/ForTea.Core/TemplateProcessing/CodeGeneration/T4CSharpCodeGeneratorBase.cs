using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Helpers;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	internal abstract class T4CSharpCodeGeneratorBase
	{
		[NotNull]
		protected IT4File File { get; }

		/// <summary>Initializes a new instance of the <see cref="T4CSharpCodeGeneratorBase"/> class.</summary>
		/// <param name="file">The associated T4 file whose C# code behind will be generated.</param>
		protected T4CSharpCodeGeneratorBase([NotNull] IT4File file) => File = file;

		[NotNull]
		public T4CSharpCodeGenerationResult Generate() => CreateConverter(Collector.Collect()).Convert();

		[NotNull]
		protected abstract T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		[NotNull]
		protected abstract T4CSharpCodeGenerationIntermediateResultsConverterBase CreateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult);
	}
}
