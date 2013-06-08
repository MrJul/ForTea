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
using System;
using System.Text;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Holds a generated result for code-behind generation.
	/// </summary>
	internal sealed class GenerationResult {
		private readonly StringBuilder _builder = new StringBuilder();
		private readonly IGeneratedRangeMap _generatedRangeMap;

		/// <summary>
		/// Gets the string builder containing the generated text.
		/// </summary>
		internal StringBuilder Builder {
			get { return _builder; }
		}

		/// <summary>
		/// Gets the generated range map.
		/// </summary>
		internal IGeneratedRangeMap GeneratedRangeMap {
			get { return _generatedRangeMap; }
		}

		/// <summary>
		/// Appends a mapped text.
		/// </summary>
		/// <param name="text">The text to add.</param>
		/// <param name="textRange">The original text range.</param>
		internal void AppendMapped([CanBeNull] string text, TreeTextRange textRange) {
			if (String.IsNullOrEmpty(text))
				return;

			var startOffset = new TreeOffset(_builder.Length);
			_builder.Append(text);
			var endOffset = new TreeOffset(_builder.Length);
			_generatedRangeMap.Add(
				new TreeTextRange<Generated>(startOffset, endOffset),
				new TreeTextRange<Original>(textRange));
		}
		
		/// <summary>
		/// Appends a mapped node.
		/// </summary>
		/// <param name="treeNode">The tree node to add.</param>
		internal void AppendMapped([CanBeNull] ITreeNode treeNode) {
			if (treeNode == null)
				return;

			var startOffset = new TreeOffset(_builder.Length);
			treeNode.GetText(_builder);
			var endOffset = new TreeOffset(_builder.Length);
			_generatedRangeMap.Add(
				new TreeTextRange<Generated>(startOffset, endOffset),
				new TreeTextRange<Original>(treeNode.GetTreeTextRange()));
		}

		/// <summary>
		/// Appends another <see cref="GenerationResult"/> to this result.
		/// </summary>
		/// <param name="otherResult">The other result to append.</param>
		internal void Append([CanBeNull] GenerationResult otherResult) {
			if (otherResult == null || otherResult._builder.Length == 0)
				return;

			int offset = _builder.Length;
			_builder.Append(otherResult._builder);
			_generatedRangeMap.AppendWithShiftToGenerated(otherResult._generatedRangeMap, offset);
		}
		
		/// <summary>
		/// Creates a new instance of <see cref="GenerationResult"/> for a given file.
		/// </summary>
		/// <param name="file">The T4 file that will be used for code-behind generation.</param>
		internal GenerationResult([NotNull] IT4File file) {
			_builder = new StringBuilder(1024);
			_generatedRangeMap = new GeneratedRangeMapTree(file);
		}

	}

}