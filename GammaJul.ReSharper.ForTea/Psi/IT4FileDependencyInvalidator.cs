using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	public interface IT4FileDependencyInvalidator {

		void AddCommittedFilePath([NotNull] FileSystemPath path);

	}

}