using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public sealed class T4CSharpCodeGenerationInfoCollector : T4CSharpCodeGenerationInfoCollectorBase
	{
		public T4CSharpCodeGenerationInfoCollector(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file, manager)
		{
		}

		protected override void AddCommentStart(StringBuilder builder)
		{
		}

		protected override void AddCommentEnd(StringBuilder builder)
		{
		}
	}
}
