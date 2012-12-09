using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Contains the different types of T4 composite elements.
	/// </summary>
	public static class T4ElementTypes {

		public static readonly CompositeNodeType T4File = new T4CompositeNodeType<T4File>();
		public static readonly CompositeNodeType T4StatementBlock = new T4CompositeNodeType<T4StatementBlock>();
		public static readonly CompositeNodeType T4ExpressionBlock = new T4CompositeNodeType<T4ExpressionBlock>();
		public static readonly CompositeNodeType T4FeatureBlock = new T4CompositeNodeType<T4FeatureBlock>();
		public static readonly CompositeNodeType T4Directive = new T4CompositeNodeType<T4Directive>();
		public static readonly CompositeNodeType T4DirectiveAttribute = new T4CompositeNodeType<T4DirectiveAttribute>();
		public static readonly CompositeNodeType T4Include = new T4CompositeNodeType<T4Include>();

		private class T4CompositeNodeType<T> : CompositeNodeType
		where T : CompositeElement, new() {
			
			public override CompositeElement Create() {
				return new T();
			}

			internal T4CompositeNodeType()
				: base(typeof(T).Name) {
			}

		}

	}

}