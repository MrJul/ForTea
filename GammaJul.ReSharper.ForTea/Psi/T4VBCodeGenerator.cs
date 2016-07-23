#region License

//    Copyright 2012 Julien Lebosquain
//    Copyright 2016 Caelan Sayler - [caelantsayler]at[gmail]com
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
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// This class generates a code-behind file from VB embedded statements and directives in the T4 file.
	/// </summary>
	internal sealed class T4VBCodeGenerator : IRecursiveElementProcessor {

		internal const string CodeCommentStart = "/*_T4\x200CCodeStart_*/";
		internal const string CodeCommentEnd = "/*_T4\x200CCodeEnd_*/";
		internal const string ClassName = "Generated\x200CTransformation";
		internal const string DefaultBaseClassName = "Microsoft.VisualStudio.TextTemplating.TextTransformation";
		internal const string TransformTextMethodName = "TransformText";
		private readonly DirectiveInfoManager _directiveInfoManager;
		private readonly IT4File _file;
		private GenerationResult _featureResult;
		private bool _hasHost;
		private int _includeDepth;
		private GenerationResult _inheritsResult;
		private GenerationResult _parametersResult;
		private bool _rootFeatureStarted;
		private GenerationResult _transformTextResult;
		private GenerationResult _usingsResult;

		/// <summary>
		/// Initializes a new instance of the <see cref="T4VBCodeGenerator" /> class.
		/// </summary>
		/// <param name="file">The associated T4 file whose VB code behind will be generated.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager" />.</param>
		internal T4VBCodeGenerator([NotNull] IT4File file, [NotNull] DirectiveInfoManager directiveInfoManager) {
			_file = file;
			_directiveInfoManager = directiveInfoManager;
		}

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

		bool IRecursiveElementProcessor.ProcessingIsFinished
		{
			get
			{
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
		/// Handles an import directive, equivalent of an using directive in VB.Net.
		/// </summary>
		/// <param name="directive">The import directive.</param>
		private void HandleImportDirective([NotNull] IT4Directive directive) {
			var ns = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Import.NamespaceAttribute.Name);
			if (ns.First == null || ns.Second == null)
				return;

			_usingsResult.Builder.Append("Imports ");
			_usingsResult.AppendMapped(ns.Second, ns.First.GetTreeTextRange());
			_usingsResult.Builder.AppendLine();
		}

		/// <summary>
		/// Handles a template directive, determining if we should output a Host property and use a base class.
		/// </summary>
		/// <param name="directive">The template directive.</param>
		private void HandleTemplateDirective([NotNull] IT4Directive directive) {
			var value = directive.GetAttributeValue(_directiveInfoManager.Template.HostSpecificAttribute.Name);
			_hasHost = bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase);

			var className = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Template.InheritsAttribute.Name);
			if (className.First != null && className.Second != null)
				_inheritsResult.AppendMapped(className.Second, className.First.GetTreeTextRange());
		}

		/// <summary>
		/// Handles a parameter directive, outputting an extra property.
		/// </summary>
		/// <param name="directive">The parameter directive.</param>
		private void HandleParameterDirective([NotNull] IT4Directive directive) {
			var type = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Parameter.TypeAttribute.Name);
			if (type.First == null || type.Second == null)
				return;

			var name = directive.GetAttributeValueIgnoreOnlyWhitespace(_directiveInfoManager.Parameter.NameAttribute.Name);
			if (name.First == null || name.Second == null)
				return;

			var builder = _parametersResult.Builder;
			builder.AppendLine("<System.CodeDom.Compiler.GeneratedCodeAttribute> _");
			builder.Append("Private ReadOnly Property Global.");
			_parametersResult.AppendMapped(name.Second, name.First.GetTreeTextRange()); // name
			builder.Append(" As ");
			_parametersResult.AppendMapped(type.Second, type.First.GetTreeTextRange()); // type
			builder.AppendLine();
			builder.AppendLine("Get");
			builder.Append("Return Nothing"); // closest thing to default() keyword...
			builder.AppendLine("End Get");
			builder.AppendLine("End Property");
		}

		/// <summary>
		/// Handles a code block: depending of whether it's a feature or transform text result,
		/// it is not added to the same part of the VB.Net file.
		/// </summary>
		/// <param name="codeBlock">The code block.</param>
		private void HandleCodeBlock([NotNull] IT4CodeBlock codeBlock) {
			var codeToken = codeBlock.GetCodeToken();
			if (codeToken == null)
				return;

			GenerationResult result;
			var expressionBlock = codeBlock as T4ExpressionBlock;

			if (expressionBlock != null) {
				result = _rootFeatureStarted && _includeDepth == 0 ? _featureResult : _transformTextResult;
				result.Builder.Append("Me.Write(__\x200CToString(");
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
				result.Builder.Append("))");
			result.Builder.AppendLine();
		}

		/// <summary>
		/// Gets the namespace of the current T4 file. This is always <c>null</c> for a standard (non-preprocessed) file.
		/// </summary>
		/// <returns>A namespace, or <c>null</c>.</returns>
		[CanBeNull]
		private string GetNamespace() {
			var sourceFile = _file.GetSourceFile();
			if (sourceFile == null)
				return null;
			var projectFile = sourceFile.ToProjectFile();
			if (projectFile == null || !projectFile.IsPreprocessedT4Template())
				return null;

			var ns = projectFile.GetCustomToolNamespace();
			if (!string.IsNullOrEmpty(ns))
				return ns;

			return sourceFile.Properties.GetDefaultNamespace();
		}

		/// <summary>
		/// Generates a new VB code behind.
		/// </summary>
		/// <returns>An instance of <see cref="GenerationResult" /> containing the VB file.</returns>
		[NotNull]
		internal GenerationResult Generate() {
			_usingsResult = new GenerationResult(_file);
			_parametersResult = new GenerationResult(_file);
			_inheritsResult = new GenerationResult(_file);
			_transformTextResult = new GenerationResult(_file);
			_featureResult = new GenerationResult(_file);
			_file.ProcessDescendants(this);

			var result = new GenerationResult(_file);
			var builder = result.Builder;

			var ns = GetNamespace();
			var hasNamespace = !string.IsNullOrEmpty(ns);
			if (hasNamespace) {
				builder.AppendFormat("Namespace {0} ", ns);
				builder.AppendLine();
			}
			builder.AppendLine("Imports System");
			result.Append(_usingsResult);
			builder.AppendFormat("[{0}]", SyntheticAttribute.Name);
			builder.AppendLine();

			builder.AppendFormat("Public Class {0} {1} Inherits ", ClassName, Environment.NewLine);
			if (_inheritsResult.Builder.Length == 0)
				builder.Append(DefaultBaseClassName);
			else
				result.Append(_inheritsResult);
			builder.AppendLine();
			builder.AppendFormat("<{0}> _", SyntheticAttribute.Name);
			builder.AppendLine();
			builder.AppendLine("Private Shared Function __\x200CToString(value As Object) As String");
			builder.AppendLine("Return Nothing");
			builder.AppendLine("End Function");
			if (_hasHost)
				builder.AppendLine("Public Overridable Property Host As Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost");
			result.Append(_parametersResult);
			builder.AppendLine("<System.CodeDom.Compiler.GeneratedCodeAttribute> _");
			builder.AppendFormat("Public Overrides Function {0}() As String", TransformTextMethodName);
			builder.AppendLine();
			result.Append(_transformTextResult);
			builder.AppendLine();
			builder.AppendLine("Return GenerationEnvironment.ToString()");
			builder.AppendLine("End Function");
			result.Append(_featureResult);
			builder.AppendLine("End Class");
			if (hasNamespace)
				builder.AppendLine("End Namespace");
			return result;
		}

	}

}