using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4ModuleReferencer {
		
		private static bool ExecuteTransaction([NotNull] IPsiModule module, [NotNull] Action action) {
			PsiManager psiManager = PsiManager.GetInstance(module.GetSolution());
			if (psiManager.HasActiveTransaction) {
				action();
				return true;
			}
			return psiManager.DoTransaction(action, "T4 Assembly Reference").Succeded;
		}

	}

}