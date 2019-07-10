using GammaJul.ForTea.Core.Psi;
using JetBrains.ForTea.RiderPlugin.Features.Folding;
using JetBrains.ReSharper.Daemon.CodeFolding;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl.DocumentMarkup;

[assembly: RegisterHighlighter(T4CodeFoldingAttributes.Directive, EffectType = EffectType.FOLDING)]

namespace JetBrains.ForTea.RiderPlugin.Features.Folding
{
	[Language(typeof(T4Language))]
	public class T4CodeFoldingProcessorFactory : ICodeFoldingProcessorFactory
	{
		public ICodeFoldingProcessor CreateProcessor() => new T4CodeFoldingProcessor();
	}
}
