using System;
using GammaJul.ForTea.Core.Common;
using GammaJul.ForTea.Core.Psi.Modules;
using JetBrains.Annotations;
using JetBrains.ProjectModel.Properties;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ForTea.Core.Psi
{
	[SolutionFeaturePart]
	public class T4CSharpLanguageLevelProvider : CSharpLanguageLevelProvider
	{
		[NotNull] private readonly IT4Environment _t4Environment;

		public override bool IsApplicable(IPsiModule psiModule)
			=> psiModule is IT4FilePsiModule;

		public override CSharpLanguageLevel GetLanguageLevel(IPsiModule psiModule)
			=> _t4Environment.CSharpLanguageLevel;

		public override CSharpLanguageVersion? TryGetLanguageVersion(IPsiModule psiModule)
			=> _t4Environment.CSharpLanguageLevel.ToLanguageVersion();

		public T4CSharpLanguageLevelProvider(
			[NotNull] IT4Environment t4Environment,
			[NotNull] ILanguageLevelProjectProperty<CSharpLanguageLevel, CSharpLanguageVersion> projectProperty,
			[CanBeNull] ILanguageLevelOverrider<CSharpLanguageLevel> languageLevelOverrider = null,
			[CanBeNull] Lazy<ILanguageVersionModifier<CSharpLanguageVersion>> languageVersionModifier = null
		) : base(projectProperty, languageLevelOverrider, languageVersionModifier) => _t4Environment = t4Environment;
	}
}