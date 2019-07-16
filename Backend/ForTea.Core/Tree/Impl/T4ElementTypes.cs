using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl {

	/// <summary>Contains the different types of T4 composite elements.</summary>
	public static class T4ElementTypes {

		[NotNull] public static readonly CompositeNodeType T4File = new T4CompositeNodeType<T4File>(2001);
		[NotNull] public static readonly CompositeNodeType T4StatementBlock = new T4CompositeNodeType<T4StatementBlock>(2002);
		[NotNull] public static readonly CompositeNodeType T4ExpressionBlock = new T4CompositeNodeType<T4ExpressionBlock>(2003);
		[NotNull] public static readonly CompositeNodeType T4FeatureBlock = new T4CompositeNodeType<T4FeatureBlock>(2004);
		[NotNull] public static readonly CompositeNodeType T4Directive = new T4CompositeNodeType<T4Directive>(2005);
		[NotNull] public static readonly CompositeNodeType T4DirectiveAttribute = new T4CompositeNodeType<T4DirectiveAttribute>(2006);
		[NotNull] public static readonly CompositeNodeType T4Include = new T4CompositeNodeType<T4Include>(2007);
		[NotNull] public static readonly CompositeNodeType T4Code = new T4CompositeNodeType<T4Code>(2008);
		[NotNull] public static readonly CompositeNodeType T4Text = new T4CompositeNodeType<T4Code>(2009);
		[NotNull] public static readonly CompositeNodeType T4AttributeValue = new T4CompositeNodeType<T4Code>(2009);

		private sealed class T4CompositeNodeType<T> : CompositeNodeType
		where T : CompositeElement, new() {
			
			public override CompositeElement Create()
				=> new T();

			public T4CompositeNodeType(int index)
				: base(typeof(T).Name, index) {
			}

		}

	}

}
