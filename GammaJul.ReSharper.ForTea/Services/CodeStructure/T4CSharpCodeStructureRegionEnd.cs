using JetBrains.ReSharper.Feature.Services.CSharp.CodeStructure;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	internal sealed class T4CSharpCodeStructureRegionEnd : CodeStructurePreprocessorElement, ICodeStructureBlockEnd {

		private readonly CSharpCodeStructureRegion _parentRegion;
		
		public override ICodeStructureBlockStart ParentBlock {
			get { return _parentRegion; }
		}

		public T4CSharpCodeStructureRegionEnd(CodeStructureElement parentElement, ITreeNode preprocessorDirective, CSharpCodeStructureProcessingState state)
			: base(parentElement, preprocessorDirective) {
			_parentRegion = state.Regions.TryPeek();
		}

	}

}