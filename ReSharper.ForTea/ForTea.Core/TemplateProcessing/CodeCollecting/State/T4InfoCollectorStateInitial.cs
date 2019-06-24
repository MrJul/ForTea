using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public sealed class T4InfoCollectorStateInitial : T4InfoCollectorStateBase
	{
		public override T4InfoCollectorStateBase GetNextState(ITreeNode element)
		{
			switch (element)
			{
				case IT4Directive _: return new T4InfoCollectorSateSeenDirective();
				case T4FeatureBlock _: return new T4InfoCollectorStateSeenFeature();
				default: return this;
			}
		}

		[NotNull]
		public override string ConvertForAppending(IT4Token token) => Convert(token);

		public override bool FeatureStarted => false;
	}
}
