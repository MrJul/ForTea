using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.UI.Controls.TreeView;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Services.CodeStructure {

	internal sealed class T4CSharpCodeStructureRegion : CSharpCodeStructureRegion {

		[CanBeNull] private readonly CSharpCodeStructureRegion _parentRegion;
		
		public override ICodeStructureBlockStart ParentBlock
			=> _parentRegion;

		public override bool CanRemove()
			=> false;

		public override void Remove()
			=> throw new NotSupportedException();

		public override bool CanMoveElements(RelativeLocation location, IList<CodeStructureElement> dropElements)
			=> false;

		public override void MoveElements(RelativeLocation location, IList<CodeStructureElement> dropElements)
			=> throw new NotSupportedException();

		public T4CSharpCodeStructureRegion(
			[NotNull] CodeStructureElement parentElement,
			[NotNull] ITreeNode preprocessorDirective,
			[NotNull] CSharpCodeStructureProcessingState state
		)
			: base(parentElement, preprocessorDirective, state) {
			_parentRegion = state.Regions.TryPeek();
		}

	}

}