using System;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Transactions;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4ModuleReferencer {

		public bool CanReferenceModule(IPsiModule module, IPsiModule moduleToReference, IModuleReferenceResolveContext context) {
			return CanReferenceModule(module, moduleToReference);
		}

		public bool ReferenceModuleWithType(IPsiModule module, ITypeElement typeToReference, IModuleReferenceResolveContext resolveContext) {
			return ReferenceModuleWithType(module, typeToReference);
		}

		private static bool ExecuteTransaction([NotNull] IPsiModule module, [NotNull] Action action) {
			IPsiTransactions transactions = module.GetPsiServices().Transactions;
			if (transactions.Current != null) {
				action();
				return true;
			}
			return transactions.Execute("T4 Assembly Reference", action).Succeded;
		}

	}

}