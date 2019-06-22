using System.Collections.Generic;
using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration
{
	/// <summary>
	/// This class generates a code-behind file from C# embedded statements and directives in the T4 file.
	/// </summary>
	internal sealed class T4CSharpCodeBehindGenerator : T4CSharpCodeGeneratorBase
	{
		[NotNull] public const string GeneratedClassNameString = "Generated\x200CTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = "Generated\x200CTransformationBase";

		public T4CSharpCodeBehindGenerator(
			[NotNull] IT4File file,
			[NotNull] T4DirectiveInfoManager manager
		) : base(file) => Collector = new T4CSharpCodeBehindGenerationInfoCollector(file, manager);

		protected override string ResourceName =>
			"GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.TemplateBaseStub.cs";

		protected override void AppendSyntheticAttribute(StringBuilder builder) =>
			builder.AppendLine($"[{SyntheticAttribute.Name}]");

		protected override string GeneratedClassName => GeneratedClassNameString;
		protected override string GeneratedBaseClassName => GeneratedBaseClassNameString;
		protected override T4CSharpCodeGenerationInfoCollectorBase Collector { get; }

		protected override void AppendParameterInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			StringBuilder builder
		)
		{
		}
	}
}
