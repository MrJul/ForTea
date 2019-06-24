using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateSeenFeature : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; } // TODO: replace builder with newline counter

		public T4InfoCollectorStateSeenFeature() => Builder = new StringBuilder();

		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _: throw new T4OutputGenerationException();
				case T4FeatureBlock _:
					Builder.Clear();
					return this;
				default:
					if (element.NodeType == T4TokenNodeTypes.NewLine)
						// newline has already been appended in ConvertForAppending
						return this;
					else if (element.NodeType == T4TokenNodeTypes.Text)
						// Builder contents have already been printed in ConvertForAppending
						return new T4InfoCollectorStateSeenFeatureAndText();

					throw new T4OutputGenerationException();
			}
		}

		public override string ConvertForAppending(IT4Token token)
		{
			Builder.Append(Convert(token)); // TODO: use representation instead
			if (token.NodeType == T4TokenNodeTypes.NewLine)
			{
				return null;
			}

			string result = Builder.ToString();
			Builder.Clear();
			return result;
		}

		public override bool FeatureStarted => true;
	}
}
