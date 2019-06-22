using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
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

		[NotNull]
		public T4CSharpCodeGenerationResult UsingsResult { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult InheritsResult { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult TransformTextResult { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult FeatureResult { get; }

		[NotNull, ItemNotNull]
		private List<T4ParameterDescription> MyParameterDescriptions { get; }

		private bool HasSeenTemplateDirective { get; set; }

		[NotNull, ItemNotNull]
		public IReadOnlyCollection<T4ParameterDescription> ParameterDescriptions => MyParameterDescriptions;

		private int IncludeDepth { get; set; }
		private bool RootFeatureStarted { get; set; }
		public bool HasHost { get; private set; }
		#endregion Properties

		protected T4CSharpCodeGenerationInfoCollectorBase(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		)
		{
			File = file;
			UsingsResult = new T4CSharpCodeGenerationResult(file);
			InheritsResult = new T4CSharpCodeGenerationResult(file);
			TransformTextResult = new T4CSharpCodeGenerationResult(file);
			FeatureResult = new T4CSharpCodeGenerationResult(file);
			MyParameterDescriptions = new List<T4ParameterDescription>();
			Manager = manager;
		}

		public void Collect() => File.ProcessDescendants(this);

		#region Interface Members
		public bool InteriorShouldBeProcessed(ITreeNode element) => element is IT4Include;

		public void ProcessBeforeInterior(ITreeNode element)
		{
			if (element is IT4Include)
			{
				IncludeDepth += 1;
			}
		}

		public void ProcessAfterInterior(ITreeNode element)
		{
			switch (element)
			{
				case IT4Include _:
					--IncludeDepth;
					break;
				case IT4Directive directive:
					HandleDirective(directive);
					break;
				case IT4CodeBlock codeBlock:
					HandleCodeBlock(codeBlock);
					break;
				case IT4Token token:
					HandleToken(token);
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
		private void HandleToken([NotNull] IT4Token token)
		{
			var result = RootFeatureStarted ? FeatureResult : TransformTextResult;
			var builder = result.Builder;
			builder.Append("            this.Write(\"");
			builder.Append(StringLiteralConverter.EscapeToRegular(token.GetText()));
			builder.AppendLine("\");");
		}

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
					var result = RootFeatureStarted && IncludeDepth == 0 ? FeatureResult : TransformTextResult;
					AppendExpressionWriting(result, codeToken);
					result.Builder.AppendLine();
					break;
				case T4FeatureBlock _:
					if (IncludeDepth == 0)
						RootFeatureStarted = true;
					AppendCode(FeatureResult, codeToken);
					FeatureResult.Builder.AppendLine();
					break;
				default:
					AppendCode(TransformTextResult, codeToken);
					TransformTextResult.Builder.AppendLine();
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

			UsingsResult.Builder.Append("using ");
			UsingsResult.AppendMapped(ns.Second, ns.First.GetTreeTextRange());
			UsingsResult.Builder.AppendLine(";");
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
			string value = directive.GetAttributeValue(Manager.Template.HostSpecificAttribute.Name);
			HasHost = bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);

			(IT4Token classNameToken, string className) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(Manager.Template.InheritsAttribute.Name);
			if (classNameToken != null && className != null)
				InheritsResult.AppendMapped(className, classNameToken.GetTreeTextRange());
		}

		/// <summary>Handles a parameter directive, outputting an extra property.</summary>
		/// <param name="directive">The parameter directive.</param>
		private void HandleParameterDirective([NotNull] IT4Directive directive)
		{
			var description = T4ParameterDescription.FromDirective(directive, Manager);
			if (description == null) return;
			MyParameterDescriptions.Add(description);
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

		[NotNull]
		protected abstract string ToStringConversionStart { get; }

		[NotNull]
		protected virtual string ToStringConversionEnd => ")";

		protected abstract void AppendCode(
			[NotNull] T4CSharpCodeGenerationResult result,
			[NotNull] IT4Token token);
	}
}
