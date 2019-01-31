using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>Represents a T4 include. This is not a directive, it contains the included file tree.</summary>
	public interface IT4Include : IT4DirectiveOwner, IT4IncludeOwner {

		[CanBeNull]
		FileSystemPath Path { get; }

	}

}