using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Psi.Web.References;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	public sealed class T4FileReference : PathReferenceBase<IT4DirectiveAttribute, ITreeNode>
	{
		[NotNull]
		private FileSystemPath Path { get; }

		public T4FileReference(
			[NotNull] IT4DirectiveAttribute owner,
			[NotNull] ITreeNode node,
			[NotNull] FileSystemPath path
		) : base(
			owner,
			null,
			node,
			SelectRange(node)
		) => Path = path;

		public override FileSystemPath GetBasePath() => Path.Parent;

		protected override IReference BindToInternal(IDeclaredElement declaredElement, ISubstitution substitution) =>
			this;

		private static TreeTextRange SelectRange(ITreeNode node) => node.GetUnquotedRangeWithin();
	}
}
