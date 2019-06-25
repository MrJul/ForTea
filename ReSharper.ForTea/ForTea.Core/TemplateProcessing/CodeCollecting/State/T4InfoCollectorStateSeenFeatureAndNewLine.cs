using System.Diagnostics;
using System.Text;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public class T4InfoCollectorStateSeenFeatureAndNewLine : T4InfoCollectorStateBase
	{
		private bool IsAlive { get; set; }
		private int NewLines { get; set; }

		public T4InfoCollectorStateSeenFeatureAndNewLine()
		{
			IsAlive = true;
			NewLines = 0;
		}

		[Conditional("JET_MODE_ASSERT")]
		private void Die() => IsAlive = false;
		
		[Conditional("JET_MODE_ASSERT")]
		private void Check() => Assertion.Assert(IsAlive, "Attempted to use dead state");

		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			Check();
			switch (element)
			{
				case IT4Directive _: throw new T4OutputGenerationException(); // TODO: remove
				case T4FeatureBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature();
				default:
					if (element.NodeType == T4TokenNodeTypes.NewLine)
						// newline has already been appended in ConvertForAppending
						return this;
					else if (element.NodeType == T4TokenNodeTypes.Text)
					{
						Die();
						// Builder contents have already been returned in ConvertForAppending
						return new T4InfoCollectorStateSeenFeatureAndText();
					}

					throw new T4OutputGenerationException();
			}
		}

		public override string ConvertTokenForAppending(IT4Token token)
		{
			Check();
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
				Check();
				return true;
			}
		}
	}
}
