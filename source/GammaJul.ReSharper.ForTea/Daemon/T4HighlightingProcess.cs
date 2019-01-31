using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Parsing;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>Process that highlights block tags and missing token errors.</summary>
	internal sealed class T4HighlightingProcess : T4DaemonStageProcess {
		
		public override void ProcessBeforeInterior(ITreeNode element) {
			string attributeId = GetHighlightingAttributeId(element);
			if (attributeId != null) {
				DocumentRange range = element.GetHighlightingRange();
				AddHighlighting(range, new PredefinedHighlighting(attributeId, range));
			}
		}

		[CanBeNull]
		private static string GetHighlightingAttributeId([NotNull] ITreeNode element) {
			if (!(element.GetTokenType() is T4TokenNodeType tokenType))
				return null;
			
			if (tokenType.IsTag)
				return VsPredefinedHighlighterIds.HtmlServerSideScript;

			if (tokenType == T4TokenNodeTypes.Equal)
				return VsPredefinedHighlighterIds.HtmlOperator;

			if (tokenType == T4TokenNodeTypes.Quote || tokenType == T4TokenNodeTypes.Value)
				return VsPredefinedHighlighterIds.HtmlAttributeValue;

			if (tokenType == T4TokenNodeTypes.Name) {
				return element.Parent is IT4Directive
					? VsPredefinedHighlighterIds.HtmlElementName
					: VsPredefinedHighlighterIds.HtmlAttributeName;
			}

			return null;
		}
		
		internal T4HighlightingProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess)
			: base(file, daemonProcess) {
		}

	}

}