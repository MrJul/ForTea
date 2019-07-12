using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.ProblemAnalyzers
{
	[ElementProblemAnalyzer(typeof(IT4CodeBlock),
		HighlightingTypes = new[] {typeof(EmptyBlockHighlighting)})]
	public class T4TestProblemAnalyzer : ElementProblemAnalyzer<IT4CodeBlock>
	{
		protected override void Run(
			IT4CodeBlock element,
			ElementProblemAnalyzerData data,
			IHighlightingConsumer consumer
		)
		{
			consumer.AddHighlighting(new EmptyBlockHighlighting(element));
		}
	}
}
