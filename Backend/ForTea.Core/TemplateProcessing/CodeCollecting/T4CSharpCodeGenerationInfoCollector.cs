using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

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

		protected override void AppendCode(T4CSharpCodeGenerationResult result, TreeElement token) =>
			result.AppendMapped(token);

		protected override void AppendTransformation(string message)
		{
			var result = Result.FeatureStarted
				? Result.CollectedFeatures
				: Result.CollectedTransformation;
			result.Append("            this.Write(\"");
			result.Append(message);
			result.AppendLine("\");");
		}
	}
}
