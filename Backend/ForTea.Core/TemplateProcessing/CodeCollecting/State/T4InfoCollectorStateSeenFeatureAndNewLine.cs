using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public class T4InfoCollectorStateSeenFeatureAndNewLine : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; }

		public T4InfoCollectorStateSeenFeatureAndNewLine() => Builder = new StringBuilder();

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case T4FeatureBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature();
				default:
					if (element.NodeType == T4TokenNodeTypes.NewLine) return this;
					else if (element.NodeType == T4TokenNodeTypes.Text)
					{
						Die();
						return new T4InfoCollectorStateSeenFeatureAndText(Builder);
					}

					throw new T4OutputGenerationException();
			}
		}

		protected override bool FeatureStartedSafe => true;
		protected override void ConsumeTokenSafe(IT4Token token) => Builder.Append(Convert(token));
		protected override string ProduceBeforeEofSafe() => null;

		protected override string ProduceSafe(ITreeNode lookahead)
		{
			if (lookahead is T4FeatureBlock) return null;
			return Builder.ToString();
		}
	}
}
