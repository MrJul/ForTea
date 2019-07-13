using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ForTea.ReSharperPlugin.Daemon.Highlightings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Processes {
	
	/// <summary>
	/// Highlights C# keywords, identifiers, etc.
	/// Usually, those are colored by Visual Studio C# language service. However, it's not available in T4 files so we have to do it ourselves.
	/// </summary>
	internal class T4CSharpHighlightingProcess : CSharpIncrementalDaemonStageProcessBase {

		[NotNull]
		private static readonly NodeTypeSet _csharpOperators = new NodeTypeSet(
			CSharpTokenType.COMMA,
			CSharpTokenType.DOT,
			CSharpTokenType.EQ,
			CSharpTokenType.GT,
			CSharpTokenType.GTGTEQ,
			CSharpTokenType.GTGT,
			CSharpTokenType.LT,
			CSharpTokenType.EXCL,
			CSharpTokenType.TILDE,
			CSharpTokenType.QUEST,
			CSharpTokenType.COLON,
			CSharpTokenType.PLUS,
			CSharpTokenType.MINUS,
			CSharpTokenType.ASTERISK,
			CSharpTokenType.DIV,
			CSharpTokenType.AND,
			CSharpTokenType.OR,
			CSharpTokenType.XOR,
			CSharpTokenType.PERC,
			CSharpTokenType.EQEQ,
			CSharpTokenType.LE,
			CSharpTokenType.GE,
			CSharpTokenType.NE,
			CSharpTokenType.ANDAND,
			CSharpTokenType.OROR,
			CSharpTokenType.PLUSPLUS,
			CSharpTokenType.MINUSMINUS,
			CSharpTokenType.LTLT,
			CSharpTokenType.PLUSEQ,
			CSharpTokenType.MINUSEQ,
			CSharpTokenType.ASTERISKEQ,
			CSharpTokenType.DIVEQ,
			CSharpTokenType.ANDEQ,
			CSharpTokenType.OREQ,
			CSharpTokenType.XOREQ,
			CSharpTokenType.PERCEQ,
			CSharpTokenType.LTLTEQ,
			CSharpTokenType.ARROW,
			CSharpTokenType.LAMBDA_ARROW,
			CSharpTokenType.DOUBLE_COLON,
			CSharpTokenType.DOUBLE_QUEST
		);

		public override void VisitNode(ITreeNode node, IHighlightingConsumer context) {
			base.VisitNode(node, context);
			
			DocumentRange highlightingRange = node.GetHighlightingRange();
			//context.AddHighlighting(new PredefinedHighlighting(VsPredefinedHighlighterIds.RazorCode), highlightingRange);

			string attributeId = GetHighlightingAttributeId(node);
			if (attributeId != null)
				context.AddHighlighting(new PredefinedHighlighting(attributeId, highlightingRange));
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element) {
			TokenNodeType tokenType = element.GetTokenType();
			if (tokenType == null)
				return null;

			if (tokenType.IsKeyword)
				return PredefinedHighlighterIds.Keyword;
			if (tokenType.IsComment)
				return PredefinedHighlighterIds.Comment;
			if (tokenType.IsStringLiteral) {
				// TODO: see why this highlighter fails (no MEF classification)
				//if (element.GetText().IndexOf('@') == 0)
				//	return T4CSharpVerbatimStringHighlighting.Instance;
				return PredefinedHighlighterIds.String;
			}
			if (tokenType.IsConstantLiteral) {
				if (tokenType == CSharpTokenType.CHARACTER_LITERAL)
					return PredefinedHighlighterIds.String;
				return PredefinedHighlighterIds.Number;
			}

			if (_csharpOperators[tokenType])
				return PredefinedHighlighterIds.Operator;
			
			if (tokenType.IsIdentifier) {
				
				if (element.Parent is ITypeDeclaration declaration)
					return GetTypeElementHighlightingAttributeId(declaration.DeclaredElement);

				if ((element.Parent as IReferenceName)?.Reference.Resolve().DeclaredElement is ITypeElement typeElement)
					return GetTypeElementHighlightingAttributeId(typeElement);

				return PredefinedHighlighterIds.Identifier;
			}

			return null;
		}

		[CanBeNull]
		private static string GetTypeElementHighlightingAttributeId([CanBeNull] ITypeElement element) {
			return null;
			// TODO: see why these highlighters fail (no MEF classification)
			//if (element == null)
			//	return null;
			//if (element is IInterface)
			//	return T4CSharpInterfaceHighlighting.Instance;
			//if (element is IStruct)
			//	return T4CSharpStructHighlighting.Instance;
			//if (element is IEnum)
			//	return T4CSharpEnumHighlighting.Instance;
			//if (element is IDelegate)
			//	return T4CSharpDelegateHighlighting.Instance;
			//return T4CSharpTypeHighlighting.Instance;
		}

		public T4CSharpHighlightingProcess(
			[NotNull] IDaemonProcess process,
			[NotNull] IContextBoundSettingsStore settingsStore,
			[NotNull] ICSharpFile file
		)
			: base(process, settingsStore, file) {
		}

	}

}
