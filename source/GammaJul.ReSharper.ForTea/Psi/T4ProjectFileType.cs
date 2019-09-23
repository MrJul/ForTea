using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>Represents a T4 project file type.</summary>
	[ProjectFileTypeDefinition(Name)]
	public class T4ProjectFileType : KnownProjectFileType {

		/// <summary>Gets an unique instance of <see cref="T4ProjectFileType"/>.</summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public new static T4ProjectFileType Instance { get; private set; }

		/// <summary>Gets the name of the file type.</summary>
		public new const string Name = T4Language.Name;

		private T4ProjectFileType()
			: base(Name, Name, new[] { ".tt", ".ttinclude", ".t4" }) {
		}

		protected T4ProjectFileType([NotNull] string name, [NotNull] string presentableName, [NotNull] IEnumerable<string> extensions)
			: base(name, presentableName, extensions) {
		}

		protected T4ProjectFileType([NotNull] string name, [NotNull] string presentableName)
			: base(name, presentableName) {
		}

		protected T4ProjectFileType([NotNull] string name)
			: base(name) {
		}

	}

}