using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace GammaJul.ForTea.Core.Daemon.Highlightings {

	[StaticSeverityHighlighting(
		Severity.ERROR,
		T4Language.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class InvalidAttributeValueHighlighting : T4Highlighting<IT4Token> {

		[CanBeNull]
		public DirectiveAttributeInfo DirectiveAttributeInfo { get; }

		[NotNull]
		public override string ToolTip { get; }

		/// <summary>Initializes a new instance of the <see cref="InvalidAttributeValueHighlighting"/> class.</summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		/// <param name="directiveAttributeInfo">The <see cref="DirectiveAttributeInfo"/> representing the attribute in error.</param>
		/// <param name="errorMessage">The error message.</param>
		public InvalidAttributeValueHighlighting(
			[NotNull] IT4Token associatedNode,
			[CanBeNull] DirectiveAttributeInfo directiveAttributeInfo,
			[NotNull] string errorMessage
		)
			: base(associatedNode) {
			DirectiveAttributeInfo = directiveAttributeInfo;
			ToolTip = errorMessage;
		}

	}

}