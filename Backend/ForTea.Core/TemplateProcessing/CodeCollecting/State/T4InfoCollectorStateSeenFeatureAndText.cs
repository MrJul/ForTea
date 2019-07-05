using System.Text;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public class T4InfoCollectorStateSeenFeatureAndText : T4InfoCollectorStateBase
	{
		[NotNull]
		private StringBuilder Builder { get; }

		public T4InfoCollectorStateSeenFeatureAndText([NotNull] StringBuilder builder) => Builder = builder;

		protected override IT4InfoCollectorState GetNextStateSafe(ITreeNode element)
		{
			switch (element)
			{
				case T4FeatureBlock _:
					Die();
					return new T4InfoCollectorStateSeenFeature();
				case IT4Token _: return this;
				default: throw new T4OutputGenerationException();
			}
		}

		protected override bool FeatureStartedSafe => true;
		protected override void ConsumeTokenSafe(IT4Token token) => Builder.Append(Convert(token));
		protected override string ProduceSafe(ITreeNode lookahead) => Builder.ToString();
		protected override string ProduceBeforeEofSafe() => throw new T4OutputGenerationException();
	}
}
