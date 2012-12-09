using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {
	
	/// <summary>
	/// Highlights C# keywords, identifiets, etc.
	/// Usually, those are colored by Visual Studio C# language service. However, it's not available in T4 files so we have to it ourselves.
	/// </summary>
	internal class T4CSharpHighlightingProcess : CSharpIncrementalDaemonStageProcessBase {

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
			string attributeId = GetHighlightingAttributeId(node);
			if (attributeId != null)
				context.AddHighlighting(new PredefinedHighlighting(attributeId), node.GetHighlightingRange());
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element) {
			TokenNodeType tokenType = element.GetTokenType();
			if (tokenType == null)
				return null;

			if (tokenType.IsKeyword)
				return VsPredefinedHighlighterIds.Keyword;
			if (tokenType.IsComment)
				return VsPredefinedHighlighterIds.Comment;
			if (tokenType.IsStringLiteral) {
				// TODO: see why this highlighter fails (no MEF classification)
				//if (element.GetText().IndexOf('@') == 0)
				//	return T4CSharpVerbatimStringHighlighting.Instance;
				return VsPredefinedHighlighterIds.String;
			}
			if (tokenType.IsConstantLiteral) {
				if (tokenType == CSharpTokenType.CHARACTER_LITERAL)
					return VsPredefinedHighlighterIds.String;
				return VsPredefinedHighlighterIds.Number;
			}

			if (_csharpOperators[tokenType])
				return VsPredefinedHighlighterIds.Operator;
			
			if (tokenType.IsIdentifier) {
				
				var declaration = element.Parent as ITypeDeclaration;
				if (declaration != null)
					return GetTypeElementHighlightingAttributeId(declaration.DeclaredElement);

				var referenceName = element.Parent as IReferenceName;
				if (referenceName != null) {
					var typeElement = referenceName.Reference.Resolve().DeclaredElement as ITypeElement;
					if (typeElement != null)
						return GetTypeElementHighlightingAttributeId(typeElement);
				}

				return VsPredefinedHighlighterIds.Identifier;
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

		public T4CSharpHighlightingProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, ICSharpFile file)
			: base(process, settingsStore, file) {
		}

	}

}