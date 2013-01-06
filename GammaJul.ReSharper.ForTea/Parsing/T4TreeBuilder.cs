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
using System;
using System.Collections.Generic;
using System.Globalization;
using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Parsing {

	/// <summary>
	/// Builds a T4 tree from a lexer. This is where the parsing really happens.
	/// </summary>
	internal sealed partial class T4TreeBuilder {

		private readonly List<IT4Include> _includes = new List<IT4Include>();
		private readonly T4Environment _t4Environment;
		private readonly DirectiveInfoManager _directiveInfoManager;
		private readonly PsiBuilderLexer _builderLexer;
		private readonly IPsiSourceFile _sourceFile;
		private readonly ISolution _solution;
		private readonly T4PsiModule _macroResolveModule;
		private readonly HashSet<FileSystemPath> _existingIncludePaths;
		private List<T4Directive> _notClosedDirectives;

		/// <summary>
		/// Advances the lexer to the next token.
		/// </summary>
		/// <returns>The type of the next token.</returns>
		[CanBeNull]
		private T4TokenNodeType Advance() {
			_builderLexer.Advance();
			return GetTokenType();
		}

		/// <summary>
		/// Returns the type of the lexer current token.
		/// </summary>
		/// <returns>The type of the current token.</returns>
		[CanBeNull]
		private T4TokenNodeType GetTokenType() {
			return (T4TokenNodeType) _builderLexer.TokenType;
		}

		/// <summary>
		/// Creates a <see cref="IT4File"/>.
		/// </summary>
		/// <returns>An instance of <see cref="IT4File"/> containing a T4 tree.</returns>
		[NotNull]
		internal IT4File CreateT4Tree() {
			_builderLexer.Start();
			var file = new T4File();
			
			Parse(file);
			
			if (_sourceFile != null) {
				file.SetSourceFile(_sourceFile);
				file.DocumentRangeTranslator = new T4DocumentRangeTranslator(file, _sourceFile, _includes);
			}

			return file;
		}

		[NotNull]
		private T4Include CreateIncludeT4Tree() {
			_builderLexer.Start();
			var include = new T4Include();
			Parse(include);

			if (_sourceFile != null)
				include.DocumentRangeTranslator = new T4DocumentRangeTranslator(include, _sourceFile, _includes);

			return include;
		}

		private void Parse([NotNull] CompositeElement parentElement) {
			T4TokenNodeType tokenNodeType = GetNonCodeBlockTokenType(parentElement);
			while (tokenNodeType != null) {
				if (tokenNodeType == T4TokenNodeTypes.DirectiveStart)
					ParseDirective(parentElement);
				else {
					AppendNewChild(parentElement, tokenNodeType);
					Advance();
				}
				tokenNodeType = GetNonCodeBlockTokenType(parentElement);
			}
			if (_builderLexer.HasSkippedTokens)
				_builderLexer.AppendSkippedTokens(parentElement);
			if (_notClosedDirectives != null)
				FixTopLevelSpace(parentElement, _notClosedDirectives);
		}

		/// <summary>
		/// Appends an error element where a token of a given type was expected.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="missingTokenType">The missing token type.</param>
		private void AppendMissingToken([NotNull] CompositeElement parentElement, MissingTokenType missingTokenType) {
			AppendNewChild(parentElement, new MissingTokenErrorElement(missingTokenType));
		}

		/// <summary>
		/// Gets the first token that is not a code block related token.
		/// </summary>
		/// <param name="parentElement">The parent element where the potential code block will be appended as a child.</param>
		/// <returns>A <see cref="T4TokenNodeType"/>, or <c>null</c>.</returns>
		[CanBeNull]
		private T4TokenNodeType GetNonCodeBlockTokenType(CompositeElement parentElement) {
			var tokenNodeType = GetTokenType();
			while (TryParseCodeBlock(tokenNodeType, parentElement))
				tokenNodeType = Advance();
			return tokenNodeType;
		}

		/// <summary>
		/// Checks if the current token represents the beginning of a code block, if yes, parse every code block after the token.
		/// </summary>
		/// <param name="tokenNodeType">The current token type.</param>
		/// <param name="parentElement">The parent element where the potential code block will be appended as a child.</param>
		/// <returns><c>true</c> is a code block has been parsed, <c>false</c> otherwise.</returns>
		private bool TryParseCodeBlock([CanBeNull] T4TokenNodeType tokenNodeType, [NotNull] CompositeElement parentElement) {
			if (tokenNodeType != null) {

				T4CodeBlock codeBlock;
				if (tokenNodeType == T4TokenNodeTypes.StatementStart)
					codeBlock = new T4StatementBlock();
				else if (tokenNodeType == T4TokenNodeTypes.ExpressionStart)
					codeBlock = new T4ExpressionBlock();
				else if (tokenNodeType == T4TokenNodeTypes.FeatureStart)
					codeBlock = new T4FeatureBlock();
				else
					codeBlock = null;

				if (codeBlock != null) {
					AppendNewChild(parentElement, ParseCodeBlock(tokenNodeType, codeBlock));
					return true;
				}

			}
			return false;
		}

		/// <summary>
		/// Appends a new composite element to the tree.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="childElement">The child element.</param>
		private void AppendNewChild(CompositeElement parentElement, TreeElement childElement) {
			_builderLexer.AppendNewChild(parentElement, childElement);
		}

		/// <summary>
		/// Creates and appends a new token to the tree.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="tokenNodeType">Type of the token node to create and add.</param>
		private void AppendNewChild([NotNull] CompositeElement parentElement, [NotNull] T4TokenNodeType tokenNodeType) {
			var token = tokenNodeType.Create(_builderLexer.Buffer, new TreeOffset(_builderLexer.TokenStart), new TreeOffset(_builderLexer.TokenEnd));
			_builderLexer.AppendNewChild(parentElement, token);
		}

		private LeafElementBase CreateCurrentToken() {
			T4TokenNodeType tokenType = GetTokenType();
			Assertion.AssertNotNull(tokenType, "tokenType == null");
			LeafElementBase token = tokenType.Create(_builderLexer.Buffer, new TreeOffset(_builderLexer.TokenStart), new TreeOffset(_builderLexer.TokenEnd));
			return token;
		}

		/// <summary>
		/// Parses a T4 code block.
		/// </summary>
		/// <param name="codeStartTokenNodeType">The statement start token type.</param>
		/// <param name="codeBlock">An empty code block that will contain the parsed code block.</param>
		[NotNull]
		private T4CodeBlock ParseCodeBlock([NotNull] T4TokenNodeType codeStartTokenNodeType, [NotNull] T4CodeBlock codeBlock) {

			// appends the code start token (<#/<#+/<#=) to the block
			AppendNewChild(codeBlock, codeStartTokenNodeType);

			// parse every Code token until a BlockEnd (that is also appended) or EOF is reached
			bool blockEnded = false;
			do {
				T4TokenNodeType nextTokenType = Advance();
				if (nextTokenType == null) {
					AppendMissingToken(codeBlock, MissingTokenType.BlockEnd);
					blockEnded = true;
				}
				else {
					if (nextTokenType != T4TokenNodeTypes.Code) {
						if (nextTokenType == T4TokenNodeTypes.BlockEnd)
							blockEnded = true;
						else
							AppendMissingToken(codeBlock, MissingTokenType.BlockEnd);
					}
					AppendNewChild(codeBlock, nextTokenType);
				}
			}
			while (!blockEnded);

			return codeBlock;
		}

		private void ParseDirective([NotNull] CompositeElement parentElement) {
			var directive = new T4Directive();

			// appends the directive start token (<#@)
			AppendNewChild(directive, T4TokenNodeTypes.DirectiveStart);
			Advance();

			// builds the directive (name and attributes)
			var directiveBuilder = new DirectiveBuilder(this);
			T4TokenNodeType tokenType = GetTokenType();
			while (tokenType != null && !tokenType.IsTag) {
				if (tokenType == T4TokenNodeTypes.Name)
					directiveBuilder.AddName();
				else if (tokenType == T4TokenNodeTypes.Equal)
					directiveBuilder.AddEqual();
				else if (tokenType == T4TokenNodeTypes.Quote)
					directiveBuilder.AddQuote();
				else if (tokenType == T4TokenNodeTypes.Value)
					directiveBuilder.AddValue();
				tokenType = Advance();
			}
			directiveBuilder.Finish(directive);

			// appends the block end token if available
			if (tokenType == T4TokenNodeTypes.BlockEnd) {
				AppendNewChild(directive, T4TokenNodeTypes.BlockEnd);
				Advance();
			}
			else {
				AppendMissingToken(directive, MissingTokenType.BlockEnd);
				if (_notClosedDirectives == null)
					_notClosedDirectives = new List<T4Directive>();
				_notClosedDirectives.Add(directive);
			}

			AppendNewChild(parentElement, directive);

			// checks if we're including a file
			if (directive.IsSpecificDirective(_directiveInfoManager.Include))
				HandleIncludeDirective(directive, parentElement);
		}

		private void HandleIncludeDirective([NotNull] IT4Directive directive, [NotNull] CompositeElement parentElement) {
			var fileAttr = (T4DirectiveAttribute) directive.GetAttribute(_directiveInfoManager.Include.FileAttribute.Name);
			if (fileAttr == null)
				return;

			IT4Token valueToken = fileAttr.GetValueToken();
			if (valueToken == null)
				return;

			HandleInclude(valueToken.GetText(), fileAttr, parentElement);
		}

		private void HandleInclude([CanBeNull] string includeFileName, [NotNull] T4DirectiveAttribute fileAttr, [NotNull] CompositeElement parentElement) {
			FileSystemPath includePath = ResolveInclude(includeFileName);
			if (includePath.IsEmpty) {
				fileAttr.ValueError = String.Format(CultureInfo.InvariantCulture, "Unresolved file \"{0}\"", includePath);
				return;
			}

			if (!_existingIncludePaths.Add(includePath)) {
				fileAttr.ValueError = String.Format(CultureInfo.InvariantCulture, "Already included file \"{0}\"", includePath);
				return;
			}

			FileSystemPath sourceLocation = _sourceFile.GetLocation();
			if (includePath == sourceLocation) {
				fileAttr.ValueError = "Recursive include";
				_existingIncludePaths.Add(includePath);
				return;
			}

			if (!includePath.ExistsFile) {
				fileAttr.ValueError = String.Format(CultureInfo.InvariantCulture, "File \"{0}\" not found", includePath);
				return;
			}

			// find the matching include in the existing solution source files
			// or create a new one if the include file is outside the solution
			IPsiSourceFile includeSourceFile = includePath.FindSourceFileInSolution(_solution) ?? CreateIncludeSourceFile(includePath);
			if (includeSourceFile == null) {
				Logger.LogError("Include failed: " + includePath);
				fileAttr.ValueError = "No current solution";
				return;
			}

			ILexer includeLexer = CreateLexer(includeSourceFile);
			var builder = new T4TreeBuilder(_t4Environment, _directiveInfoManager, includeLexer, includeSourceFile, _existingIncludePaths, _solution, _macroResolveModule);
			T4Include include = builder.CreateIncludeT4Tree();
			include.Path = includePath;
			_includes.Add(include);

			// do not use AppendNewChild, we don't want the PsiBuilderLexer to move line breaks from the include into the main file.
			parentElement.AddChild(include);
		}

		[CanBeNull]
		private IPsiSourceFile CreateIncludeSourceFile([NotNull] FileSystemPath path) {
			if (_solution == null)
				return null;

			var sourceFileManager = _solution.GetComponent<T4OutsideSolutionSourceFileManager>();
			return sourceFileManager.GetOrCreateSourceFile(path);
		}

		[NotNull]
		private static ILexer CreateLexer([NotNull] IPsiSourceFile includeSourceFile) {
			LanguageService languageService = T4Language.Instance.LanguageService();
			Assertion.AssertNotNull(languageService, "languageService != null");
			return languageService.GetPrimaryLexerFactory().CreateLexer(includeSourceFile.Document.Buffer);
		}

		[NotNull]
		private FileSystemPath ResolveInclude([CanBeNull] string fileName) {
			if (String.IsNullOrEmpty(fileName))
				return FileSystemPath.Empty;

			try {

				// an include path can contain environment variables and visual studio macros
				fileName = Environment.ExpandEnvironmentVariables(fileName);
				fileName = ExpandVisualStudioMacros(fileName);
				
				// absolute file path, nothing to search for
				var path = new FileSystemPath(fileName);
				if (path.IsAbsolute)
					return path;

				// search relative to the current file
				FileSystemPath sourceLocation = _sourceFile.GetLocation();
				FileSystemPath currentDirIncludePath;
				if (!sourceLocation.IsEmpty) {
					currentDirIncludePath = sourceLocation.Directory.Combine(fileName);
					if (currentDirIncludePath.ExistsFile)
						return currentDirIncludePath;
				}
				else
					currentDirIncludePath = FileSystemPath.Empty;

				// search in global include paths
				foreach (FileSystemPath includePath in _t4Environment.IncludePaths) {
					FileSystemPath resultPath = includePath.Combine(fileName);
					if (resultPath.ExistsFile)
						return resultPath;
				}

				return currentDirIncludePath;
			}
			catch (InvalidPathException) {
			}
			catch (ArgumentException) {
			}
			return FileSystemPath.Empty;
		}

		/// <summary>
		/// Expands the Visual Studio macros inside a filename, eg $(SolutionDir).
		/// </summary>
		/// <param name="fileName">The file name to expand.</param>
		/// <returns><paramref name="fileName"/> with expanded macros.</returns>
		[NotNull]
		private string ExpandVisualStudioMacros([NotNull] string fileName) {
			return _sourceFile != null
				? VsBuildMacroHelper.ResolveMacros(fileName, _macroResolveModule)
				: fileName;
		}

		/// <summary>
		/// Unclosed directives may have trailing spaces that are skipped then added at file level by the <see cref="PsiBuilderLexer"/>.
		/// In this T4 parser, space tokens can only appear inside directives so we're putting them back in.
		/// </summary>
		/// <param name="file">The file containing non closed directives.</param>
		/// <param name="notClosedDirectives">The list of directives that aren't closed.</param>
		private static void FixTopLevelSpace([NotNull] CompositeElement file, [NotNull] IEnumerable<T4Directive> notClosedDirectives) {
			foreach (T4Directive directive in notClosedDirectives) {
				ITreeNode potentialSpace = directive.NextSibling;
				if (potentialSpace == null || potentialSpace.GetTokenType() != T4TokenNodeTypes.Space)
					continue;
				file.DeleteChildRange(potentialSpace, potentialSpace);
				Assertion.Assert(directive.LastChild is IErrorElement, "directive.LastChild is IErrorElement");
				directive.AddChildBefore(potentialSpace, directive.LastChild);
			}
		}

		[CanBeNull]
		private static ISolution GetSourceSolution([CanBeNull] IPsiSourceFile sourceFile) {
			return sourceFile != null ? sourceFile.GetSolution() : null;
		}

		[CanBeNull]
		private static T4PsiModule GetSourceModule([CanBeNull] IPsiSourceFile sourceFile) {
			return sourceFile != null ? sourceFile.PsiModule as T4PsiModule : null;
		}

		internal T4TreeBuilder([NotNull] T4Environment t4Environment, [NotNull] DirectiveInfoManager directiveInfoManager, [NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile sourceFile = null)
			: this(t4Environment, directiveInfoManager, lexer, sourceFile, new HashSet<FileSystemPath>(), GetSourceSolution(sourceFile), GetSourceModule(sourceFile)) {
		}

		private T4TreeBuilder([NotNull] T4Environment t4Environment, [NotNull] DirectiveInfoManager directiveInfoManager, [NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile sourceFile, [NotNull] HashSet<FileSystemPath> existingIncludePaths, [CanBeNull] ISolution solution,
			[CanBeNull] T4PsiModule macroResolveModule) {
			_t4Environment = t4Environment;
			_directiveInfoManager = directiveInfoManager;
			_builderLexer = new PsiBuilderLexer(lexer, tnt => tnt.IsWhitespace);
			_existingIncludePaths = existingIncludePaths;
			_sourceFile = sourceFile;
			_solution = solution;
			_macroResolveModule = macroResolveModule;
		}

	}

}