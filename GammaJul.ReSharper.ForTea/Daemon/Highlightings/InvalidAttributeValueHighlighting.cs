using GammaJul.ReSharper.ForTea.Psi;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	[StaticSeverityHighlighting(Severity.ERROR, T4Language.Name, OverlapResolve = OverlapResolveKind.ERROR, ShowToolTipInStatusBar = true, AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE)]
	public class InvalidAttributeValueHighlighting : T4Highlighting<IT4Token> {
		private readonly DirectiveAttributeInfo _directiveAttributeInfo;
		private readonly string _errorMessage;

		[CanBeNull]
		public DirectiveAttributeInfo DirectiveAttributeInfo {
			get { return _directiveAttributeInfo; }
		}

		public override string ToolTip {
			get { return _errorMessage; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidAttributeValueHighlighting"/> class.
		/// </summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		/// <param name="directiveAttributeInfo">The <see cref="DirectiveAttributeInfo"/> representing the attribute in error.</param>
		/// <param name="errorMessage">The error message.</param>
		public InvalidAttributeValueHighlighting([NotNull] IT4Token associatedNode, [CanBeNull] DirectiveAttributeInfo directiveAttributeInfo,
			[NotNull] string errorMessage)
			: base(associatedNode) {
			_directiveAttributeInfo = directiveAttributeInfo;
			_errorMessage = errorMessage;
		}

	}

}