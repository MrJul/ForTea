using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi.Directives;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Parsing {
	
	internal sealed class T4Parser : IParser {

		[NotNull] private readonly IT4Environment _t4Environment;
		[NotNull] private readonly T4DirectiveInfoManager _directiveInfoManager;
		[NotNull] private readonly ILexer _lexer;
		[CanBeNull] private readonly IPsiSourceFile _sourceFile;

		public IFile ParseFile()
			=> new T4TreeBuilder(_t4Environment, _directiveInfoManager, _lexer, _sourceFile).CreateT4Tree();

		public T4Parser(
			[NotNull] IT4Environment t4Environment,
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile sourceFile
		) {
			_t4Environment = t4Environment;
			_directiveInfoManager = directiveInfoManager;
			_lexer = lexer;
			_sourceFile = sourceFile;
		}

	}

}