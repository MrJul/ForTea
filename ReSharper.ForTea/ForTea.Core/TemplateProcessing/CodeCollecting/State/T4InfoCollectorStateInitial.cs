using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateInitial : T4InfoCollectorStateBase
	{
		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case T4FeatureBlock _: return new T4InfoCollectorStateSeenFeature();
				case IT4Directive _:
				case T4StatementBlock _:
				case T4ExpressionBlock _: return new T4InfoCollectorSateSeenDirectiveOrNonFeatureBlock();
				default: return this;
			}
		}

		[NotNull]
		public override string ConvertTokenForAppending(IT4Token token) => Convert(token);

		public override bool FeatureStarted => false;
	}
}
