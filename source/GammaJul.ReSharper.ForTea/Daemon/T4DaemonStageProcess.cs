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
using System.Collections.Generic;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	/// <summary>
	/// Base daemon stage process class for T4 stages.
	/// </summary>
	/// <remarks></remarks>
	internal abstract class T4DaemonStageProcess : IDaemonStageProcess, IRecursiveElementProcessor {

		private readonly IDaemonProcess _daemonProcess;
		private readonly IT4File _file;
		private readonly List<HighlightingInfo> _highlightings = new List<HighlightingInfo>();

		/// <summary>
		/// Gets the associated daemon process.
		/// </summary>
		public IDaemonProcess DaemonProcess {
			get { return _daemonProcess; }
		}

		/// <summary>
		/// Gets the associated T4 file.
		/// </summary>
		internal IT4File File {
			get { return _file; }
		}

		/// <summary>
		/// Returns whether the interior of a node should be processed.
		/// </summary>
		/// <param name="element">The node to check.</param>
		/// <returns><c>true</c> if the descendants of <paramref name="element"/> will be processed, <c>false</c> otherwise.</returns>
		public virtual bool InteriorShouldBeProcessed(ITreeNode element) {
			return !(element is IT4Include);
		}

		/// <summary>
		/// Processes a node, before its descendants are processed.
		/// </summary>
		/// <param name="element">The node to process.</param>
		public virtual void ProcessBeforeInterior(ITreeNode element) {
		}

		/// <summary>
		/// Processes a node, after its descendants have been processed.
		/// </summary>
		/// <param name="element">The node that was processed.</param>
		public virtual void ProcessAfterInterior(ITreeNode element) {
		}

		/// <summary>
		/// Gets whetheer the processing is finished.
		/// </summary>
		/// <remarks>This property also checks for interruption.</remarks>
		public virtual bool ProcessingIsFinished {
			get {
				if (_daemonProcess.InterruptFlag)
					throw new ProcessCancelledException();
				return false;
			}
		}

		/// <summary>
		/// Executes the process.
		/// </summary>
		public virtual void Execute(Action<DaemonStageResult> commiter) {
			File.ProcessDescendants(this);
			commiter(new DaemonStageResult(_highlightings.ToArray()));
		}

		protected void AddHighlighting([NotNull] HighlightingInfo highlighting) {
			_highlightings.Add(highlighting);
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T4DaemonStageProcess"/> class.
		/// </summary>
		/// <param name="file">The associated T4 file.</param>
		/// <param name="daemonProcess">The associated daemon process.</param>
		protected T4DaemonStageProcess([NotNull] IT4File file, [NotNull] IDaemonProcess daemonProcess) {
			_file = file;
			_daemonProcess = daemonProcess;
		}

	}

}