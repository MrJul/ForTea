using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpInteractiveIntermediateConverter : T4CSharpIntermediateConverter
	{
		public T4CSharpInteractiveIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
		}

		// C# Interactive doesn't allow to declare namespaces
		protected override string GetNamespace() => null;

		protected override void AppendFileSuffix(T4CSharpCodeGenerationResult result) =>
			result.Builder.Append($"Console.WriteLine(new {GeneratedClassName}().TransformText());");
	}
}
