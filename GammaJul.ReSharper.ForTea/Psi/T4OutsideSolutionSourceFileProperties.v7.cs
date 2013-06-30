using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal partial class T4OutsideSolutionSourceFileProperties {

		public IList<PreProcessingDirective> GetDefines() {
			return EmptyList<PreProcessingDirective>.InstanceList;
		}

		public T GetCustomProperties<T>()
		where T : class, ICustomPsiSourceFileProperties {
			return default(T);
		}

	}

}