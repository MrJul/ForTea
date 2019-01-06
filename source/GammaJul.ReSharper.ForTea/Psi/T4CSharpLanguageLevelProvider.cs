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
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ReSharper.ForTea.Psi {

	[SolutionFeaturePart]
	public class T4CSharpLanguageLevelProvider : CSharpLanguageLevelProvider {

		private readonly ILanguageLevelProjectProperty<CSharpLanguageLevel, CSharpLanguageVersion> _projectProperty;

		[NotNull] private readonly T4Environment _t4Environment;

		public override bool IsApplicable(IPsiModule psiModule)
			=> psiModule is T4PsiModule;
		
		public override CSharpLanguageLevel GetLanguageLevel(IPsiModule psiModule)
			=> (CSharpLanguageLevel) _projectProperty.GetLanguageLevel(((IProjectPsiModule) psiModule).Project).ToLanguageVersion();
		
		public T4CSharpLanguageLevelProvider(ILanguageLevelProjectProperty<CSharpLanguageLevel, CSharpLanguageVersion> projectProperty, ILanguageLevelOverrider<CSharpLanguageLevel> languageLevelOverrider = null, Lazy<ILanguageVersionModifier<CSharpLanguageVersion>> languageVersionModifier = null)
			: base(projectProperty, languageLevelOverrider, languageVersionModifier) {
			_projectProperty = projectProperty;
		}

	}

}