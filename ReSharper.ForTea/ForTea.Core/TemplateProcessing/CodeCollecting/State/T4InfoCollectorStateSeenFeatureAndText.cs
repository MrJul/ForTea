using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public class T4InfoCollectorStateSeenFeatureAndText : T4InfoCollectorStateBase
	{
		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _: throw new T4OutputGenerationException();
				case T4FeatureBlock _: return new T4InfoCollectorStateSeenFeature();
				default: return this;
			}
		}

		[NotNull]
		public override string ConvertTokenForAppending(IT4Token token) => Convert(token);
		public override bool FeatureStarted => true;
	}
}
