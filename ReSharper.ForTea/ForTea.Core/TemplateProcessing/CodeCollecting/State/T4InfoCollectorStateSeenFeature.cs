using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
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
					if (element.NodeType == T4TokenNodeTypes.NewLine)
						return new T4InfoCollectorStateSeenFeatureAndNewLine();
					else if (element.NodeType == T4TokenNodeTypes.Text)
						return new T4InfoCollectorStateSeenFeatureAndText(new StringBuilder(Convert(LastToken)));

					throw new T4OutputGenerationException();
			}
		}

		protected override bool FeatureStartedSafe => false;

		protected override void ConsumeTokenSafe(IT4Token token)
		{
			if (token.NodeType != T4TokenNodeTypes.NewLine) LastToken = token;
		}

		// This state never produces anything. Instead, it passes information to other states
		protected override string ProduceSafe() => null;
	}
}
