using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>This class generates a code-behind file from C# embedded statements and directives in the T4 file.</summary>
	internal sealed class T4CSharpCodeGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file)
		{
			GeneratedClassName = File.GetSourceFile()?.Name.WithExtension("хуй") ?? "хуй.хуй";
			GeneratedBaseClassName = GeneratedClassName + "Base";
			Collector = new T4CSharpCodeGenerationInfoCollector(file, manager);
		}

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseFull.cs";

		protected override void AppendSyntheticAttribute(StringBuilder builder)
		{
		}

		protected override string GeneratedClassName { get; }
		protected override string GeneratedBaseClassName { get; }
		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }
	}
}
