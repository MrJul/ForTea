using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateSeenFeature : T4InfoCollectorStateBase
	{
		private bool IsAlive { get; set; }
		private int NewLines { get; set; }

		public T4InfoCollectorStateSeenFeature()
		{
			IsAlive = true;
			NewLines = 0;
		}

		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			Assertion.Assert(IsAlive, "Attempted to use dead state");
			switch (element)
			{
				case IT4Directive _: throw new T4OutputGenerationException();
				case T4FeatureBlock _:
					NewLines = 0;
					return this;
				default:
					if (element.NodeType == T4TokenNodeTypes.NewLine)
						// newline has already been appended in ConvertForAppending
						return this;
					else if (element.NodeType == T4TokenNodeTypes.Text)
					{
						// State is not supposed to be used after it first sees text
						IsAlive = false;
						// Builder contents have already been returned in ConvertForAppending
						return new T4InfoCollectorStateSeenFeatureAndText();
					}

					throw new T4OutputGenerationException();
			}
		}

		public override string ConvertForAppending(IT4Token token)
		{
			Assertion.Assert(IsAlive, "Attempted to use dead state");
			if (token.NodeType == T4TokenNodeTypes.NewLine)
			{
				NewLines += 1;
				return null;
			}

			var result = new StringBuilder();
			for (int i = 0; i < NewLines; i += 1)
			{
				result.AppendLine();
			}

			// Convert is intentionally not used here
			result.Append(token.GetText());
			return StringLiteralConverter.EscapeToRegular(result.ToString());
		}

		public override bool FeatureStarted
		{
			get
			{
				Assertion.Assert(IsAlive, "Attempted to use dead state");
				return true;
			}
		}
	}
}
