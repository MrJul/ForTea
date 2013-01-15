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
using System.Text;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// This class generates a code-behind file from C# embedded statements and directive in the T4 file.
	/// </summary>
	internal sealed class T4CSharpCodeGenerator : IRecursiveElementProcessor {

		internal const string CodeCommentStart = "/*_T4\x200CCodeStart_*/";
		internal const string CodeCommentEnd = "/*_T4\x200CCodeEnd_*/";

		private readonly IT4File _file;
		private readonly DirectiveInfoManager _directiveInfoManager;
		private int _includeDepth;
		private bool _rootFeatureStarted;
		private GenerationResult _usingsResult;
		private GenerationResult _parametersResult;
		private GenerationResult _transformTextResult;
		private GenerationResult _featureResult;
		private bool _hasHost;

		bool IRecursiveElementProcessor.InteriorShouldBeProcessed(ITreeNode element) {
			return element is IT4CodeBlock
				|| element is IT4Include;
		}

		void IRecursiveElementProcessor.ProcessBeforeInterior(ITreeNode element) {
			if (element is IT4Include)
				++_includeDepth;
		}

		void IRecursiveElementProcessor.ProcessAfterInterior(ITreeNode element) {
			if (element is IT4Include) {
				--_includeDepth;
				return;
			}

			var directive = element as IT4Directive;
			if (directive != null) {
				HandleDirective(directive);
				return;
			}

			var codeBlock = element as IT4CodeBlock;
			if (codeBlock != null)
				HandleCodeBlock(codeBlock);
		}

		bool IRecursiveElementProcessor.ProcessingIsFinished {
			get {
				InterruptableActivityCookie.CheckAndThrow();
				return false;
			}
		}

		/// <summary>
		/// Handles a directive in the tree.
		/// </summary>
		/// <param name="directive">The directive.</param>
		private void HandleDirective([NotNull] IT4Directive directive) {
			if (directive.IsSpecificDirective(_directiveInfoManager.Import))
				HandleImportDirective(directive);
			else if (directive.IsSpecificDirective(_directiveInfoManager.Template))
				HandleTemplateDirective(directive);
			else if (directive.IsSpecificDirective(_directiveInfoManager.Parameter))
				HandleParameterDirective(directive);
		}

		/// <summary>
		/// Handles an import directive, equivalent of an using directive in C#.
		/// </summary>
		/// <param name="directive">The import directive.</param>
		private void HandleImportDirective([NotNull] IT4Directive directive) {
			Pair<IT4Token, string> ns = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Import.NamespaceAttribute.Name);
			if (ns.First == null || ns.Second == null)
				return;

			_usingsResult.Builder.Append("using ");
			_usingsResult.AppendMapped(ns.Second, ns.First.GetTreeTextRange());
			_usingsResult.Builder.AppendLine(";");
		}

		/// <summary>
		/// Handles a template directive, determining if we should output a Host property.
		/// </summary>
		/// <param name="directive">The template directive.</param>
		private void HandleTemplateDirective([NotNull] IT4Directive directive) {
			string value = directive.GetAttributeValue(_directiveInfoManager.Template.HostSpecificAttribute.Name);
			_hasHost = Boolean.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Handles a parameter directive, outputting an extra property.
		/// </summary>
		/// <param name="directive">The parameter directive.</param>
		private void HandleParameterDirective([NotNull] IT4Directive directive) {
			Pair<IT4Token, string> type = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Parameter.TypeAttribute.Name);
			if (type.First == null || type.Second == null)
				return;

			Pair<IT4Token, string> name = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Parameter.NameAttribute.Name);
			if (name.First == null || name.Second == null)
				return;
			
			StringBuilder builder = _parametersResult.Builder;
			builder.Append("[System.CodeDom.Compiler.GeneratedCodeAttribute] private global::");
			_parametersResult.AppendMapped(type.Second, type.First.GetTreeTextRange());
			builder.Append(' ');
			_parametersResult.AppendMapped(name.Second, name.First.GetTreeTextRange());
			builder.Append(" { get { return default(global::");
			builder.Append(type.Second);
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

			GenerationResult result;
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

		/// <summary>
		/// Gets the namespace of the current T4 file. This is always <c>null</c> for a standard (non-preprocessed) file.
		/// </summary>
		/// <returns>A namespace, or <c>null</c>.</returns>
		[CanBeNull]
		private string GetNamespace() {
			IPsiSourceFile sourceFile = _file.GetSourceFile();
			if (sourceFile == null)
				return null;
			IProjectFile projectFile = sourceFile.ToProjectFile();
			if (projectFile == null || !projectFile.IsPreprocessedT4Template())
				return null;
			
			string ns = projectFile.GetProperties().CustomToolNamespace;
			if (!String.IsNullOrEmpty(ns))
				return ns;

			return sourceFile.Properties.GetDefaultNamespace();
		}

		/// <summary>
		/// Generates a new C# code behind.
		/// </summary>
		/// <returns>An instance of <see cref="GenerationResult"/> containing the C# file.</returns>
		[NotNull]
		internal GenerationResult Generate() {
			_usingsResult = new GenerationResult(_file);
			_parametersResult = new GenerationResult(_file);
			_transformTextResult = new GenerationResult(_file);
			_featureResult = new GenerationResult(_file);
			_file.ProcessDescendants(this);

			var result = new GenerationResult(_file);
			StringBuilder builder = result.Builder;

			string ns = GetNamespace();
			bool hasNamespace = !String.IsNullOrEmpty(ns);
			if (hasNamespace) {
				builder.AppendFormat("namespace {0} {{", ns);
				builder.AppendLine();
			}
			builder.AppendLine("using System;");
			result.Append(_usingsResult);
			builder.AppendFormat("[{0}]", PsiManager.SyntheticAttribute);
			builder.AppendLine();
			builder.AppendLine("public class Generated\x200CTransformation : Microsoft.VisualStudio.TextTemplating.TextTransformation {");
			builder.AppendFormat("[{0}] private static string __\x200CToString(object value) {{ return null; }}", PsiManager.SyntheticAttribute);
			builder.AppendLine();
			if (_hasHost)
				builder.AppendLine("public virtual Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost Host { get; set; }");
			result.Append(_parametersResult);
			builder.AppendLine("[System.CodeDom.Compiler.GeneratedCodeAttribute] public override string TransformText() {");
			result.Append(_transformTextResult);
			builder.AppendLine();
			builder.AppendLine("return GenerationEnvironment.ToString();");
			builder.AppendLine("}");
			result.Append(_featureResult);
			builder.AppendLine("}");
			if (hasNamespace)
				builder.AppendLine("}");
			return result;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4CSharpCodeGenerator"/> class.
		/// </summary>
		/// <param name="file">The associated T4 file whose C# code behind will be generated.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		internal T4CSharpCodeGenerator([NotNull] IT4File file, [NotNull] DirectiveInfoManager directiveInfoManager) {
			_file = file;
			_directiveInfoManager = directiveInfoManager;
		}

	}

}