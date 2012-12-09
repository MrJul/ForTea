using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>
	/// Base class for all T4 related highlightings.
	/// </summary>
	/// <typeparam name="TNode">The type of the node.</typeparam>
	public abstract class T4Highlighting<TNode> : IHighlighting
	where TNode : ITreeNode {
		private readonly TNode _associatedNode;

		/// <summary>
		/// Gets the tree node associated with this highlighting.
		/// </summary>
		/// <remarks></remarks>
		public TNode AssociatedNode {
			get { return _associatedNode; }
		}

		/// <summary>
		/// Returns true if data (PSI, text ranges) associated with highlighting is valid
		/// </summary>
		public bool IsValid() {
			return _associatedNode.IsValid();
		}

		/// <summary>
		/// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
		///             To override the default mechanism of tooltip, mark the implementation class with 
		///             <see cref="T:JetBrains.ReSharper.Daemon.DaemonTooltipProviderAttribute"/> attribute, and then this property will not be called
		/// </summary>
		public abstract string ToolTip { get; }

		/// <summary>
		/// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlightingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
		/// </summary>
		public string ErrorStripeToolTip {
			get { return ToolTip; }
		}

		/// <summary>
		/// Specifies the offset from the Range.StartOffset to set the cursor to when navigating 
		///             to this highlighting. Usually returns <c>0</c>
		/// </summary>
		public int NavigationOffsetPatch {
			get { return 0; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4Highlighting{TNode}"/> class.
		/// </summary>
		/// <param name="associatedNode">The tree node associated with this highlighting.</param>
		protected T4Highlighting([NotNull] TNode associatedNode) {
			if (ReferenceEquals(associatedNode, null))
				throw new ArgumentNullException("associatedNode");
			_associatedNode = associatedNode;
		}
	}

}