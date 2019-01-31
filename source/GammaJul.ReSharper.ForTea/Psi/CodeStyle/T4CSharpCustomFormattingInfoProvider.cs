using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi.CodeStyle {

	[ProjectFileType(typeof(T4ProjectFileType))]
	public class T4CSharpCustomFormattingInfoProvider : DummyCSharpCustomFormattingInfoProvider {

		public override bool CanModifyInsideNodeRange(ITreeNode leftElement, ITreeNode rightElement) {
			var leftBlock = leftElement.GetT4ContainerFromCSharpNode<IT4CodeBlock>();
			if (leftBlock != null)
				return !(leftBlock.Parent is IT4Include);

			var rightBlock = rightElement.GetT4ContainerFromCSharpNode<IT4CodeBlock>();
			return rightBlock != null && !(rightBlock.Parent is IT4Include);
		}

		public override SpaceType GetBlockSpaceType(CSharpFmtStageContext ctx, CSharpCodeFormattingContext context) {
			ITreeNode leftChild = ctx.LeftChild;
			if (leftChild is ICommentNode
			&& leftChild.GetText() == T4CSharpCodeGenerator.CodeCommentStart
			&& !leftChild.HasLineFeedsTo(ctx.RightChild, context.CodeFormatter))
				return ctx.Parent is IClassBody ? SpaceType.Vertical : SpaceType.Horizontal;

			if (ctx.RightChild is ICommentNode
			&& ctx.RightChild.GetText() == T4CSharpCodeGenerator.CodeCommentEnd)
				return ctx.Parent is IClassBody || leftChild.HasLineFeedsTo(ctx.RightChild, context.CodeFormatter) ? SpaceType.Vertical : SpaceType.Horizontal;

			return SpaceType.Default;
		}

	}

}