using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ForTea.Core.Daemon.Highlightings {

	[StaticSeverityHighlighting(
		Severity.ERROR,
		CSharpProjectFileType.Name,
		OverlapResolve = OverlapResolveKind.ERROR,
		ShowToolTipInStatusBar = true,
		AttributeId = HighlightingAttributeIds.ERROR_ATTRIBUTE
	)]
	public class MissingTransformTextMethodHighlighting : IHighlighting {

		[NotNull]
		public ITypeUsage BaseClassNode { get; }

		[NotNull]
		public ITypeElement BaseClass { get; }

		public bool IsValid()
			=> BaseClassNode.IsValid();

		public string ToolTip
			=> "Base class doesn't have a valid TransformText method.";

		public string ErrorStripeToolTip
			=> ToolTip;

		public DocumentRange CalculateRange()
			=> BaseClassNode.GetNavigationRange();

		public MissingTransformTextMethodHighlighting(
			[NotNull] ITypeUsage baseClassNode,
			[NotNull] ITypeElement baseClass
		)
		{
			BaseClassNode = baseClassNode;
			BaseClass = baseClass;
		}

	}

}
