using System;
using System.Text;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	class T4CSharpCodeGenerationInfoCollector : IRecursiveElementProcessor
	{
		#region Properties
		[NotNull]
		private IT4File File { get; }

		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult UsingsResult { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult ParametersResult { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult InheritsResult { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult TransformTextResult { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult FeatureResult { get; }

		private int IncludeDepth { get; set; }
		private bool RootFeatureStarted { get; set; }
		public bool HasHost { get; private set; }
		#endregion Properties

		public T4CSharpCodeGenerationInfoCollector([NotNull] IT4File file, T4DirectiveInfoManager manager)
		{
			File = file;
			UsingsResult = new T4CSharpCodeGenerationResult(file);
			ParametersResult = new T4CSharpCodeGenerationResult(file);
			InheritsResult = new T4CSharpCodeGenerationResult(file);
			TransformTextResult = new T4CSharpCodeGenerationResult(file);
			FeatureResult = new T4CSharpCodeGenerationResult(file);
			Manager = manager;
		}

		public void Collect() => File.ProcessDescendants(this);

		#region Interface Members
		public bool InteriorShouldBeProcessed(ITreeNode element) =>
			element is IT4CodeBlock || element is IT4Include;

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
					return;
				case IT4Directive directive:
					HandleDirective(directive);
					return;
				case IT4CodeBlock codeBlock:
					HandleCodeBlock(codeBlock);
					return;
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
			IT4Token codeToken = codeBlock.GetCodeToken();
			if (codeToken == null) return;

			T4CSharpCodeGenerationResult result;
			var expressionBlock = codeBlock as T4ExpressionBlock;

			if (expressionBlock != null)
			{
				result = RootFeatureStarted && IncludeDepth == 0 ? FeatureResult : TransformTextResult;
				result.Builder.Append("            this.Write(this.ToStringHelper.ToStringWithCulture(");
			}
			else
			{
				if (codeBlock is T4FeatureBlock)
				{
					if (IncludeDepth == 0)
						RootFeatureStarted = true;
					result = FeatureResult;
				}
				else
					result = TransformTextResult;
			}

			result.Builder.Append(T4CSharpCodeGeneratorBase.CodeCommentStart);
			result.AppendMapped(codeToken);
			result.Builder.Append(T4CSharpCodeGeneratorBase.CodeCommentEnd);

			if (expressionBlock != null)
				result.Builder.Append("));");
			result.Builder.AppendLine();
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
			var (typeToken, type) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(Manager.Parameter.TypeAttribute.Name);

			if (typeToken == null || type == null)
				return;

			(IT4Token nameToken, string name) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(Manager.Parameter.NameAttribute.Name);

			if (nameToken == null || name == null)
				return;

			StringBuilder builder = ParametersResult.Builder;
			builder.Append("[System.CodeDom.Compiler.GeneratedCodeAttribute] private global::");
			ParametersResult.AppendMapped(type, typeToken.GetTreeTextRange());
			builder.Append(' ');
			ParametersResult.AppendMapped(name, nameToken.GetTreeTextRange());
			builder.Append(" { get { return default(global::");
			builder.Append(type);
			builder.AppendLine("); } }");
		}
		#endregion Utils
	}
}
