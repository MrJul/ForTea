using GammaJul.ForTea.Core.Psi;
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

		protected override string ToStringConversionStart => "this.ToStringHelper.ToStringWithCulture(";

		protected override void AppendCode(T4CSharpCodeGenerationResult result, IT4Token token) =>
			result.AppendMapped(token);
	}
}
