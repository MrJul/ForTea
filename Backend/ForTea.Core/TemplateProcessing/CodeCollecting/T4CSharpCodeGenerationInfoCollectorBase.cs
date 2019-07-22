using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Parsing.Builders;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public abstract class T4CSharpCodeGenerationInfoCollectorBase : IRecursiveElementProcessor
	{
		#region Properties
		[NotNull]
		private IT4File File { get; }

		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		[NotNull, ItemNotNull]
		private Stack<T4CSharpCodeGenerationIntermediateResult> Results { get; }

		private bool HasSeenTemplateDirective { get; set; }

		[NotNull]
		protected T4CSharpCodeGenerationIntermediateResult Result => Results.Peek();
		#endregion Properties

		protected T4CSharpCodeGenerationInfoCollectorBase(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		)
		{
			File = file;
			Results = new Stack<T4CSharpCodeGenerationIntermediateResult>();
			Manager = manager;
		}

		[NotNull]
		public T4CSharpCodeGenerationIntermediateResult Collect()
		{
			Results.Push(new T4CSharpCodeGenerationIntermediateResult(File));
			File.ProcessDescendants(this);
			string suffix = Result.State.ProduceBeforeEof();
			if (!suffix.IsNullOrEmpty()) AppendTransformation(suffix);
			return Results.Pop();
		}

		#region Interface Members
		public bool InteriorShouldBeProcessed(ITreeNode element) => element is IT4Include;

		public void ProcessBeforeInterior(ITreeNode element)
		{
			if (!(element is IT4Include include)) return;
			Results.Push(new T4CSharpCodeGenerationIntermediateResult(File));
			var target = include.Path.Resolve();

			if (target?.LanguageType.Is<T4ProjectFileType>() == true)
				target.GetPrimaryPsiFile()?.ProcessDescendants(this);
			else BuildT4Tree(target).ProcessDescendants(this);
		}

		private IT4File BuildT4Tree(IPsiSourceFile target)
		{
			var languageService = T4Language.Instance.LanguageService();
			Assertion.AssertNotNull(languageService, "languageService != null");
			var lexer = languageService.GetPrimaryLexerFactory().CreateLexer(target.Document.Buffer);
			var environment = File.GetSolution().GetComponent<IT4Environment>();
			return new T4TreeBuilder(environment, Manager, lexer, target).CreateT4Tree();
		}

		public void ProcessAfterInterior(ITreeNode element)
		{
			AppendRemainingMessage(element);
			switch (element)
			{
				case IT4Include _:
					string suffix = Result.State.ProduceBeforeEof();
					if (!suffix.IsNullOrEmpty()) AppendTransformation(suffix);
					var intermediateResults = Results.Pop();
					Result.Append(intermediateResults);
					return; // Do not advance state here
				case IT4Directive directive:
					HandleDirective(directive);
					break;
				case IT4CodeBlock codeBlock:
					HandleCodeBlock(codeBlock);
					break;
				case IT4Token token:
					Result.State.ConsumeToken(token);
					break;
			}

			Result.AdvanceState(element);
		}

		public bool ProcessingIsFinished
		{
			get
			{
				InterruptableActivityCookie.CheckAndThrow();
				return false;
			}
		}
		#endregion Interface Members

		#region Utils
		/// <summary>Handles a directive in the tree.</summary>
		/// <param name="directive">The directive.</param>
		private void HandleDirective([NotNull] IT4Directive directive)
		{
			if (directive.IsSpecificDirective(Manager.Import))
				HandleImportDirective(directive);
			else if (directive.IsSpecificDirective(Manager.Template))
				HandleTemplateDirective(directive);
			else if (directive.IsSpecificDirective(Manager.Parameter))
				HandleParameterDirective(directive);
		}

		/// <summary>
		/// Handles a code block: depending of whether it's a feature or transform text result,
		/// it is not added to the same part of the C# file.
		/// </summary>
		/// <param name="codeBlock">The code block.</param>
		private void HandleCodeBlock([NotNull] IT4CodeBlock codeBlock)
		{
			var codeToken = codeBlock.GetCodeToken();
			if (codeToken == null) return;
			switch (codeBlock)
			{
				case T4ExpressionBlock _:
					var result = Result.FeatureStarted
						? Result.CollectedFeatures
						: Result.CollectedTransformation;
					AppendExpressionWriting(result, codeToken);
					result.AppendLine();
					break;
				case T4FeatureBlock _:
					AppendCode(Result.CollectedFeatures, codeToken);
					Result.CollectedFeatures.AppendLine();
					break;
				default:
					AppendCode(Result.CollectedTransformation, codeToken);
					Result.CollectedTransformation.AppendLine();
					break;
			}
		}

		/// <summary>Handles an import directive, equivalent of an using directive in C#.</summary>
		/// <param name="directive">The import directive.</param>
		private void HandleImportDirective([NotNull] IT4Directive directive)
		{
			Pair<ITreeNode, string> ns =
				directive.GetAttributeValueIgnoreOnlyWhitespace(Manager.Import.NamespaceAttribute.Name);

			if (ns.First == null || ns.Second == null)
				return;

			Result.CollectedImports.Append("using ");
			Result.CollectedImports.AppendMapped(ns.Second, ns.First.GetTreeTextRange());
			Result.CollectedImports.AppendLine(";");
		}

		/// <summary>
		/// Handles a template directive,
		/// determining if we should output a Host property
		/// and use a base class.
		/// </summary>
		/// <param name="directive">The template directive.</param>
		private void HandleTemplateDirective([NotNull] IT4Directive directive)
		{
			if (HasSeenTemplateDirective) return;
			HasSeenTemplateDirective = true;
			string hostSpecific = directive.GetAttributeValue(Manager.Template.HostSpecificAttribute.Name);
			if (bool.TrueString.Equals(hostSpecific, StringComparison.OrdinalIgnoreCase)) Result.RequireHost();

			(ITreeNode classNameToken, string className) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(Manager.Template.InheritsAttribute.Name);
			if (classNameToken != null && className != null)
				Result.CollectedBaseClass.AppendMapped(className, classNameToken.GetTreeTextRange());
		}

		/// <summary>Handles a parameter directive, outputting an extra property.</summary>
		/// <param name="directive">The parameter directive.</param>
		private void HandleParameterDirective([NotNull] IT4Directive directive)
		{
			var description = T4ParameterDescription.FromDirective(directive, Manager);
			if (description == null) return;
			Result.Append(description);
		}

		private void AppendExpressionWriting(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] TreeElement token
		)
		{
			result.Append("            this.Write(");
			result.Append(ToStringConversionStart);
			AppendCode(result, token);
			result.Append(ToStringConversionEnd);
			result.Append(");");
		}

		private void AppendRemainingMessage([NotNull] ITreeNode lookahead)
		{
			if (lookahead is IT4Token) return;
			string produced = Result.State.Produce(lookahead);
			if (produced.IsNullOrEmpty()) return;
			// ReSharper disable once AssignNullToNotNullAttribute
			AppendTransformation(produced);
		}
		#endregion Utils

		protected abstract void AppendTransformation([NotNull] string message);

		[NotNull]
		protected abstract string ToStringConversionStart { get; }

		[NotNull]
		protected virtual string ToStringConversionEnd => ")";

		protected abstract void AppendCode(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] TreeElement token);
	}
}
