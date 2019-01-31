using System.Collections.Generic;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>Represents the difference between two <see cref="T4FileData" />.</summary>
	public sealed class T4FileDataDiff {

		/// <summary>Gets an enumeration of all added assemblies.</summary>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<string> AddedAssemblies { get; }

		/// <summary>Gets an enumeration of all removed assemblies.</summary>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<string> RemovedAssemblies { get; }

		/// <summary>Gets en enumeration of all added macros.</summary>
		[NotNull]
		[ItemNotNull]
		public IEnumerable<string> AddedMacros { get; }

		public T4FileDataDiff(
			[NotNull] [ItemNotNull] IEnumerable<string> addedAssemblies,
			[NotNull] [ItemNotNull] IEnumerable<string> removedAssemblies,
			[NotNull] [ItemNotNull] IEnumerable<string> addedMacros
		) {
			AddedAssemblies = addedAssemblies;
			RemovedAssemblies = removedAssemblies;
			AddedMacros = addedMacros;
		}

	}

}