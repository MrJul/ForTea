using System.Linq;
using System.Text;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Parsing {

	/// <summary>A factory for T4 composite elements.</summary>
	public static class T4ElementFactory {

		/// <summary>Creates a new statement block (&lt;# ... #&gt;).</summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="T4StatementBlock"/>.</returns>
		[NotNull]
		public static T4StatementBlock CreateStatementBlock([CanBeNull] string code)
			=> (T4StatementBlock) CreateTreeAndGetFirstChild("<#" + code + "#>");

		/// <summary>Creates a new feature block (&lt;#+ ... #&gt;).</summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="T4FeatureBlock"/>.</returns>
		[NotNull]
		public static T4FeatureBlock CreateFeatureBlock([CanBeNull] string code)
			=> (T4FeatureBlock) CreateTreeAndGetFirstChild("<#+" + code + "#>");

		/// <summary>Creates a new expression block (&lt;#= ... #&gt;).</summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="T4ExpressionBlock"/>.</returns>
		[NotNull]
		public static T4ExpressionBlock CreateExpressionBlock([CanBeNull] string code)
			=> (T4ExpressionBlock) CreateTreeAndGetFirstChild("<#=" + code + "#>");

		/// <summary>Creates a new directive (&lt;#@ ... #&gt;).</summary>
		/// <param name="directiveName">Name of the directive.</param>
		/// <param name="attributes">The directive attributes.</param>
		/// <returns>A new instance of <see cref="T4Directive"/>.</returns>
		[NotNull]
		public static T4Directive CreateDirective([CanBeNull] string directiveName, [CanBeNull] params Pair<string, string>[] attributes) {
			var builder = new StringBuilder("<#@ ");
			builder.Append(directiveName);
			if (attributes != null) {
				foreach (var pair in attributes)
					builder.AppendFormat(" {0}=\"{1}\"", pair.First, pair.Second);
			}
			builder.Append(" #>");
			return (T4Directive) CreateTreeAndGetFirstChild(builder.ToString());
		}

		/// <summary>Creates a new directive attribute.</summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <returns>A new instance of <see cref="IT4DirectiveAttribute"/>.</returns>
		[NotNull]
		public static IT4DirectiveAttribute CreateDirectiveAttribute([CanBeNull] string name, [CanBeNull] string value) {
			T4Directive directive = CreateDirective("dummy", Pair.Of(name, value));
			return directive.GetAttributes().First();
		}

		[NotNull]
		private static ITreeNode CreateTreeAndGetFirstChild([NotNull] string text) {
			LanguageService languageService = T4Language.Instance.LanguageService();
			Assertion.AssertNotNull(languageService, "languageService != null");

			ILexer lexer = languageService.GetPrimaryLexerFactory().CreateLexer(new StringBuffer(text));
			IParser parser = languageService.CreateParser(lexer, null, null);
			IFile file = parser.ParseFile();
			Assertion.AssertNotNull(file, "file != null");
			Assertion.AssertNotNull(file.FirstChild, "file.FirstChild != null");
			return file.FirstChild;
		}

	}

}