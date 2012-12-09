using GammaJul.ReSharper.ForTea.Psi.Directives;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Parsing {
	
	internal sealed class T4Parser : IParser {

		private readonly T4Environment _t4Environment;
		private readonly DirectiveInfoManager _directiveInfoManager;
		private readonly ILexer _lexer;
		private readonly IPsiSourceFile _sourceFile;

		public IFile ParseFile() {
			return new T4TreeBuilder(_t4Environment, _directiveInfoManager, _lexer, _sourceFile).CreateT4Tree();
		}

		internal T4Parser([NotNull] T4Environment t4Environment, [NotNull] DirectiveInfoManager directiveInfoManager, [NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile sourceFile) {
			_t4Environment = t4Environment;
			_directiveInfoManager = directiveInfoManager;
			_lexer = lexer;
			_sourceFile = sourceFile;
		}

	}

}