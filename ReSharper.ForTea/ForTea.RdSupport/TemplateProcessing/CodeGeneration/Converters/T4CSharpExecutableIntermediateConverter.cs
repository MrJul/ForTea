using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpExecutableIntermediateConverter : T4CSharpIntermediateConverter
	{
		[NotNull] private const string SuffixResource =
			"JetBrains.ForTea.RdSupport.TemplateProcessing.CodeGeneration.TemplateBaseFullExecutableSuffix.cs";

		public T4CSharpExecutableIntermediateConverter(
			[NotNull] T4CSharpCodeGenerationIntermediateResult result,
			[NotNull] IT4File file
		) : base(result, file)
		{
		}

		protected override void AppendNamespaceContentSuffix(T4CSharpCodeGenerationResult result)
		{
			var provider = new T4TemplateResourceProvider(SuffixResource, this);
			string suffix = provider.ProcessResource(GeneratedBaseClassName, GeneratedClassName);
			result.Builder.Append(suffix);
		}
	}
}
