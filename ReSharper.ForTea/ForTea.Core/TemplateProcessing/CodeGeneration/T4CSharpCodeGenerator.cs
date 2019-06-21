using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>This class generates a code-behind file from C# embedded statements and directives in the T4 file.</summary>
	internal class T4CSharpCodeGenerator : T4CSharpCodeGeneratorBase
	{
		[NotNull]
		private T4TemplateBaseProvider Provider { get; }

		public T4CSharpCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager,
			[NotNull] T4TemplateBaseProvider provider
		) : base(
			file,
			manager
		) => Provider = provider;

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseFull.cs";

		protected override void AppendSyntheticAttribute(StringBuilder builder)
		{
		}
	}
}
