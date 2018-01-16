#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion


using JetBrains.DocumentModel;
using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon.Highlightings {

	/// <summary>
	/// Base class for all T4 related highlightings.
	/// </summary>
	/// <typeparam name="TNode">The type of the node.</typeparam>
	public abstract class T4Highlighting<TNode> : IHighlighting
	where TNode : ITreeNode {

		[NotNull] private readonly TNode _associatedNode;

		/// <summary>
		/// Gets the tree node associated with this highlighting.
		/// </summary>
		/// <remarks></remarks>
		[NotNull]
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

		public DocumentRange CalculateRange() {
			return _associatedNode.GetNavigationRange();
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