using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Modules;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
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

		[CanBeNull]
		private ISolution Solution { get; }

		[CanBeNull]
		private IT4FilePsiModule MacroResolveModule { get; }

		[NotNull]
		private HashSet<FileSystemPath> ExistingIncludePaths { get; }

		[CanBeNull]
		private List<T4Directive> NotClosedDirectives { get; set; }
		#endregion

		internal T4TreeBuilder(
			[NotNull] IT4Environment environment,
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile sourceFile = null
		) : this(
			environment,
			directiveInfoManager,
			lexer,
			sourceFile,
			new HashSet<FileSystemPath>(),
			sourceFile?.GetSolution(),
			sourceFile?.PsiModule as IT4FilePsiModule
		)
		{
		}

		private T4TreeBuilder(
			[NotNull] IT4Environment environment,
			[NotNull] T4DirectiveInfoManager directiveInfoManager,
			[NotNull] ILexer lexer,
			[CanBeNull] IPsiSourceFile sourceFile,
			[NotNull] HashSet<FileSystemPath> existingIncludePaths,
			[CanBeNull] ISolution solution,
			[CanBeNull] IT4FilePsiModule macroResolveModule
		)
		{
			Environment = environment;
			DirectiveInfoManager = directiveInfoManager;
			BuilderLexer = new PsiBuilderLexer(lexer, tnt => tnt.IsWhitespace);
			ExistingIncludePaths = existingIncludePaths;
			SourceFile = sourceFile;
			Solution = solution;
			MacroResolveModule = macroResolveModule;
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

		[NotNull]
		private T4Include CreateIncludeT4Tree()
		{
			BuilderLexer.Start();
			var include = new T4Include();
			Parse(include);

			if (SourceFile != null)
				include.DocumentRangeTranslator = new T4DocumentRangeTranslator(include, SourceFile, Includes);

			return include;
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
			var code = new T4Code();
			foreach (var element in elements)
			{
				BuilderLexer.AppendNewChild(code, element);
			}

			return code;
		}

		/// <summary>Creates and appends a new token to the tree.</summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="tokenNodeType">Type of the token node to create and add.</param>
		private void AppendNewChild([NotNull] CompositeElement parentElement, [NotNull] T4TokenNodeType tokenNodeType)
		{
			var token = tokenNodeType.Create(BuilderLexer.Buffer, new TreeOffset(BuilderLexer.TokenStart),
				new TreeOffset(BuilderLexer.TokenEnd));
			BuilderLexer.AppendNewChild(parentElement, token);
		}

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
		private T4CodeBlock ParseCodeBlock([NotNull] T4TokenNodeType codeStartTokenNodeType,
			[NotNull] T4CodeBlock codeBlock)
		{
			// appends the code start token (<#/<#+/<#=) to the block
			AppendNewChild(codeBlock, codeStartTokenNodeType);

			// parse every RAW_CODE token until a BLOCK_END (that is also appended) or EOF is reached
			bool blockEnded = false;
			do
			{
				var nextTokenType = Advance();
				if (nextTokenType == null)
				{
					AppendMissingToken(codeBlock, MissingTokenType.BlockEnd);
					blockEnded = true;
				}
				else
				{
					if (nextTokenType != T4TokenNodeTypes.RAW_CODE)
					{
						if (nextTokenType == T4TokenNodeTypes.BLOCK_END)
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
			var includePath = ResolveInclude(includeFileName);
			if (includePath.IsEmpty)
			{
				fileAttr.ValueError =
					String.Format(CultureInfo.InvariantCulture, "Unresolved file \"{0}\"", includePath);
				return;
			}

			fileAttr.Reference = includePath;

			if (!ExistingIncludePaths.Add(includePath))
			{
				if (!once)
					fileAttr.ValueError = String.Format(CultureInfo.InvariantCulture, "Already included file \"{0}\"",
						includePath);
				return;
			}

			var sourceLocation = SourceFile.GetLocation();
			if (includePath == sourceLocation)
			{
				fileAttr.ValueError = "Recursive include";
				ExistingIncludePaths.Add(includePath);
				return;
			}

			if (!includePath.ExistsFile)
			{
				fileAttr.ValueError =
					String.Format(CultureInfo.InvariantCulture, "File \"{0}\" not found", includePath);
				return;
			}

			// find the matching include in the existing solution source files
			// or create a new one if the include file is outside the solution
			var includeSourceFile =
				includePath.FindSourceFileInSolution(Solution) ?? CreateIncludeSourceFile(includePath);
			if (includeSourceFile == null)
			{
				fileAttr.ValueError = "No current solution";
				return;
			}

			var includeLexer = CreateLexer(includeSourceFile);
			var builder = new T4TreeBuilder(Environment, DirectiveInfoManager, includeLexer, includeSourceFile,
				ExistingIncludePaths, Solution, MacroResolveModule);
			var include = builder.CreateIncludeT4Tree();
			include.Path = includePath;
			Includes.Add(include);

			// do not use AppendNewChild, we don't want the PsiBuilderLexer to move line breaks from the include into the main file.
			parentElement.AddChild(include);
		}

		[CanBeNull]
		private IPsiSourceFile CreateIncludeSourceFile([NotNull] FileSystemPath path)
			=> Solution?.TryGetComponent<T4OutsideSolutionSourceFileManager>()?.GetOrCreateSourceFile(path);

		[NotNull]
		private static ILexer CreateLexer([NotNull] IPsiSourceFile includeSourceFile)
		{
			var languageService = T4Language.Instance.LanguageService();
			Assertion.AssertNotNull(languageService, "languageService != null");
			return languageService.GetPrimaryLexerFactory().CreateLexer(includeSourceFile.Document.Buffer);
		}

		[NotNull]
		private FileSystemPath ResolveInclude([CanBeNull] string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return FileSystemPath.Empty;

			try
			{
				// an include path can contain environment variables and visual studio macros
				fileName = System.Environment.ExpandEnvironmentVariables(fileName);
				fileName = ExpandVisualStudioMacros(fileName);

				// absolute file path, nothing to search for
				var path = FileSystemPath.TryParse(fileName);
				if (path.IsEmpty)
					return FileSystemPath.Empty;
				if (path.IsAbsolute)
					return path;

				// search relative to the current file
				var sourceLocation = SourceFile.GetLocation();
				FileSystemPath currentDirIncludePath;
				if (!sourceLocation.IsEmpty)
				{
					currentDirIncludePath = sourceLocation.Directory.Combine(fileName);
					if (currentDirIncludePath.ExistsFile)
						return currentDirIncludePath;
				}
				else
					currentDirIncludePath = FileSystemPath.Empty;

				// search in global include paths
				return Environment
					       .IncludePaths
					       .Select(includePath => includePath.Combine(fileName))
					       .FirstOrDefault(resultPath => resultPath.ExistsFile)
				       ?? currentDirIncludePath;
			}
			catch (InvalidPathException)
			{
			}
			catch (ArgumentException)
			{
			}

			return FileSystemPath.Empty;
		}

		/// <summary>Expands the Visual Studio macros inside a filename, eg $(SolutionDir).</summary>
		/// <param name="fileName">The file name to expand.</param>
		/// <returns><paramref name="fileName"/> with expanded macros.</returns>
		[NotNull]
		private string ExpandVisualStudioMacros([NotNull] string fileName)
			=> SourceFile != null
				? VsBuildMacroHelper.ResolveMacros(fileName, MacroResolveModule)
				: fileName;

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
