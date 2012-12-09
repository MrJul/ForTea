using GammaJul.ReSharper.ForTea.Psi.Directives;
using GammaJul.ReSharper.ForTea.Tree;
using JetBrains.Annotations;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Psi {

	/// <summary>
	/// Cache holding <see cref="T4FileData"/> for each T4 file.
	/// </summary>
	[PsiComponent]
	public sealed class T4FileDataCache {

		private readonly DirectiveInfoManager _directiveInfoManager;
		private readonly WeakToStrongDictionary<IPsiSourceFile, T4FileData> _fileDataBySourceFile = new WeakToStrongDictionary<IPsiSourceFile, T4FileData>();
		private readonly Signal<Pair<IPsiSourceFile, T4FileDataDiff>> _fileDataChanged;

		[NotNull]
		public Signal<Pair<IPsiSourceFile, T4FileDataDiff>> FileDataChanged {
			get { return _fileDataChanged; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T4FileDataCache"/> class.
		/// </summary>
		/// <param name="lifetime">The lifetime of this class.</param>
		/// <param name="psiManager">The PSI manager.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="DirectiveInfoManager"/>.</param>
		public T4FileDataCache([NotNull] Lifetime lifetime, [NotNull] PsiManager psiManager, [NotNull] DirectiveInfoManager directiveInfoManager) {
			_directiveInfoManager = directiveInfoManager;
			_fileDataChanged = new Signal<Pair<IPsiSourceFile, T4FileDataDiff>>(lifetime, "T4FileDataCache.FileDataChanged");
			lifetime.AddBracket(
				() => psiManager.PsiFileCreated += OnPsiFileChanged,
				() => psiManager.PsiFileCreated -= OnPsiFileChanged);
			lifetime.AddBracket(
				() => psiManager.PhysicalPsiChanged += OnPhysicalPsiChanged,
				() => psiManager.PhysicalPsiChanged -= OnPhysicalPsiChanged);
			lifetime.AddDispose(_fileDataBySourceFile);
		}

		/// <summary>
		/// Called when a PSI file changes.
		/// </summary>
		/// <param name="treeNode">The tree node that changed.</param>
		/// <param name="psiChangedElementType">The type of the PSI change.</param>
		private void OnPhysicalPsiChanged(ITreeNode treeNode, PsiChangedElementType psiChangedElementType) {
			if (treeNode != null && psiChangedElementType == PsiChangedElementType.CONTENTS_CHANGED)
				OnPsiFileChanged(treeNode.GetContainingFile());
		}

		/// <summary>
		/// Called when a PSI file is created.
		/// </summary>
		/// <param name="file">The file that was created.</param>
		private void OnPsiFileChanged([CanBeNull] IFile file) {
			var t4File = file as IT4File;
			if (t4File != null)
				CreateOrUpdateData(t4File);
		}

		private void CreateOrUpdateData([NotNull] IT4File t4File) {
			IPsiSourceFile sourceFile = t4File.GetSourceFile();
			if (sourceFile == null || !sourceFile.LanguageType.Is<T4ProjectFileType>())
				return;

			var newData = new T4FileData(t4File, _directiveInfoManager);
			T4FileData existingData;
			lock (_fileDataBySourceFile) {
				_fileDataBySourceFile.TryGetValue(sourceFile, out existingData);
				_fileDataBySourceFile[sourceFile] = newData;
			}

			T4FileDataDiff diff = newData.DiffWith(existingData);
			if (diff != null)
				_fileDataChanged.Fire(Pair.Of(sourceFile, diff));
		}

	}

}