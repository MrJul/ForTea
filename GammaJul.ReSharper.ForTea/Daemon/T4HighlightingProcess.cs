using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>
	/// Process that highlights block tags and missing token errors.
	/// </summary>
	internal sealed class T4HighlightingProcess : T4DaemonStageProcess {
		
		/// <summary>
		/// Processes a node, before its descendants are processed.
		/// </summary>
		/// <param name="element">The node to process.</param>
		public override void ProcessBeforeInterior(ITreeNode element) {
			string attributeId = GetHighlightingAttributeId(element);
			if (attributeId != null)
				AddHighlighting(new HighlightingInfo(element.GetHighlightingRange(), new PredefinedHighlighting(attributeId)));
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element) {
			var tokenType = element.GetTokenType() as T4TokenNodeType;
			if (tokenType == null)
				return null;
			
			/*TODO: reenable; maybe make a real VS language service only for T4 coloring (leaving the C# to ReSharper)?
			 * currently the background flickers on each keystroke */
			/*if (tokenType == T4TokenNodeTypes.Code)
				AddHighlighting(new HighlightingInfo(element.GetHighlightingRange(), T4CodeHighlighting.Instance));
			else*/
			if (tokenType.IsTag)
				return VsPredefinedHighlighterIds.HtmlServerSideScript;
			if (tokenType == T4TokenNodeTypes.Equal)
				return VsPredefinedHighlighterIds.HtmlOperator;
			if (tokenType == T4TokenNodeTypes.Quote || tokenType == T4TokenNodeTypes.Value)
				return VsPredefinedHighlighterIds.HtmlAttributeValue;
			if (tokenType == T4TokenNodeTypes.Name) {
				if (element.Parent is IT4Directive)
					return VsPredefinedHighlighterIds.HtmlElementName;
				return VsPredefinedHighlighterIds.HtmlAttributeName;
			}
			return null;
		}
		
		internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
			: base(file, daemonProcess) {
		}

	}

}