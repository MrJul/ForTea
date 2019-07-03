using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Psi.Web.References;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	public sealed class T4FileReference : PathReferenceBase<IT4DirectiveAttribute, IT4Token>
	{
		[NotNull]
		private FileSystemPath Path { get; }

		public T4FileReference(
			[NotNull] IT4DirectiveAttribute owner,
			[NotNull] IT4Token token,
			[NotNull] FileSystemPath path
		) : base(
			owner,
			null,
			token,
			SelectRange(token)
		) => Path = path;

		public override FileSystemPath GetBasePath() => Path.Parent;

		protected override IReference BindToInternal(IDeclaredElement declaredElement, ISubstitution substitution) =>
			this;

		private static TreeTextRange SelectRange(IT4Token token) => token.GetUnquotedRangeWithin();
	}
}
