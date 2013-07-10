using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal partial class T4OutsideSolutionSourceFileProperties {

		public ICollection<PreProcessingDirective> GetDefines() {
			return EmptyList<PreProcessingDirective>.InstanceList;
		}

		public bool IsICacheParticipant {
			get { return true; }
		}

	}

}