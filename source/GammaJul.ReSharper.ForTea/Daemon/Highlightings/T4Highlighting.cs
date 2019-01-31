using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>Base class for all T4 related highlightings.</summary>
	/// <typeparam name="TNode">The type of the node.</typeparam>
	public abstract class T4Highlighting<TNode> : IHighlighting
	where TNode : ITreeNode {

		/// <summary>Gets the tree node associated with this highlighting.</summary>
		[NotNull]
		public TNode AssociatedNode { get; }

		public bool IsValid()
			=> AssociatedNode.IsValid();

		public abstract string ToolTip { get; }

		public string ErrorStripeToolTip
			=> ToolTip;

		public DocumentRange CalculateRange()
			=> AssociatedNode.GetNavigationRange();

		/// <summary>Initializes a new instance of the <see cref="T4Highlighting{TNode}"/> class.</summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		protected T4Highlighting([NotNull] TNode associatedNode) {
			AssociatedNode = associatedNode;
		}

	}

}