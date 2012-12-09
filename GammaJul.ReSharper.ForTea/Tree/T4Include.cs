using System.Collections.Generic;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl.PsiManagerImpl;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// A T4 include. This is not a directive, it contains the included file tree.
	/// </summary>
	internal sealed class T4Include : T4CompositeElement, IT4Include {

		public override NodeType NodeType {
			get { return T4ElementTypes.T4Include; }
		}

		public FileSystemPath Path { get; set; }

		/// <summary>
		/// Gets a list of direct includes.
		/// </summary>
		/// <returns>A list of <see cref="IT4Include"/>.</returns>
		public IEnumerable<IT4Include> GetIncludes() {
			return this.Children<IT4Include>();
		}

		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			return T4TokenRole.Unknown;
		}

		public IEnumerable<IT4Directive> GetDirectives() {
			return this.Children<IT4Directive>();
		}

		public IDocumentRangeTranslator DocumentRangeTranslator { get; set; }

	}

}