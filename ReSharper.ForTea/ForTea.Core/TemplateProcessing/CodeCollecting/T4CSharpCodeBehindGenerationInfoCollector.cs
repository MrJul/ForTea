using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public sealed class T4CSharpCodeBehindGenerationInfoCollector : T4CSharpCodeGenerationInfoCollectorBase
	{
		internal const string CodeCommentStart = "/*_T4\x200CCodeStart_*/";
		internal const string CodeCommentEnd = "/*_T4\x200CCodeEnd_*/";

		public T4CSharpCodeBehindGenerationInfoCollector(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file, manager)
		{
		}

		protected override void AddCommentStart(StringBuilder builder) =>
			builder.Append(CodeCommentStart);

		protected override void AddCommentEnd(StringBuilder builder) =>
			builder.Append(CodeCommentEnd);
	}
}
