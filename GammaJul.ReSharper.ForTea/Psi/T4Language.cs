using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Represents the T4 language.
	/// </summary>
	[LanguageDefinition(Name)]
	public class T4Language : KnownLanguage {

		/// <summary>
		/// Gets the name of the T4 language.
		/// </summary>
		public new const string Name = "T4";

		/// <summary>
		/// Gets an unique instance of <see cref="T4Language"/>.
		/// </summary>
		[UsedImplicitly(ImplicitUseKindFlags.Assign)]
		public static T4Language Instance;

		private T4Language()
			: base(Name, Name) {
		}

		protected T4Language([NotNull] string name, [NotNull] string presentableName)
			: base(name, presentableName) {
		}

		protected T4Language([NotNull] string name)
			: base(name) {
		}

	}

}