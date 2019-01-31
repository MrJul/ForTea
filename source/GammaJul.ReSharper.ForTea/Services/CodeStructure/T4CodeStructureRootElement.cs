using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CodeStructure;

namespace GammaJul.ReSharper.ForTea.Services.CodeStructure {

	public class T4CodeStructureRootElement : CodeStructureRootElement {

		public T4CodeStructureRootElement([NotNull] IT4File file)
			: base(file) {
		}

	}

}