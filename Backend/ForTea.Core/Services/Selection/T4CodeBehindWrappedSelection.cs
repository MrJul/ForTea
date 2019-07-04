using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Services.Selection {

	public class T4CodeBehindWrappedSelection : ISelectedRange {

		[NotNull] private readonly IT4File _file;
		[NotNull] private readonly ISelectedRange _codeBehindRange;

		public DocumentRange Range
			=> _codeBehindRange.Range;

		public ISelectedRange Parent {
			get {
				ISelectedRange parent = _codeBehindRange.Parent;
				if (parent != null && parent.Range.IsValid())
					return new T4CodeBehindWrappedSelection(_file, parent);
				ITreeNode node = _file.FindNodeAt(Range);
				return node == null ? null : new T4NodeSelection(_file, node);
			}
		}

		public ExtendToTheWholeLinePolicy ExtendToWholeLine
			=> _codeBehindRange.ExtendToWholeLine;

		public T4CodeBehindWrappedSelection([NotNull] IT4File file, [NotNull] ISelectedRange codeBehindRange) {
			_file = file;
			_codeBehindRange = codeBehindRange;
		}

	}

}