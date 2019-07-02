using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Daemon.Processes {

	/// <summary>Base daemon stage process class for T4 stages.</summary>
	public abstract class T4DaemonStageProcess : IDaemonStageProcess, IRecursiveElementProcessor {

		[NotNull] [ItemNotNull] private readonly List<HighlightingInfo> _highlightings = new List<HighlightingInfo>();

		public IDaemonProcess DaemonProcess { get; }

		/// <summary>Gets the associated T4 file.</summary>
		internal IT4File File { get; }

		public virtual bool InteriorShouldBeProcessed(ITreeNode element)
			=> !(element is IT4Include);

		public virtual void ProcessBeforeInterior(ITreeNode element) {
		}

		public virtual void ProcessAfterInterior(ITreeNode element) {
		}

		public bool ProcessingIsFinished
			=> false;

		public virtual void Execute(Action<DaemonStageResult> commiter) {
			File.ProcessDescendants(this);
			commiter(new DaemonStageResult(_highlightings.ToArray()));
		}

		protected void AddHighlighting(DocumentRange range, [NotNull] IHighlighting highlighting)
			=> _highlightings.Add(new HighlightingInfo(range, highlighting));

		/// <summary>Initializes a new instance of the <see cref="T4DaemonStageProcess"/> class.</summary>
		/// <param name="file">The associated T4 file.</param>
		/// <param name="daemonProcess">The associated daemon process.</param>
		protected T4DaemonStageProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess) {
			File = file;
			DaemonProcess = daemonProcess;
		}

	}

}