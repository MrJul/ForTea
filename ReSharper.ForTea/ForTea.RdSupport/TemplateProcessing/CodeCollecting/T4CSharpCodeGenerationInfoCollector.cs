using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;

namespace JetBrains.ForTea.RdSupport.TemplateProcessing.CodeCollecting
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

		protected override void AppendToken(T4CSharpCodeGenerationIntermediateResult intermediateResult, IT4Token token)
		{
			string forAppending = Result.State.ConvertTokenForAppending(token);
			if (forAppending == null) return;
			
			var result = intermediateResult.FeatureStarted
				? intermediateResult.CollectedFeatures
				: intermediateResult.CollectedTransformation;
			result.Append("            this.Write(\"");
			result.Append(forAppending);
			result.AppendLine("\");");
		}
	}
}
