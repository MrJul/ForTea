using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateSeenFeature : T4InfoCollectorStateBase
	{
		private IT4Token LastToken { get; set; }

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case T4FeatureBlock _: return this;
				default:
					Die();
					if (element.NodeType == T4TokenNodeTypes.NEW_LINE)
						return new T4InfoCollectorStateSeenFeatureAndNewLine();
					else if (element.NodeType == T4TokenNodeTypes.RAW_TEXT)
						return new T4InfoCollectorStateSeenFeatureAndText(new StringBuilder(Convert(LastToken)));

					throw new T4OutputGenerationException();
			}
		}

		protected override bool FeatureStartedSafe => false;

		protected override void ConsumeTokenSafe(IT4Token token)
		{
			if (token.NodeType != T4TokenNodeTypes.NEW_LINE) LastToken = token;
		}

		protected override string ProduceSafe(ITreeNode lookahead) => null;
		protected override string ProduceBeforeEofSafe() => null;
	}
}
