using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal sealed class T4OutsideSolutionSourceFileProperties : IPsiSourceFileProperties {

		public IEnumerable<string> GetPreImportedNamespaces() {
			return EmptyList<string>.InstanceList;
		}

		public string GetDefaultNamespace() {
			return String.Empty;
		}

		public IList<PreProcessingDirective> GetDefines() {
			return EmptyList<PreProcessingDirective>.InstanceList;
		}

		public T GetCustomProperties<T>()
		where T : class, ICustomPsiSourceFileProperties {
			return default(T);
		}

		public bool ShouldBuildPsi {
			get { return true; }
		}

		public bool IsGeneratedFile {
			get { return false; }
		}

		public bool ProvidesCodeModel {
			get { return true; }
		}

		public bool IsNonUserFile {
			get { return false; }
		}

	}

}