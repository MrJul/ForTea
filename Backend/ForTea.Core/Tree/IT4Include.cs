using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree
{
	/// <summary>Represents a T4 include. This is not a directive, it contains the included file tree.</summary>
	public interface IT4Include : IT4DirectiveOwner, IT4IncludeOwner
	{
		[NotNull]
		IT4PathWithMacros Path { get; set; }
	}
}
