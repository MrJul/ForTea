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
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Services.Selection {

	public class T4CodeBehindWrappedSelection : ISelectedRange {

		private readonly IT4File _file;
		private readonly ISelectedRange _codeBehindRange;

		public DocumentRange Range {
			get { return _codeBehindRange.Range; }
		}

		public ISelectedRange Parent {
			get {
				ISelectedRange parent = _codeBehindRange.Parent;
				if (parent != null && parent.Range.IsValid())
					return new T4CodeBehindWrappedSelection(_file, parent);
				ITreeNode node = _file.FindNodeAt(Range);
				return node == null ? null : new T4NodeSelection(_file, node);
			}
		}

		public ExtendToTheWholeLinePolicy ExtendToWholeLine {
			get { return _codeBehindRange.ExtendToWholeLine; }
		}

		public T4CodeBehindWrappedSelection([NotNull] IT4File file, [NotNull] ISelectedRange codeBehindRange) {
			_file = file;
			_codeBehindRange = codeBehindRange;
		}

	}

}