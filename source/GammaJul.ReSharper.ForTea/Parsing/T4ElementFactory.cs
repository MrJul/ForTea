#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion


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

	/// <summary>
	/// A factory for T4 composite elements.
	/// </summary>
	public sealed class T4ElementFactory {

		/// <summary>
		/// Gets an unique instance of <see cref="T4ElementFactory"/>.
		/// </summary>
		public static readonly T4ElementFactory Instance = new T4ElementFactory();

		/// <summary>
		/// Creates a new statement block (&lt;# ... #&gt;).
		/// </summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="T4StatementBlock"/>.</returns>
		[NotNull]
		public T4StatementBlock CreateStatementBlock([CanBeNull] string code) {
			return (T4StatementBlock) CreateTreeAndGetFirstChild("<#" + code + "#>");
		}

		/// <summary>
		/// Creates a new feature block (&lt;#+ ... #&gt;).
		/// </summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="T4FeatureBlock"/>.</returns>
		[NotNull]
		public T4FeatureBlock CreateFeatureBlock([CanBeNull] string code) {
			return (T4FeatureBlock) CreateTreeAndGetFirstChild("<#+" + code + "#>");
		}

		/// <summary>
		/// Creates a new expression block (&lt;#= ... #&gt;).
		/// </summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="T4ExpressionBlock"/>.</returns>
		[NotNull]
		public T4ExpressionBlock CreateExpressionBlock([CanBeNull] string code) {
			return (T4ExpressionBlock) CreateTreeAndGetFirstChild("<#=" + code + "#>");
		}

		/// <summary>
		/// Creates a new directive (&lt;#@ ... #&gt;).
		/// </summary>
		/// <param name="directiveName">Name of the directive.</param>
		/// <param name="attributes">The directive attributes.</param>
		/// <returns>A new instance of <see cref="T4Directive"/>.</returns>
		[NotNull]
		public T4Directive CreateDirective([CanBeNull] string directiveName, [CanBeNull] params Pair<string, string>[] attributes) {
			var builder = new StringBuilder("<#@ ");
			builder.Append(directiveName);
			if (attributes != null) {
				foreach (var pair in attributes)
					builder.AppendFormat(" {0}=\"{1}\"", pair.First, pair.Second);
			}
			builder.Append(" #>");
			return (T4Directive) CreateTreeAndGetFirstChild(builder.ToString());
		}

		/// <summary>
		/// Creates a new directive attribute.
		/// </summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <returns>A new instance of <see cref="IT4DirectiveAttribute"/>.</returns>
		[NotNull]
		public IT4DirectiveAttribute CreateDirectiveAttribute([CanBeNull] string name, [CanBeNull] string value) {
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

		private T4ElementFactory() {
		}
	}

}