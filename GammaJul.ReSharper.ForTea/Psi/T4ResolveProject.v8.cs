using System;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.Common;
using JetBrains.Util;
using PlatformID = JetBrains.ProjectModel.PlatformID;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal partial class T4ResolveProject {

		FileSystemPath IProject.GetOutputDirectory() {
			return FileSystemPath.Empty;
		}

		FileSystemPath IProject.GetOutputFilePath() {
			return FileSystemPath.Empty;
		}

		private sealed class T4ResolveProjectProperties : ProjectPropertiesBase, IProjectProperties {

			public override IBuildSettings BuildSettings {
				get { return null; }
			}

			public ProjectLanguage DefaultLanguage {
				get { return ProjectLanguage.UNKNOWN; }
			}

			public ProjectKind ProjectKind {
				get { return ProjectKind.MISC_FILES_PROJECT; }
			}

			public IProjectConfiguration ActiveConfiguration {
				get { return new UnsupportedProjectConfiguration(); }
			}

			internal T4ResolveProjectProperties([NotNull] PlatformID platformID)
				: base(EmptyList<Guid>.InstanceList, platformID, Guid.Empty) {
			}

		}

	}

}