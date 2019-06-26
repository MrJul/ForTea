using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Generators;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.RdSupport.TemplateProcessing.CodeCollecting;
using JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.Converters;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.Generators
{
	/// <summary>
	/// This class preprocesses T4 file
	/// to produce C# file that can be compiled and run correctly.
	/// </summary>
	public class T4CSharpCodeGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file) => Collector = new T4CSharpCodeGenerationInfoCollector(file, manager);

		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		protected override T4CSharpIntermediateConverterBase CreateConverter(
			T4CSharpCodeGenerationIntermediateResult intermediateResult
		) => new T4CSharpIntermediateConverter(intermediateResult, File);
	}
}
