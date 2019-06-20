using System;
using System.IO;
using System.Reflection;
using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi
{
	/// <summary>This class generates a code-behind file from C# embedded statements and directives in the T4 file.</summary>
	internal sealed class T4CSharpCodeGenerator : IRecursiveElementProcessor
	{
		internal const string CodeCommentStart = "/*_T4\x200CCodeStart_*/";
		internal const string CodeCommentEnd = "/*_T4\x200CCodeEnd_*/";
		internal const string ClassName = "Generated\x200CTransformation";
		internal const string TransformTextMethodName = "TransformText";

		internal const string DefaultBaseClassFullName =
			"Microsoft.VisualStudio.TextTemplating." + "TextTransformation";

		[NotNull] private readonly IT4File _file;
		[NotNull] private readonly DirectiveInfoManager _directiveInfoManager;
		[NotNull] private readonly T4CSharpCodeGenerationResult _usingsResult;
		[NotNull] private readonly T4CSharpCodeGenerationResult _parametersResult;
		[NotNull] private readonly T4CSharpCodeGenerationResult _inheritsResult;
		[NotNull] private readonly T4CSharpCodeGenerationResult _transformTextResult;
		[NotNull] private readonly T4CSharpCodeGenerationResult _featureResult;
		[NotNull] private readonly Lazy<string> baseClassDescription;

		private int _includeDepth;
		private bool _rootFeatureStarted;
		private bool _hasHost;

		private bool HasBaseClass => !_inheritsResult.Builder.IsEmpty();

		bool IRecursiveElementProcessor.InteriorShouldBeProcessed(ITreeNode element)
			=> element is IT4CodeBlock
			|| element is IT4Include;

		void IRecursiveElementProcessor.ProcessBeforeInterior(ITreeNode element) {
			if (element is IT4Include)
				++_includeDepth;
		}

		void IRecursiveElementProcessor.ProcessAfterInterior(ITreeNode element) {
			switch (element) {
				case IT4Include _:
					--_includeDepth;
					return;
				case IT4Directive directive:
					HandleDirective(directive);
					return;
				case IT4CodeBlock codeBlock:
					HandleCodeBlock(codeBlock);
					break;
			}
		}

		bool IRecursiveElementProcessor.ProcessingIsFinished {
			get {
				InterruptableActivityCookie.CheckAndThrow();
				return false;
			}
		}

		/// <summary>Handles a directive in the tree.</summary>
		/// <param name="directive">The directive.</param>
		private void HandleDirective([NotNull] IT4Directive directive) {
			if (directive.IsSpecificDirective(_directiveInfoManager.Import))
				HandleImportDirective(directive);
			else if (directive.IsSpecificDirective(_directiveInfoManager.Template))
				HandleTemplateDirective(directive);
			else if (directive.IsSpecificDirective(_directiveInfoManager.Parameter))
				HandleParameterDirective(directive);
		}

		/// <summary>Handles an import directive, equivalent of an using directive in C#.</summary>
		/// <param name="directive">The import directive.</param>
		private void HandleImportDirective([NotNull] IT4Directive directive) {
			Pair<IT4Token, string> ns = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Import.NamespaceAttribute.Name);
			if (ns.First == null || ns.Second == null)
				return;

			_usingsResult.Builder.Append("using ");
			_usingsResult.AppendMapped(ns.Second, ns.First.GetTreeTextRange());
			_usingsResult.Builder.AppendLine(";");
		}

		/// <summary>Handles a template directive, determining if we should output a Host property and use a base class.</summary>
		/// <param name="directive">The template directive.</param>
		private void HandleTemplateDirective([NotNull] IT4Directive directive) {
			string value = directive.GetAttributeValue(_directiveInfoManager.Template.HostSpecificAttribute.Name);
			_hasHost = Boolean.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);

			(IT4Token classNameToken, string className) = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Template.InheritsAttribute.Name);
			if (classNameToken != null && className != null)
				_inheritsResult.AppendMapped(className, classNameToken.GetTreeTextRange());
		}

		/// <summary>Handles a parameter directive, outputting an extra property.</summary>
		/// <param name="directive">The parameter directive.</param>
		private void HandleParameterDirective([NotNull] IT4Directive directive) {
			(IT4Token typeToken, string type) = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Parameter.TypeAttribute.Name);
			if (typeToken == null || type == null)
				return;

			(IT4Token nameToken, string name) = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Parameter.NameAttribute.Name);
			if (nameToken == null || name == null)
				return;
			
			StringBuilder builder = _parametersResult.Builder;
			builder.Append("[System.CodeDom.Compiler.GeneratedCodeAttribute] private global::");
			_parametersResult.AppendMapped(type, typeToken.GetTreeTextRange());
			builder.Append(' ');
			_parametersResult.AppendMapped(name, nameToken.GetTreeTextRange());
			builder.Append(" { get { return default(global::");
			builder.Append(type);
			builder.AppendLine("); } }");
		}

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
				result = _rootFeatureStarted && _includeDepth == 0 ? _featureResult : _transformTextResult;
				result.Builder.Append("this.Write(__\x200CToString(");
			}
			else {
				if (codeBlock is T4FeatureBlock) {
					if (_includeDepth == 0)
						_rootFeatureStarted = true;
					result = _featureResult;
				}
				else
					result = _transformTextResult;
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
			IPsiSourceFile sourceFile = _file.GetSourceFile();
			IProjectFile projectFile = sourceFile?.ToProjectFile();
			if (projectFile == null || !projectFile.IsPreprocessedT4Template())
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
			_file.ProcessDescendants(this);
			var result = new T4CSharpCodeGenerationResult(_file);
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
			result.Append(_usingsResult);
			
			AppendBaseClass(result.Builder);
			AppendClass(result);
		}

		private void AppendClass(T4CSharpCodeGenerationResult result)
		{
			result.Builder.AppendLine($"[{SyntheticAttribute.Name}]");
			result.Builder.Append($"public class {ClassName} : ");
			AppendBaseClassName(result);
			result.Builder.AppendLine();
			result.Builder.AppendLine("{");
			result.Builder.AppendLine($"[{SyntheticAttribute.Name}]");
			result.Builder.AppendLine($"private static string __\x200CToString(object value) {{ return null; }}");
			if (_hasHost)
				result.Builder.AppendLine("public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
			result.Append(_parametersResult);
			result.Builder.AppendLine($"[System.CodeDom.Compiler.GeneratedCodeAttribute(\"Rider\", \"whatever\")] public override string {TransformTextMethodName}() {{");
			result.Append(_transformTextResult);
			result.Builder.AppendLine();
			result.Builder.AppendLine("return GenerationEnvironment.ToString();");
			result.Builder.AppendLine("}");
			result.Append(_featureResult);
			result.Builder.AppendLine("}");
		}

		private void AppendBaseClassName(T4CSharpCodeGenerationResult result)
		{
			if (HasBaseClass)
			{
				result.Append(_inheritsResult);
			}
			else
			{
				result.Builder.Append("TextTransformation");
			}
		}

		private string ReadBaseClass()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			const string name = "GammaJul.ForTea.Core.TextTransformation.cs";
			// ReSharper disable once AssignNullToNotNullAttribute
			using (Stream stream = assembly.GetManifestResourceStream(name))
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
		
		private void AppendBaseClass([NotNull] StringBuilder builder)
		{
			if (HasBaseClass) return;
			builder.AppendLine(baseClassDescription.Value);
		}

		/// <summary>Initializes a new instance of the <see cref="T4CSharpCodeGenerator"/> class.</summary>
		/// <param name="file">The associated T4 file whose C# code behind will be generated.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		public T4CSharpCodeGenerator([NotNull] IT4File file, [NotNull] DirectiveInfoManager directiveInfoManager) {
			_file = file;
			_directiveInfoManager = directiveInfoManager;
			_usingsResult = new T4CSharpCodeGenerationResult(file);
			_parametersResult = new T4CSharpCodeGenerationResult(file);
			_inheritsResult = new T4CSharpCodeGenerationResult(file);
			_transformTextResult = new T4CSharpCodeGenerationResult(file);
			_featureResult = new T4CSharpCodeGenerationResult(file);
			baseClassDescription = Lazy.Of(ReadBaseClass, false);
		}

	}

}