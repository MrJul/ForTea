using System;
using System.Text;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	/// <summary>This class generates a code-behind file from C# embedded statements and directives in the T4 file.</summary>
	internal sealed class T4CSharpCodeGenerator : IRecursiveElementProcessor
	{
		internal const string CodeCommentStart = "/*_T4\x200CCodeStart_*/";
		internal const string CodeCommentEnd = "/*_T4\x200CCodeEnd_*/";
		internal const string ClassName = "Generated\x200CTransformation";
		internal const string TransformTextMethodName = "TransformText";

		internal const string DefaultBaseClassFullName =
			"Microsoft.VisualStudio.TextTemplating." + T4TemplateBaseProvider.DefaultBaseClassName;

		[NotNull]
		private IT4File File { get; }

		[NotNull]
		private DirectiveInfoManager DirectiveInfoManager { get; }

		[NotNull]
		private T4CSharpCodeGenerationResult UsingsResult { get; }

		[NotNull]
		private T4CSharpCodeGenerationResult ParametersResult { get; }

		[NotNull]
		private T4CSharpCodeGenerationResult InheritsResult { get; }

		[NotNull]
		private T4CSharpCodeGenerationResult TransformTextResult { get; }

		[NotNull]
		private T4CSharpCodeGenerationResult FeatureResult { get; }

		[NotNull]
		private T4TemplateBaseProvider Provider { get; }
		
		private bool ShouldInsertSyntheticAttributes { get; }

		private int IncludeDepth { get; set; }
		private bool RootFeatureStarted { get; set; }
		private bool HasHost { get; set; }
		private bool HasBaseClass => !InheritsResult.Builder.IsEmpty();

		bool IRecursiveElementProcessor.InteriorShouldBeProcessed(ITreeNode element) =>
			element is IT4CodeBlock || element is IT4Include;

		void IRecursiveElementProcessor.ProcessBeforeInterior(ITreeNode element) {
			if (element is IT4Include)
				++IncludeDepth;
		}

		void IRecursiveElementProcessor.ProcessAfterInterior(ITreeNode element) {
			switch (element) {
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

		bool IRecursiveElementProcessor.ProcessingIsFinished {
			get {
				InterruptableActivityCookie.CheckAndThrow();
				return false;
			}
		}

		#region Directive Handling
		/// <summary>Handles a directive in the tree.</summary>
		/// <param name="directive">The directive.</param>
		private void HandleDirective([NotNull] IT4Directive directive)
		{
			if (directive.IsSpecificDirective(DirectiveInfoManager.Import))
				HandleImportDirective(directive);
			else if (directive.IsSpecificDirective(DirectiveInfoManager.Template))
				HandleTemplateDirective(directive);
			else if (directive.IsSpecificDirective(DirectiveInfoManager.Parameter))
				HandleParameterDirective(directive);
		}

		/// <summary>Handles an import directive, equivalent of an using directive in C#.</summary>
		/// <param name="directive">The import directive.</param>
		private void HandleImportDirective([NotNull] IT4Directive directive)
		{
			Pair<IT4Token, string> ns =
				directive.GetAttributeValueIgnoreOnlyWhitespace(DirectiveInfoManager.Import.NamespaceAttribute.Name);

			if (ns.First == null || ns.Second == null)
				return;

			UsingsResult.Builder.Append("using ");
			UsingsResult.AppendMapped(ns.Second, ns.First.GetTreeTextRange());
			UsingsResult.Builder.AppendLine(";");
		}

		/// <summary>Handles a template directive, determining if we should output a Host property and use a base class.</summary>
		/// <param name="directive">The template directive.</param>
		private void HandleTemplateDirective([NotNull] IT4Directive directive)
		{
			string value = directive.GetAttributeValue(DirectiveInfoManager.Template.HostSpecificAttribute.Name);
			HasHost = Boolean.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);

			(IT4Token classNameToken, string className) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(DirectiveInfoManager.Template.InheritsAttribute.Name);
			if (classNameToken != null && className != null)
				InheritsResult.AppendMapped(className, classNameToken.GetTreeTextRange());
		}

		/// <summary>Handles a parameter directive, outputting an extra property.</summary>
		/// <param name="directive">The parameter directive.</param>
		private void HandleParameterDirective([NotNull] IT4Directive directive)
		{
			(IT4Token typeToken, string type) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(DirectiveInfoManager.Parameter.TypeAttribute.Name);

			if (typeToken == null || type == null)
				return;

			(IT4Token nameToken, string name) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(DirectiveInfoManager.Parameter.NameAttribute.Name);

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
		#endregion Directive Handling
		
		/// <summary>
		/// Handles a code block: depending of whether it's a feature or transform text result,
		/// it is not added to the same part of the C# file.
		/// </summary>
		/// <param name="codeBlock">The code block.</param>
		private void HandleCodeBlock([NotNull] IT4CodeBlock codeBlock) {
			IT4Token codeToken = codeBlock.GetCodeToken();
			if (codeToken == null)
				return;

			T4CSharpCodeGenerationResult result;
			var expressionBlock = codeBlock as T4ExpressionBlock;

			if (expressionBlock != null) {
				result = RootFeatureStarted && IncludeDepth == 0 ? FeatureResult : TransformTextResult;
				result.Builder.Append("this.Write(__\x200CToString(");
			}
			else {
				if (codeBlock is T4FeatureBlock) {
					if (IncludeDepth == 0)
						RootFeatureStarted = true;
					result = FeatureResult;
				}
				else
					result = TransformTextResult;
			}

			result.Builder.Append(CodeCommentStart);
			result.AppendMapped(codeToken);
			result.Builder.Append(CodeCommentEnd);

			if (expressionBlock != null)
				result.Builder.Append("));");
			result.Builder.AppendLine();
		}

		/// <summary>Gets the namespace of the current T4 file. This is always <c>null</c> for a standard (non-preprocessed) file.</summary>
		/// <returns>A namespace, or <c>null</c>.</returns>
		[CanBeNull]
		private string GetNamespace() {
			IPsiSourceFile sourceFile = File.GetSourceFile();
			IProjectFile projectFile = sourceFile?.ToProjectFile();
			if (projectFile?.IsPreprocessedT4Template() != true)
				return null;

			string ns = projectFile.GetCustomToolNamespace();
			if (!String.IsNullOrEmpty(ns))
				return ns;

			return sourceFile.Properties.GetDefaultNamespace();
		}

		/// <summary>Generates a new C# code behind.</summary>
		/// <returns>An instance of <see cref="T4CSharpCodeGenerationResult"/> containing the C# file.</returns>
		[NotNull]
		public T4CSharpCodeGenerationResult Generate() {
			File.ProcessDescendants(this);
			var result = new T4CSharpCodeGenerationResult(File);
			string ns = GetNamespace();
			bool hasNamespace = !string.IsNullOrEmpty(ns);
			if (hasNamespace)
			{
				result.Builder.AppendLine($"namespace {ns} {{");
				AppendNamespaceContents(result);
				result.Builder.AppendLine("}");
			}
			else
			{
				AppendNamespaceContents(result);
			}

			return result;
		}

		private void AppendNamespaceContents(T4CSharpCodeGenerationResult result)
		{
			result.Builder.AppendLine("using System;");
			result.Append(UsingsResult);
			AppendClass(result);
			AppendBaseClass(result.Builder);
		}

		private void AppendSyntheticAttribute([NotNull] StringBuilder builder)
		{
			if (!ShouldInsertSyntheticAttributes) return;
			builder.AppendLine($"[{SyntheticAttribute.Name}]");
		}

		private void AppendClass(T4CSharpCodeGenerationResult result)
		{
			AppendSyntheticAttribute(result.Builder);
			result.Builder.Append($"public class {ClassName} : ");
			AppendBaseClassName(result);
			result.Builder.AppendLine();
			result.Builder.AppendLine("{");
			if (HasHost)
				result.Builder.AppendLine("public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
			result.Append(ParametersResult);
			result.Builder.AppendLine($"[System.CodeDom.Compiler.GeneratedCodeAttribute(\"Rider\", \"whatever\")] public override string {TransformTextMethodName}() {{");
			result.Append(TransformTextResult);
			result.Builder.AppendLine();
			result.Builder.AppendLine("return GenerationEnvironment.ToString();");
			result.Builder.AppendLine("}");
			result.Append(FeatureResult);
			result.Builder.AppendLine("}");
		}

		private void AppendBaseClassName(T4CSharpCodeGenerationResult result)
		{
			if (HasBaseClass)
			{
				result.Append(InheritsResult);
			}
			else
			{
				result.Builder.Append(T4TemplateBaseProvider.DefaultBaseClassName);
			}
		}

		private void AppendBaseClass([NotNull] StringBuilder builder)
		{
			if (HasBaseClass) return;
			builder.AppendLine(Provider.CreateTemplateBase());
		}

		/// <summary>Initializes a new instance of the <see cref="T4CSharpCodeGenerator"/> class.</summary>
		/// <param name="file">The associated T4 file whose C# code behind will be generated.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="Psi.Directives.DirectiveInfoManager"/>.</param>
		/// <param name="provider">Base provider service</param>
		/// <param name="shouldInsertSyntheticAttributes">
		/// Whether there should be [__ReSharperSynthetic] attributes in generated code
		/// </param>
		public T4CSharpCodeGenerator(
			[NotNull] IT4File file,
			[NotNull] DirectiveInfoManager directiveInfoManager,
			[NotNull] T4TemplateBaseProvider provider,
			bool shouldInsertSyntheticAttributes
		)
		{
			File = file;
			DirectiveInfoManager = directiveInfoManager;
			UsingsResult = new T4CSharpCodeGenerationResult(file);
			ParametersResult = new T4CSharpCodeGenerationResult(file);
			InheritsResult = new T4CSharpCodeGenerationResult(file);
			TransformTextResult = new T4CSharpCodeGenerationResult(file);
			FeatureResult = new T4CSharpCodeGenerationResult(file);
			Provider = provider;
			ShouldInsertSyntheticAttributes = shouldInsertSyntheticAttributes;
		}

	}

}