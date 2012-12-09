using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Represents a T4 project file type.
	/// </summary>
	[ProjectFileTypeDefinition(Name)]
	public class T4ProjectFileType : KnownProjectFileType {

		/// <summary>
		/// Gets an unique instance of <see cref="T4ProjectFileType"/>.
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public new static T4ProjectFileType Instance;

		/// <summary>
		/// Gets the name of the file type.
		/// </summary>
		public new const string Name = T4Language.Name;

		/// <summary>
		/// Gets the default extension of T4 files.
		/// </summary>
		public const string TtExtension = ".tt";
		public const string TtIncludeExtension = ".ttinclude";

		private T4ProjectFileType()
			: base(Name, Name, new[] { TtExtension, TtIncludeExtension }) {
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