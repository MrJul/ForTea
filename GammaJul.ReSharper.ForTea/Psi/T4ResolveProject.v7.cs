using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.Common;
using JetBrains.Util;
using PlatformID = JetBrains.ProjectModel.PlatformID;

namespace GammaJul.ReSharper.ForTea.Psi {

	internal partial class T4ResolveProject {

		ICollection<IModuleToModuleReference> IModule.GetReferences() {
			return EmptyList<IModuleToModuleReference>.InstanceList;
		}

		bool IProject.IsWebProject {
			get { return false; }
		}

		bool IProject.IsWebApplication {
			get { return false; }
		}

		ProjectKind IProject.ProjectKind {
			get { return _projectProperties.ProjectKind; }
		}

		ProjectLanguage IProject.DefaultLanguage {
			get { return _projectProperties.DefaultLanguage; }
		}

		IBuildSettings IProject.BuildSettings {
			get { return _projectProperties.BuildSettings; }
		}

		IProjectConfiguration IProject.ActiveConfiguration {
			get { return null; }
		}


		private sealed class T4ResolveProjectProperties : ProjectPropertiesBase {

			public override IBuildSettings BuildSettings {
				get { return null; }
			}

			public override ProjectLanguage DefaultLanguage {
				get { return ProjectLanguage.UNKNOWN; }
			}

			public override ProjectKind ProjectKind {
				get { return ProjectKind.MISC_FILES_PROJECT; }
			}

			internal T4ResolveProjectProperties([NotNull] PlatformID platformID)
				: base(EmptyList<Guid>.InstanceList, platformID, Guid.Empty) {
			}

		}

	}

}