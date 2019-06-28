using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpCodeBehindIntermediateConverter :
		T4CSharpIntermediateConverterBase
	{
		[NotNull] public const string GeneratedClassNameString = "Generated\x200CTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = GeneratedClassNameString + "Base";

		public T4CSharpCodeBehindIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
		}

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseStub.cs";

		protected override string GeneratedClassName => GeneratedClassNameString;
		protected override string GeneratedBaseClassName => GeneratedBaseClassNameString;

		protected override void AppendSyntheticAttribute(T4CSharpCodeGenerationResult result) =>
			result.AppendLine($"        [{SyntheticAttribute.Name}]");

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			T4CSharpCodeGenerationResult result)
		{
			// There's no need to initialize parameters in code-behind since this code is never displayed anyway 
		}
	}
}