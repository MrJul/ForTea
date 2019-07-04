using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Processes {

	internal sealed class T4ErrorProcess : T4DaemonStageProcessBase {

		[NotNull] private readonly T4DirectiveInfoManager _directiveInfoManager;

		[CanBeNull] private T4FeatureBlock _lastFeature;
		private bool _gotFeature;
		private bool _gotLastFeature;
		private bool _inLastFeature;
		private bool _afterLastFeatureErrorAdded;

		private static DocumentRange GetMissingTokenRange([NotNull] MissingTokenErrorElement element) {
			DocumentRange range = element.GetDocumentRange();
			range = range.TextRange.EndOffset >= range.Document.GetTextLength()
				? range.ExtendLeft(1)
				: range.ExtendRight(1);
			return range;
		}

		public override void ProcessBeforeInterior(ITreeNode element) {
			AnalyzeUnsupportedLanguage(element);
			
			switch (element) {

				case MissingTokenErrorElement errorElement:
					AddHighlighting(GetMissingTokenRange(errorElement), new MissingTokenHighlighting(errorElement));
					return;

				// can't have a statement block (<# #>) after a feature block (<#+ #>)
				case T4StatementBlock statementBlock when _gotFeature:
					AddHighlighting(element.GetHighlightingRange(), new StatementAfterFeatureHighlighting(statementBlock));
					return;

				case IT4Directive directive:
					ProcessDirective(directive);
					break;

				case T4FeatureBlock _:
					_gotFeature = true;
					if (element == _lastFeature) {
						_gotLastFeature = true;
						_inLastFeature = true;
						return;
					}
					break;
			}

			// verify that a directive attribute value is valid
			if (element.GetTokenType() == T4TokenNodeTypes.Value) {
				ProcessAttributeValue((T4Token) element);
				return;
			}

			// can't have anything after the last feature block
			if (!_gotLastFeature || _inLastFeature || _afterLastFeatureErrorAdded)
				return;

			TokenNodeType tokenType = element.GetTokenType();
			if (tokenType != null && tokenType.IsWhitespace)
				return;

			// highlight from just after the last feature to the end of the document
			DocumentRange range = element.GetHighlightingRange().SetEndTo(File.GetDocumentRange().EndOffset);
			AddHighlighting(range, new AfterLastFeatureHighlighting(element));
			_afterLastFeatureErrorAdded = true;
		}

		private void AnalyzeUnsupportedLanguage([NotNull] ITreeNode element)
		{
			if (element.NodeType != T4TokenNodeTypes.Value) return;
			if (!(element.Parent is IT4DirectiveAttribute attribute)) return;
			if (!(attribute.Parent is T4Directive directive)) return;
			if (!directive.IsSpecificDirective(_directiveInfoManager.Template)) return;
			if (attribute.GetName() != _directiveInfoManager.Template.LanguageAttribute.Name) return;
			if (_directiveInfoManager.GetLanguageType(File).Is<CSharpLanguage>()) return;
			AddHighlighting(element.GetDocumentRange(), new UnsupportedLanguageHighlighting((IT4Token) element));
		}

		private void ProcessAttributeValue([NotNull] T4Token valueNode) {
			if (!(valueNode.Parent is IT4DirectiveAttribute attribute))
				return;

			if (attribute.ValueError != null) {
				AddHighlighting(valueNode.GetHighlightingRange(), new InvalidAttributeValueHighlighting(valueNode, null, attribute.ValueError));
				return;
			}

			if (!(attribute.Parent is IT4Directive directive))
				return;

			DirectiveAttributeInfo attributeInfo = _directiveInfoManager.GetDirectiveByName(directive.GetName())?.GetAttributeByName(attribute.GetName());
			if (attributeInfo == null || attributeInfo.IsValid(valueNode.GetText()))
				return;

			AddHighlighting(valueNode.GetHighlightingRange(), new InvalidAttributeValueHighlighting(valueNode, attributeInfo, "Invalid attribute value"));
		}

		private void ProcessDirective([NotNull] IT4Directive directive) {
			IT4Token nameToken = directive.GetNameToken();
			if (nameToken == null)
				return;

			DirectiveInfo directiveInfo = _directiveInfoManager.GetDirectiveByName(nameToken.GetText());
			if (directiveInfo == null)
				return;

			// Notify of missing required attributes.
			IEnumerable<string> attributeNames = directive.GetAttributes().SelectNotNull(attr => attr.GetName());
			var hashSet = new JetHashSet<string>(attributeNames, StringComparer.OrdinalIgnoreCase);
			foreach (DirectiveAttributeInfo attributeInfo in directiveInfo.SupportedAttributes) {
				if (attributeInfo.IsRequired && !hashSet.Contains(attributeInfo.Name))
					AddHighlighting(nameToken.GetHighlightingRange(), new MissingRequiredAttributeHighlighting(nameToken, attributeInfo.Name));
			}

			// Assembly attributes in preprocessed templates are useless.
			if (directiveInfo == _directiveInfoManager.Assembly && DaemonProcess.SourceFile.ToProjectFile().IsPreprocessedT4Template())
				AddHighlighting(directive.GetHighlightingRange(), new IgnoredAssemblyDirectiveHighlighting(directive));
		}

		public override void ProcessAfterInterior(ITreeNode element) {
			if (element == _lastFeature)
				_inLastFeature = false;
		}

		public override void Execute(Action<DaemonStageResult> commiter) {
			_lastFeature = File.GetFeatureBlocks().LastOrDefault();
			base.Execute(commiter);
		}

		/// <summary>Initializes a new instance of the <see cref="T4DaemonStageProcessBase"/> class.</summary>
		/// <param name="file">The associated T4 file.</param>
		/// <param name="daemonProcess">The associated daemon process.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="T4DirectiveInfoManager"/>.</param>
		public T4ErrorProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess, [NotNull] T4DirectiveInfoManager directiveInfoManager)
			: base(file, daemonProcess) {
			_directiveInfoManager = directiveInfoManager;
		}

	}

}
