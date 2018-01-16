#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion


using System;
using System.Linq;
using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Module referencer that adds an assembly directive to a T4 file.
	/// </summary>
	[ModuleReferencer(Priority = -10)]
	public class T4ModuleReferencer : IModuleReferencer {

		private readonly T4Environment _environment;
		private readonly DirectiveInfoManager _directiveInfoManager;

		public bool CanReferenceModule(IPsiModule module, IPsiModule moduleToReference) {
			var t4PsiModule = module as T4PsiModule;
			if (t4PsiModule == null || !t4PsiModule.IsValid() || moduleToReference == null)
				return false;
			
			var assembly = moduleToReference.ContainingProjectModule as IAssembly;
			return assembly != null && assembly.TargetFrameworkId.Equals(_environment.TargetFrameworkId);
		}
		
		/// <summary>
		/// Returns true if module is referenced
		/// </summary>
		public bool ReferenceModule(IPsiModule module, IPsiModule moduleToReference) {
			return ReferenceModuleImpl(module, moduleToReference, null);
		}

		public bool ReferenceModuleWithType(IPsiModule module, ITypeElement typeToReference) {
			return ReferenceModuleImpl(module, typeToReference.Module, typeToReference.GetContainingNamespace().QualifiedName);
		}

		private bool ReferenceModuleImpl([NotNull] IPsiModule module, [NotNull] IPsiModule moduleToReference, [CanBeNull] string ns) {
			if (!CanReferenceModule(module, moduleToReference))
				return false;

			var t4PsiModule = (T4PsiModule) module;
			var assembly = (IAssembly) moduleToReference.ContainingProjectModule;
			Assertion.AssertNotNull(assembly, "assembly != null");

			var t4File = t4PsiModule.SourceFile.GetTheOnlyPsiFile(T4Language.Instance) as IT4File;
			if (t4File == null)
				return false;

			Action action = () => {

				// add assembly directive
				t4File.AddDirective(_directiveInfoManager.Assembly.CreateDirective(assembly.FullAssemblyName), _directiveInfoManager);

				// add import directive if necessary
				if (!String.IsNullOrEmpty(ns)
				&& !t4File.GetDirectives(_directiveInfoManager.Import).Any(d => String.Equals(ns, d.GetAttributeValue(_directiveInfoManager.Import.NamespaceAttribute.Name), StringComparison.Ordinal)))
					t4File.AddDirective(_directiveInfoManager.Import.CreateDirective(ns), _directiveInfoManager);

			};

			return ExecuteTransaction(module, action);
		}

		private static bool ExecuteTransaction([NotNull] IPsiModule module, [NotNull] Action action) {
			IPsiTransactions transactions = module.GetPsiServices().Transactions;
			if (transactions.Current != null) {
				action();
				return true;
			}
			return transactions.Execute("T4 Assembly Reference", action).Succeded;
		}

		public T4ModuleReferencer([NotNull] T4Environment environment, [NotNull] DirectiveInfoManager directiveInfoManager) {
			_environment = environment;
			_directiveInfoManager = directiveInfoManager;
		}

	}

}