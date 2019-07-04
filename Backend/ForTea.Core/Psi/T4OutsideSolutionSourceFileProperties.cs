using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {

	internal sealed class T4OutsideSolutionSourceFileProperties : IPsiSourceFileProperties {

		public IEnumerable<string> GetPreImportedNamespaces()
			=> EmptyList<string>.InstanceList;

		public string GetDefaultNamespace()
			=> String.Empty;

		public bool ShouldBuildPsi
			=> true;

		public bool IsGeneratedFile
			=> false;

		public bool ProvidesCodeModel
			=> true;

		public bool IsNonUserFile
			=> false;

		public ICollection<PreProcessingDirective> GetDefines()
			=> EmptyList<PreProcessingDirective>.InstanceList;

		public bool IsICacheParticipant
			=> false;

	}

}