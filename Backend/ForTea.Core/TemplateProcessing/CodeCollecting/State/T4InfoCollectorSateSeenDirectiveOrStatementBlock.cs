using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorSateSeenDirectiveOrStatementBlock : T4InfoCollectorStateBase
	{
		private StringBuilder Builder { get; }
		public T4InfoCollectorSateSeenDirectiveOrStatementBlock() => Builder = new StringBuilder();

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _:
				case T4StatementBlock _: return this;
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
			if (token.NodeType != T4TokenNodeTypes.NEW_LINE) Builder.Append(Convert(token));
		}

		protected override string ProduceSafe(ITreeNode lookahead) => null;
		protected override string ProduceBeforeEofSafe() => null;
	}
}
