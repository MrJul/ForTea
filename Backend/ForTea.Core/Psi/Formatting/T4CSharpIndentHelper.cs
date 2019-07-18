using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Asp;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Format;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Formatting
{
	public class T4CSharpIndentHelper
	{
		private FormatSettingsKeyBase GlobalSettings { get; }
		public T4CSharpIndentHelper(FormatSettingsKeyBase globalSettings) => GlobalSettings = globalSettings;
		public string IndentStr => GlobalSettings.GetIndentStr();
		public int TabSize => GlobalSettings.INDENT_SIZE;

		public bool IsGeneratedMethodMember(ITreeNode node)
		{
			var block = node.GetT4ContainerFromCSharpNode<IT4CodeBlock>();
			return block != null && !(block.Parent is IT4Include);
		}

		public bool IsTypeMemberLikeNode(ITreeNode node, IPsiSourceFile sourceFile)
		{
			if ((node is ITypeMemberDeclaration || node is IMultipleFieldDeclaration ||
			     node is IMultipleEventDeclaration) && !(node is IEventDeclaration))
			{
				var parentDeclaration = node.GetContainingNode<IClassLikeDeclaration>();
				if (parentDeclaration != null)
					return Equals(parentDeclaration.DeclaredElement, sourceFile.GetAspTypeElement());
			}

			return false;
		}

		public bool IsStatement(ITreeNode node) => node is IStatement;
	}
}
