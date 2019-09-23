using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	[StaticSeverityHighlighting(
		Severity.ERROR,
		CSharpProjectFileType.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class MissingTransformTextMethodHighlighting : IHighlighting {

		[NotNull]
		public IUserTypeUsage DeclaredTypeUsage { get; }

		public bool IsValid()
			=> DeclaredTypeUsage.IsValid();

		public string ToolTip
			=> "Base class doesn't have a valid TransformText method.";

		public string ErrorStripeToolTip
			=> ToolTip;

		public DocumentRange CalculateRange()
			=> DeclaredTypeUsage.GetNavigationRange();

		public MissingTransformTextMethodHighlighting([NotNull] IUserTypeUsage declaredTypeUsage) {
			DeclaredTypeUsage = declaredTypeUsage;
		}

	}

}