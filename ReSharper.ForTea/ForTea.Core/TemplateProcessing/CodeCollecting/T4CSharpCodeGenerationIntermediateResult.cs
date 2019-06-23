using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public sealed class T4CSharpCodeGenerationIntermediateResult
	{
		[NotNull]
		public T4CSharpCodeGenerationResult CollectedImports { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult CollectedBaseClass { get; }

		[NotNull]
		public T4CSharpCodeGenerationResult CollectedTransformation { get; }
		
		[NotNull]
		public T4CSharpCodeGenerationResult CollectedFeatures { get; }
		
		public bool FeatureStarted { get; private set; }
		
		public bool HasHost { get; private set; }

		public bool HasBaseClass => !CollectedBaseClass.Builder.IsEmpty();

		public void StartFeature() => FeatureStarted = true;

		public void RequireHost() => HasHost = true;
		
		public T4CSharpCodeGenerationIntermediateResult([NotNull] IT4File file)
		{
			CollectedImports = new T4CSharpCodeGenerationResult(file);
			CollectedBaseClass = new T4CSharpCodeGenerationResult(file);
			CollectedTransformation = new T4CSharpCodeGenerationResult(file);
			CollectedFeatures = new T4CSharpCodeGenerationResult(file);
			FeatureStarted = false;
			HasHost = false;
		}

		public void Append([NotNull] T4CSharpCodeGenerationIntermediateResult other)
		{
			CollectedFeatures.Append(other.CollectedImports);
			// base class is intentionally ignored
			CollectedTransformation.Append(other.CollectedTransformation);
			CollectedFeatures.Append(other.CollectedFeatures);
			// 'feature started' is intentionally ignored
			HasHost = HasHost || other.HasHost;
		}
	}
}
