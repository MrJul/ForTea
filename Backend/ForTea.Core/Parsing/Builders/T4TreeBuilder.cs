using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.Parsing.Builders
{
	/// <summary>Builds a T4 tree from a lexer. This is where the parsing really happens.</summary>
	internal sealed class T4TreeBuilder
	{
		#region Properties
		[NotNull]
		private List<IT4Include> Includes { get; } = new List<IT4Include>();

		[NotNull]
		private IT4Environment Environment { get; }

		[NotNull]
		private T4DirectiveInfoManager DirectiveInfoManager { get; }

		[NotNull]
		private PsiBuilderLexer BuilderLexer { get; }

		[CanBeNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		private HashSet<IT4PathWithMacros> ExistingIncludePaths { get; }

		[CanBeNull]
		private List<T4Directive> NotClosedDirectives { get; set; }
		#endregion

		internal T4TreeBuilder(
			[NotNull] IT4Environment environment,
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile sourceFile
		)
		{
			Environment = environment;
			DirectiveInfoManager = directiveInfoManager;
			BuilderLexer = new PsiBuilderLexer(lexer, tnt => tnt.IsWhitespace);
			ExistingIncludePaths = new HashSet<IT4PathWithMacros>();
			SourceFile = sourceFile;
		}

		/// <summary>Advances the lexer to the next token.</summary>
		/// <returns>The type of the next token.</returns>
		[CanBeNull]
		private T4TokenNodeType Advance()
		{
			BuilderLexer.Advance();
			return GetTokenType();
		}

		/// <summary>Returns the type of the lexer current token.</summary>
		/// <returns>The type of the current token.</returns>
		[CanBeNull]
		private T4TokenNodeType GetTokenType()
			=> (T4TokenNodeType) BuilderLexer.TokenType;

		/// <summary>Creates a <see cref="IT4File"/>.</summary>
		/// <returns>An instance of <see cref="IT4File"/> containing a T4 tree.</returns>
		[NotNull]
		public IT4File CreateT4Tree()
		{
			BuilderLexer.Start();
			var file = new T4File();

			Parse(file);

			if (SourceFile != null)
			{
				file.SetSourceFile(SourceFile);
				file.DocumentRangeTranslator = new T4DocumentRangeTranslator(file, SourceFile, Includes);
			}

			return file;
		}

		private void Parse([NotNull] CompositeElement parentElement)
		{
			var tokenNodeType = GetNonCodeBlockTokenType(parentElement);
			while (tokenNodeType != null)
			{
				if (tokenNodeType == T4TokenNodeTypes.DIRECTIVE_START)
					ParseDirective(parentElement);
				else
				{
					AppendNewChild(parentElement, tokenNodeType);
					Advance();
				}

				tokenNodeType = GetNonCodeBlockTokenType(parentElement);
			}

			if (BuilderLexer.HasSkippedTokens)
				BuilderLexer.AppendSkippedTokens(parentElement);
			if (NotClosedDirectives != null)
				FixTopLevelSpace(parentElement, NotClosedDirectives);
		}

		/// <summary>Appends an error element where a token of a given type was expected.</summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="missingTokenType">The missing token type.</param>
		internal void AppendMissingToken([NotNull] CompositeElement parentElement, MissingTokenType missingTokenType)
			=> AppendNewChild(parentElement, new MissingTokenErrorElement(missingTokenType));

		/// <summary>Gets the first token that is not a code block related token.</summary>
		/// <param name="parentElement">The parent element where the potential code block will be appended as a child.</param>
		/// <returns>A <see cref="T4TokenNodeType"/>, or <c>null</c>.</returns>
		[CanBeNull]
		private T4TokenNodeType GetNonCodeBlockTokenType(CompositeElement parentElement)
		{
			var tokenNodeType = GetTokenType();
			while (TryParseCodeBlock(tokenNodeType, parentElement))
			{
				tokenNodeType = Advance();
			}

			return tokenNodeType;
		}

		/// <summary>Checks if the current token represents the beginning of a code block, if yes, parse every code block after the token.</summary>
		/// <param name="tokenNodeType">The current token type.</param>
		/// <param name="parentElement">The parent element where the potential code block will be appended as a child.</param>
		/// <returns><c>true</c> is a code block has been parsed, <c>false</c> otherwise.</returns>
		private bool TryParseCodeBlock([CanBeNull] T4TokenNodeType tokenNodeType,
			[NotNull] CompositeElement parentElement)
		{
			if (tokenNodeType != null)
			{
				T4CodeBlock codeBlock;
				if (tokenNodeType == T4TokenNodeTypes.STATEMENT_BLOCK_START)
					codeBlock = new T4StatementBlock();
				else if (tokenNodeType == T4TokenNodeTypes.EXPRESSION_BLOCK_START)
					codeBlock = new T4ExpressionBlock();
				else if (tokenNodeType == T4TokenNodeTypes.FEATURE_BLOCK_START)
					codeBlock = new T4FeatureBlock();
				else
					codeBlock = null;

				if (codeBlock != null)
				{
					AppendNewChild(parentElement, ParseCodeBlock(tokenNodeType, codeBlock));
					return true;
				}
			}

			return false;
		}

		/// <summary>Appends a new composite element to the tree.</summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="childElement">The child element.</param>
		internal void AppendNewChild(CompositeElement parentElement, TreeElement childElement)
			=> BuilderLexer.AppendNewChild(parentElement, childElement);

		internal TreeElement CreateAttributeValue(FrugalLocalList<TreeElement> elements)
		{
			var attributeValue = new T4AttributeValue();
			foreach (var element in elements)
			{
				BuilderLexer.AppendNewChild(attributeValue, element);
			}

			return attributeValue;
		}

		/// <summary>Creates and appends a new token to the tree.</summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="tokenNodeType">Type of the token node to create and add.</param>
		private void AppendNewChild(
			[NotNull] CompositeElement parentElement,
			[NotNull] T4TokenNodeType tokenNodeType
		) => BuilderLexer.AppendNewChild(parentElement, CreateNewChild(tokenNodeType));

		private LeafElementBase CreateNewChild([NotNull] T4TokenNodeType tokenNodeType) => tokenNodeType.Create(
			BuilderLexer.Buffer,
			new TreeOffset(BuilderLexer.TokenStart),
			new TreeOffset(BuilderLexer.TokenEnd)
		);

		[NotNull]
		internal LeafElementBase CreateCurrentToken()
		{
			var tokenType = GetTokenType();
			Assertion.AssertNotNull(tokenType, "tokenType == null");

			return tokenType.Create(
				BuilderLexer.Buffer,
				new TreeOffset(BuilderLexer.TokenStart),
				new TreeOffset(BuilderLexer.TokenEnd)
			);
		}

		/// <summary>Parses a T4 code block.</summary>
		/// <param name="codeStartTokenNodeType">The statement start token type.</param>
		/// <param name="codeBlock">An empty code block that will contain the parsed code block.</param>
		[NotNull]
		private T4CodeBlock ParseCodeBlock(
			[NotNull] T4TokenNodeType codeStartTokenNodeType,
			[NotNull] T4CodeBlock codeBlock
		)
		{
			// appends the code start token (<#/<#+/<#=) to the block
			AppendNewChild(codeBlock, codeStartTokenNodeType);

			// parse every RAW_CODE token until a BLOCK_END (that is also appended) or EOF is reached
			var code = new T4Code();
			while (true)
			{
				var nextTokenType = Advance();
				if (nextTokenType == null)
				{
					AppendNewChild(codeBlock, code);
					AppendMissingToken(codeBlock, MissingTokenType.BlockEnd);
					break;
				}

				if (nextTokenType == T4TokenNodeTypes.RAW_CODE)
				{
					AppendNewChild(code, T4TokenNodeTypes.RAW_CODE);
					continue;
				}

				if (nextTokenType == T4TokenNodeTypes.BLOCK_END)
				{
					AppendNewChild(codeBlock, code);
					AppendNewChild(codeBlock, T4TokenNodeTypes.BLOCK_END);
					break;
				}
			}
			return codeBlock;
		}

		private void ParseDirective([NotNull] CompositeElement parentElement)
		{
			var directive = new T4Directive();

			// appends the directive start token (<#@)
			AppendNewChild(directive, T4TokenNodeTypes.DIRECTIVE_START);
			Advance();

			// builds the directive (name and attributes)
			var directiveBuilder = new DirectiveBuilder(this);
			var tokenType = GetTokenType();
			while (tokenType?.IsTag == false)
			{
				if (tokenType == T4TokenNodeTypes.TOKEN)
					directiveBuilder.AddName();
				else if (tokenType == T4TokenNodeTypes.EQUAL)
					directiveBuilder.AddEqual();
				else if (tokenType == T4TokenNodeTypes.QUOTE)
					directiveBuilder.AddQuote();
				else if (tokenType == T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE)
					directiveBuilder.AddValue();
				tokenType = Advance();
			}

			directiveBuilder.Complete(directive);

			// appends the block end token if available
			if (tokenType == T4TokenNodeTypes.BLOCK_END)
			{
				AppendNewChild(directive, T4TokenNodeTypes.BLOCK_END);
				Advance();
			}
			else
			{
				AppendMissingToken(directive, MissingTokenType.BlockEnd);
				if (NotClosedDirectives == null)
					NotClosedDirectives = new List<T4Directive>();
				NotClosedDirectives.Add(directive);
			}

			AppendNewChild(parentElement, directive);

			// checks if we're including a file
			if (directive.IsSpecificDirective(DirectiveInfoManager.Include))
				HandleIncludeDirective(directive, parentElement);
		}

		private void HandleIncludeDirective([NotNull] IT4Directive directive, [NotNull] CompositeElement parentElement)
		{
			if (!(directive.GetAttribute(DirectiveInfoManager.Include.FileAttribute.Name) is T4DirectiveAttribute
				fileAttr))
			{
				return;
			}

			var valueToken = fileAttr.GetValueToken();
			if (valueToken == null)
			{
				return;
			}

			bool once = false;
			if (Environment.ShouldSupportOnceAttribute)
			{
				string onceString = directive.GetAttributeValue(DirectiveInfoManager.Include.OnceAttribute.Name);
				once = bool.TrueString.Equals(onceString, StringComparison.OrdinalIgnoreCase);
			}

			HandleInclude(valueToken.GetText(), fileAttr, parentElement, once);
		}

		private void HandleInclude(
			[CanBeNull] string includeFileName,
			[NotNull] T4DirectiveAttribute fileAttr,
			[NotNull] CompositeElement parentElement,
			bool once
		)
		{
			var path = CreateIncludePath(includeFileName);
			if (AnalyzeInclude(fileAttr, path, once)) return;
			var include = new T4Include {Path = path};
			Includes.Add(include);
			// do not use AppendNewChild, we don't want the PsiBuilderLexer to move line breaks from the include into the main file.
			parentElement.AddChild(include);
		}

		// TODO: move to problem analyzer
		/// <summary>
		/// Checks whether include contains problems
		/// </summary>
		/// <returns>Whether any problems found</returns>
		private bool AnalyzeInclude(
			[NotNull] T4DirectiveAttribute fileAttr,
			[NotNull] IT4PathWithMacros path,
			bool once
		)
		{
			if (path.IsEmpty)
			{
				fileAttr.ValueError = $@"Unresolved file ""{path}""";
				return true;
			}

			fileAttr.Reference = path;
			if (!ExistingIncludePaths.Add(path))
			{
				if (!once) fileAttr.ValueError = $@"Already included file ""{path}""";
				return true;
			}

			if (path.ResolvePath() == SourceFile.GetLocation())
			{
				fileAttr.ValueError = "Recursive include";
				ExistingIncludePaths.Add(path);
				return true;
			}

			if (!path.ResolvePath().ExistsFile)
			{
				fileAttr.ValueError = $@"File ""{path}"" not found";
				return true;
			}

			return false;
		}

		private IT4PathWithMacros CreateIncludePath(
			[CanBeNull] string includeFileName
		)
		{
			if (includeFileName == null) return T4EmptyPathWithMacros.Instance;
			if (SourceFile == null) return T4EmptyPathWithMacros.Instance;
			return new T4PathWithMacros(includeFileName, SourceFile);
		}

		/// <summary>
		/// Unclosed directives may have trailing spaces that are skipped then added at file level by the <see cref="PsiBuilderLexer"/>.
		/// In this T4 parser, space tokens can only appear inside directives so we're putting them back in.
		/// </summary>
		/// <param name="file">The file containing non closed directives.</param>
		/// <param name="notClosedDirectives">The list of directives that aren't closed.</param>
		private static void FixTopLevelSpace([NotNull] CompositeElement file,
			[NotNull] IEnumerable<T4Directive> notClosedDirectives)
		{
			foreach (var directive in notClosedDirectives)
			{
				var potentialSpace = directive.NextSibling;
				if (potentialSpace == null || potentialSpace.GetTokenType() != T4TokenNodeTypes.WHITE_SPACE)
					continue;
				file.DeleteChildRange(potentialSpace, potentialSpace);
				Assertion.Assert(directive.LastChild is IErrorElement, "directive.LastChild is IErrorElement");
				directive.AddChildBefore(potentialSpace, directive.LastChild);
			}
		}
	}
}
