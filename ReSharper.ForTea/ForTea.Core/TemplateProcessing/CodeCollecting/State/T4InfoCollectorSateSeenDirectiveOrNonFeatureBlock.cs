using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorSateSeenDirectiveOrNonFeatureBlock : T4InfoCollectorStateBase
	{
		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _:
				case T4StatementBlock _:
				case T4ExpressionBlock _: return this;
				case T4FeatureBlock _: return new T4InfoCollectorStateSeenFeature();
				default: return new T4InfoCollectorStateInitial();
			}
		}

		public override string ConvertTokenForAppending(IT4Token token)
		{
			if (token.NodeType == T4TokenNodeTypes.NewLine) return null;
			return Convert(token);
		}

		public override bool FeatureStarted => false;
	}
}
