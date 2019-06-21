using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>
	/// This class generates a code-behind file from C# embedded statements and directives in the T4 file.</summary>
	internal class T4CSharpCodeBehindGenerator : T4CSharpCodeGeneratorBase
	{
		public T4CSharpCodeBehindGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(
			file,
			manager
		)
		{
		}

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseStub.cs";

		protected override void AppendSyntheticAttribute(StringBuilder builder) =>
			builder.AppendLine($"[{SyntheticAttribute.Name}]");
	}
}
