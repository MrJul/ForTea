using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public interface IT4InfoCollectorState
	{
		[CanBeNull]
		string Produce(ITreeNode lookahead);

		[CanBeNull]
		string ProduceBeforeEof();

		[NotNull]
		IT4InfoCollectorState GetNextState([NotNull] ITreeNode element);

		bool FeatureStarted { get; }
		void ConsumeToken([NotNull] IT4Token token);
	}
}
