using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Helpers
{
	public sealed class T4CSharpCodeBehindGenerationIntermediateResultsConverter :
		T4CSharpCodeGenerationIntermediateResultsConverterBase
	{
		[NotNull] public const string GeneratedClassNameString = "Generated\x200CTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = GeneratedClassNameString + "Base";

		public T4CSharpCodeBehindGenerationIntermediateResultsConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
		}

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseStub.cs";

		protected override string GeneratedClassName => GeneratedClassNameString;
		protected override string GeneratedBaseClassName => GeneratedBaseClassNameString;

		protected override void AppendSyntheticAttribute(StringBuilder builder) =>
			builder.AppendLine($"[{SyntheticAttribute.Name}]");

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			StringBuilder builder
		)
		{
			// There's no need to initialize parameters in code-behind since this code is never displayed anyway 
		}
	}
}
