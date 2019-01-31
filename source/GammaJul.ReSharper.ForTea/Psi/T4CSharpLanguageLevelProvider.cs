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
using JetBrains.Annotations;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ReSharper.ForTea.Psi {

	[SolutionFeaturePart]
	public class T4CSharpLanguageLevelProvider : CSharpLanguageLevelProvider {

		[NotNull] private readonly T4Environment _t4Environment;

		public override bool IsApplicable(IPsiModule psiModule)
			=> psiModule is T4PsiModule;

		public override CSharpLanguageLevel GetLanguageLevel(IPsiModule psiModule)
			=> _t4Environment.CSharpLanguageLevel;

		public override CSharpLanguageVersion? TryGetLanguageVersion(IPsiModule psiModule)
			=> _t4Environment.CSharpLanguageLevel.ToLanguageVersion();

		public T4CSharpLanguageLevelProvider(
			[NotNull] T4Environment t4Environment,
			[NotNull] ILanguageLevelProjectProperty<CSharpLanguageLevel, CSharpLanguageVersion> projectProperty,
			[CanBeNull] ILanguageLevelOverrider<CSharpLanguageLevel> languageLevelOverrider = null,
			[CanBeNull] Lazy<ILanguageVersionModifier<CSharpLanguageVersion>> languageVersionModifier = null)
			: base(projectProperty, languageLevelOverrider, languageVersionModifier) {
			_t4Environment = t4Environment;
		}

	}

}