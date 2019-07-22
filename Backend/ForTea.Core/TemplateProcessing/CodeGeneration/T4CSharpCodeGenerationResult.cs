using System;
using System.Text;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Shared;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration {

	/// <summary>Holds a generated result for code-behind generation.</summary>
	public sealed class T4CSharpCodeGenerationResult {

		/// <summary>Gets the string builder containing the generated text.</summary>
		[NotNull]
		private StringBuilder Builder { get; }

		[NotNull]
		public string RawText => Builder.ToString();

		public bool IsEmpty => Builder.IsEmpty();

		/// <summary>Gets the generated range map.</summary>
		[NotNull]
		public IGeneratedRangeMap GeneratedRangeMap { get; }

		/// <summary>Appends a mapped text.</summary>
		/// <param name="text">The text to add.</param>
		/// <param name="textRange">The original text range.</param>
		public void AppendMapped([CanBeNull] string text, TreeTextRange textRange) {
			if (String.IsNullOrEmpty(text))
				return;

			var startOffset = new TreeOffset(Builder.Length);
			Builder.Append(text);
			var endOffset = new TreeOffset(Builder.Length);
			GeneratedRangeMap.Add(
				new TreeTextRange<Generated>(startOffset, endOffset),
				new TreeTextRange<Original>(textRange)
			);
		}
		
		/// <summary>Appends a mapped node.</summary>
		/// <param name="treeNode">The tree node to add.</param>
		public void AppendMapped([CanBeNull] ITreeNode treeNode) {
			if (treeNode == null)
				return;

			var startOffset = new TreeOffset(Builder.Length);
			treeNode.GetText(Builder);
			var endOffset = new TreeOffset(Builder.Length);
			GeneratedRangeMap.Add(
				new TreeTextRange<Generated>(startOffset, endOffset),
				new TreeTextRange<Original>(treeNode.GetTreeTextRange())
			);
		}

		/// <summary>Appends another <see cref="T4CSharpCodeGenerationResult"/> to this result.</summary>
		/// <param name="otherResult">The other result to append.</param>
		public void Append([NotNull] T4CSharpCodeGenerationResult otherResult) {
			if (otherResult.Builder.Length == 0) return;
			int offset = Builder.Length;
			Builder.Append(otherResult.Builder);
			GeneratedRangeMap.AppendWithShiftToGenerated(otherResult.GeneratedRangeMap, offset);
		}

		public void Append([NotNull] string text) => Builder.Append(text);

		public void AppendLine() => Builder.AppendLine();

		public void AppendLine([NotNull] string text) => Builder.AppendLine(text);

		/// <summary>Creates a new instance of <see cref="T4CSharpCodeGenerationResult"/> for a given file.</summary>
		/// <param name="file">The T4 file that will be used for code-behind generation.</param>
		public T4CSharpCodeGenerationResult([NotNull] IT4File file) {
			Builder = new StringBuilder(1024);
			GeneratedRangeMap = new GeneratedRangeMapTree(file);
		}

	}

}
