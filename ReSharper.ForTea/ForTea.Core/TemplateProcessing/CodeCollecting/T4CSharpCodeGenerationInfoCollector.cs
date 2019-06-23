using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Util;

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

		protected override void AppendToken(T4CSharpCodeGenerationIntermediateResult intermediateResult, IT4Token token)
		{
			if (token.NodeType == T4TokenNodeTypes.NewLine && intermediateResult.FeatureStarted) return;
			var result = intermediateResult.FeatureStarted
				? intermediateResult.CollectedFeatures
				: intermediateResult.CollectedTransformation;
			var builder = result.Builder;
			builder.Append("            this.Write(\"");
			builder.Append(StringLiteralConverter.EscapeToRegular(token.GetText()));
			builder.AppendLine("\");");
		}
	}
}
