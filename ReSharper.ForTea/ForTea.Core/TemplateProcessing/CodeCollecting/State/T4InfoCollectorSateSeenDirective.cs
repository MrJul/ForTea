using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorSateSeenDirective : T4InfoCollectorStateBase
	{
		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _: return this;
				case T4FeatureBlock _: return new T4InfoCollectorStateSeenFeature();
				default: return new T4InfoCollectorStateInitial();
			}
		}

		public override string ConvertForAppending(IT4Token token) =>
			token.NodeType == T4TokenNodeTypes.NewLine ? null : Convert(token);

		public override bool FeatureStarted => false;
	}
}
