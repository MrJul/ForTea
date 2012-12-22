using System.Collections.Generic;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Represents the difference between two <see cref="T4FileData" />.
	/// </summary>
	public sealed class T4FileDataDiff {

		private readonly IEnumerable<string> _addedAssemblies;
		private readonly IEnumerable<string> _removedAssemblies;
		private readonly IEnumerable<string> _addedMacros;

			/// <summary>
		/// Gets an enumeration of all added assemblies.
		/// </summary>
		[NotNull]
		public IEnumerable<string> AddedAssemblies {
			get { return _addedAssemblies; }
		}

		/// <summary>
		/// Gets an enumeration of all removed assemblies.
		/// </summary>
		[NotNull]
		public IEnumerable<string> RemovedAssemblies {
			get { return _removedAssemblies; }
		}

		/// <summary>
		/// Gets en enumeration of all added macros.
		/// </summary>
		[NotNull]
		public IEnumerable<string> AddedMacros {
			get { return _addedMacros; }
		}

		public T4FileDataDiff([NotNull] IEnumerable<string> addedAssemblies, [NotNull] IEnumerable<string> removedAssemblies,
			[NotNull] IEnumerable<string> addedMacros) {
			_addedAssemblies = addedAssemblies;
			_removedAssemblies = removedAssemblies;
			_addedMacros = addedMacros;
		}

	}

}