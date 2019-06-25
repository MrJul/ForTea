using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateSeenFeature : T4InfoCollectorStateBase
	{
		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _: throw new T4OutputGenerationException(); // TODO: remove
				case T4FeatureBlock _: return this;
				default:
					if (element.NodeType == T4TokenNodeTypes.NewLine)
						return new T4InfoCollectorStateSeenFeatureAndNewLine();
					else if (element.NodeType == T4TokenNodeTypes.Text)
						return new T4InfoCollectorStateSeenFeatureAndText();

					throw new T4OutputGenerationException();
			}
		}

		public override string ConvertTokenForAppending(IT4Token token)
		{
			if (token.NodeType == T4TokenNodeTypes.NewLine) return null;
			return StringLiteralConverter.EscapeToRegular(token.GetText());
		}

		public override bool FeatureStarted => false;
	}
}
