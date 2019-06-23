using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
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

		private Stack<T4CSharpCodeGenerationIntermediateResult> Results { get; }
		private bool HasSeenTemplateDirective { get; set; }

		[NotNull]
		private T4CSharpCodeGenerationIntermediateResult Result => Results.Peek();

		private bool IsAtRootLevel => Results.Count == 1; // TODO: write a better version
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

		public T4CSharpCodeGenerationIntermediateResult Collect()
		{
			Results.Push(new T4CSharpCodeGenerationIntermediateResult(File));
			File.ProcessDescendants(this);
			return Results.Pop();
		}

		#region Interface Members
		public bool InteriorShouldBeProcessed(ITreeNode element) => element is IT4Include;

		public void ProcessBeforeInterior(ITreeNode element)
		{
			if (element is IT4Include)
			{
				Results.Push(new T4CSharpCodeGenerationIntermediateResult(File));
			}
		}

		public void ProcessAfterInterior(ITreeNode element)
		{
			switch (element)
			{
				case IT4Include _:
					var intermediateResults = Results.Pop();
					Result.Append(intermediateResults);
					break;
				case IT4Directive directive:
					HandleDirective(directive);
					break;
				case IT4CodeBlock codeBlock:
					HandleCodeBlock(codeBlock);
					break;
				case IT4Token token:
					AppendToken(Result, token);
					break;
			}
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
					result.Builder.AppendLine();
					break;
				case T4FeatureBlock _:
					if (IsAtRootLevel)
						Result.StartFeature();
					AppendCode(Result.CollectedFeatures, codeToken);
					Result.CollectedFeatures.Builder.AppendLine();
					break;
				default:
					AppendCode(Result.CollectedTransformation, codeToken);
					Result.CollectedTransformation.Builder.AppendLine();
					break;
			}
		}

		/// <summary>Handles an import directive, equivalent of an using directive in C#.</summary>
		/// <param name="directive">The import directive.</param>
		private void HandleImportDirective([NotNull] IT4Directive directive)
		{
			Pair<IT4Token, string> ns =
				directive.GetAttributeValueIgnoreOnlyWhitespace(Manager.Import.NamespaceAttribute.Name);

			if (ns.First == null || ns.Second == null)
				return;

			Result.CollectedImports.Builder.Append("using ");
			Result.CollectedImports.AppendMapped(ns.Second, ns.First.GetTreeTextRange());
			Result.CollectedImports.Builder.AppendLine(";");
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

			(IT4Token classNameToken, string className) =
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
		#endregion Utils

		private void AppendExpressionWriting(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] IT4Token token
		)
		{
			var builder = result.Builder;
			builder.Append("            this.Write(");
			builder.Append(ToStringConversionStart);
			AppendCode(result, token);
			builder.Append(ToStringConversionEnd);
			builder.Append(");");
		}

		protected abstract void AppendToken(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4Token token);

		[NotNull]
		protected abstract string ToStringConversionStart { get; }

		[NotNull]
		protected virtual string ToStringConversionEnd => ")";

		protected abstract void AppendCode(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] IT4Token token);
	}
}
