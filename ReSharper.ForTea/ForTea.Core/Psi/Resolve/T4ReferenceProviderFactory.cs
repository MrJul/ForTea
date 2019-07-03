using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	[ReferenceProviderFactory]
	public class T4ReferenceProviderFactory : IReferenceProviderFactory
	{
		public ISignal<IReferenceProviderFactory> Changed { get; }

		// ReSharper disable once AssignNullToNotNullAttribute
		public T4ReferenceProviderFactory(Lifetime lifetime) =>
			Changed = new Signal<IReferenceProviderFactory>(lifetime, GetType().FullName);

		public IReferenceFactory CreateFactory(IPsiSourceFile sourceFile, IFile file, IWordIndex wordIndexForChecks) =>
			sourceFile.PrimaryPsiLanguage.Is<T4Language>() ? new T4ReferenceFactory() : null;
	}
}
