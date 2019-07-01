using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorSateSeenDirectiveOrNonFeatureBlock : T4InfoCollectorStateBase
	{
		private StringBuilder Builder { get; }

		public T4InfoCollectorSateSeenDirectiveOrNonFeatureBlock() => Builder = new StringBuilder();

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _:
				case T4StatementBlock _:
				case T4ExpressionBlock _: return this;
				case T4FeatureBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature();
				default:
					Die();
					return new T4InfoCollectorStateInitial(Builder);
			}
		}

		protected override bool FeatureStartedSafe => false;

		protected override void ConsumeTokenSafe(IT4Token token)
		{
			if (token.NodeType != T4TokenNodeTypes.NewLine) Builder.Append(Convert(token));
		}

		// This state never produces anything
		protected override string ProduceSafe() => null;
	}
}
