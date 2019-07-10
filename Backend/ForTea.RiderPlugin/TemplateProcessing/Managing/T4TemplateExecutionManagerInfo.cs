using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.Tree;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public readonly struct T4TemplateExecutionManagerInfo
	{
		public DateTime TimeStamp { get; }

		[NotNull]
		public string Code { get; }

		[NotNull, ItemNotNull]
		public IEnumerable<MetadataReference> References { get; }

		[NotNull]
		public IT4File File { get; }

		[CanBeNull]
		public IProgressIndicator ProgressIndicator { get; }

		public T4TemplateExecutionManagerInfo(
			DateTime timeStamp,
			[NotNull] string code,
			[NotNull, ItemNotNull] IEnumerable<MetadataReference> references,
			[NotNull] IT4File file,
			[CanBeNull] IProgressIndicator progressIndicator
		)
		{
			TimeStamp = timeStamp;
			Code = code;
			References = references;
			File = file;
			ProgressIndicator = progressIndicator;
		}

		public void AssertFileHasNotChanged()
		{
			File.GetSolution().Locks.AssertReadAccessAllowed();
			if (File.GetSourceFile().NotNull().LastWriteTimeUtc != TimeStamp)
				throw new OperationCanceledException();
		}
	}
}
