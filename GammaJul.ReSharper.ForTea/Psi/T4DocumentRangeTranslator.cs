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
using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
#if SDK80
using JetBrains.ReSharper.Psi.Files;
#else
using JetBrains.ReSharper.Psi.Impl.PsiManagerImpl;
#endif
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {
	
	/// <summary>
	/// Translate T4 tree ranges from a file with includes to document ranges and vice-versa.
	/// </summary>
	internal sealed class T4DocumentRangeTranslator : IDocumentRangeTranslator {

		private readonly IT4IncludeOwner _root;
		private readonly IList<IT4Include> _includes;
		private readonly IPsiSourceFile _sourceFile;

		[NotNull]
		internal IPsiSourceFile SourceFile {
			get { return _sourceFile; }
		}

		private struct IncludeWithOffset {

			[CanBeNull]
			internal readonly IT4Include Include;

			internal readonly int Offset;
			
			internal IncludeWithOffset(int offset, [CanBeNull] IT4Include include = null) {
				Include = include;
				Offset = offset;
			}

		}

		private IncludeWithOffset FindIncludeAtOffset(TreeOffset offset, bool preferRoot) {
			// no includes, tree and document are matching
			if (_includes.Count == 0)
				return new IncludeWithOffset(offset.Offset);

			int includesLength = 0;
			int count = _includes.Count;
			for (int i = 0; i < count; i++) {
				IT4Include include = _includes[i];
				TreeTextRange includeRange = include.GetTreeTextRange();

				// the offset is before the include, in the root file
				if (offset < includeRange.StartOffset)
					return new IncludeWithOffset((offset - includesLength).Offset);
				
				// the offset is inside the include
				if (offset < includeRange.EndOffset) {
					// we're on an edge position: we can be just after the end of the root file,
					// or just at the beginning of an include; we make the choice using the preferRoot parameter
					if (offset == includeRange.StartOffset && preferRoot)
						return new IncludeWithOffset((offset - includesLength).Offset);
					return new IncludeWithOffset(offset - includeRange.StartOffset, include);
				}

				includesLength += includeRange.Length;
			}

			// the offset is after the include, in the root file
			return new IncludeWithOffset((offset - includesLength).Offset);
		}

		public DocumentRange Translate(TreeTextRange range) {
			if (!range.IsValid())
				return DocumentRange.InvalidRange;
			if (_sourceFile == null || !_sourceFile.IsValid())
				return DocumentRange.InvalidRange;

			IncludeWithOffset atStart = FindIncludeAtOffset(range.StartOffset, true);
			IncludeWithOffset atEnd = FindIncludeAtOffset(range.EndOffset, atStart.Include == null);

			// two different parts are overlapping
			IT4Include include = atStart.Include;
			if (include != atEnd.Include)
				return DocumentRange.InvalidRange;

			// recursive includes
			if (include != null) {
				if (include.DocumentRangeTranslator != null)
					return include.DocumentRangeTranslator.Translate(range);
				return DocumentRange.InvalidRange;
			}
			
			int rootStartOffset = _root.GetTreeStartOffset().Offset;
			return new DocumentRange(_sourceFile.Document, new TextRange(atStart.Offset - rootStartOffset, atEnd.Offset - rootStartOffset));
		}
		
		public DocumentRange[] GetIntersectedOriginalRanges(TreeTextRange range) {
			return new[] { Translate(range) };
		}

		public TreeTextRange Translate(DocumentRange documentRange) {
			if (!documentRange.IsValid()
			|| _sourceFile == null
			|| !_sourceFile.IsValid())
				return TreeTextRange.InvalidRange;

			if (documentRange.Document != _sourceFile.Document) {
				foreach (IT4Include include in _includes) {
					if (include.DocumentRangeTranslator != null) {
						TreeTextRange textRange = include.DocumentRangeTranslator.Translate(documentRange);
						if (textRange.IsValid())
							return textRange;
					}
				}
				return TreeTextRange.InvalidRange;
			}

			TextRange range = documentRange.TextRange;
			TreeOffset rootStartOffset = _root.GetTreeStartOffset();

			// no includes, tree and document are matching
			if (_includes.Count == 0)
				return new TreeTextRange(rootStartOffset + range.StartOffset, rootStartOffset + range.EndOffset);

			TreeOffset startOffset = Translate(range.StartOffset);
			if (!startOffset.IsValid())
				return TreeTextRange.InvalidRange;

			TreeOffset endOffset = Translate(range.EndOffset);
			if (!endOffset.IsValid())
				return TreeTextRange.InvalidRange;
			
			return new TreeTextRange(startOffset, endOffset);
		}

		private TreeOffset Translate(int documentOffset) {
			int offset = 0;

			foreach (IT4Include include in _includes) {
				TreeTextRange includeRange = include.GetTreeTextRange();

				var finalOffset = new TreeOffset(documentOffset + offset);

				// the matching file offset starts before the include, we got it
				if (finalOffset < includeRange.StartOffset)
					return finalOffset;

				offset += includeRange.Length;
			}

			// the offset is in the file, after the last include
			return new TreeOffset(documentOffset + offset);
		}

		public T4DocumentRangeTranslator([NotNull] IT4IncludeOwner root, [NotNull] IPsiSourceFile sourceFile, [NotNull] IList<IT4Include> includes) {
			_root = root;
			_sourceFile = sourceFile;
			_includes = includes;
		}

	}

}