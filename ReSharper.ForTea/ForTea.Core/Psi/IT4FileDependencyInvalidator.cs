using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {

	public interface IT4FileDependencyInvalidator {

		void AddCommittedFilePath([NotNull] FileSystemPath path);

	}

}