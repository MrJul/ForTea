#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
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