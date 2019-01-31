using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	internal sealed class T4CSharpCodeStructureRegionEnd : CodeStructurePreprocessorElement, ICodeStructureBlockEnd {

		[CanBeNull] private readonly CSharpCodeStructureRegion _parentRegion;
		
		public override ICodeStructureBlockStart ParentBlock
			=> _parentRegion;

		public T4CSharpCodeStructureRegionEnd(
			[NotNull] CodeStructureElement parentElement,
			[NotNull] ITreeNode preprocessorDirective,
			[NotNull] CSharpCodeStructureProcessingState state
		)
			: base(parentElement, preprocessorDirective) {
			_parentRegion = state.Regions.TryPeek();
		}

	}

}