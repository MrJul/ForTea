using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpExecutableIntermediateConverter : T4CSharpIntermediateConverter
	{
		[NotNull] private const string SuffixResource =
			"GammaJul.ForTea.Core.Resources.TemplateBaseFullExecutableSuffix.cs";

		public T4CSharpExecutableIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
		}

		// Avoid name clash
		protected override string GeneratedClassName =>
			"__" + base.GeneratedClassName + "__" + DefaultGeneratedClassName + "__";

		// When creating executable, it is better to put base class first,
		// to make error messages more informative
		protected override void AppendClasses(T4CSharpCodeGenerationResult result)
		{
			AppendBaseClass(result);
			AppendMainContainer(result);
			AppendClass(result);
		}

		private void AppendMainContainer(T4CSharpCodeGenerationResult result)
		{
			var provider = new T4TemplateResourceProvider(SuffixResource, this);
			string suffix = provider.ProcessResource(GeneratedBaseClassName, GeneratedClassName);
			result.Append(suffix);
		}
	}
}
