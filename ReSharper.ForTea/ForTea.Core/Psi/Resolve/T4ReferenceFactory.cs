using GammaJul.ForTea.Core.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	public class T4ReferenceFactory : IReferenceFactory
	{
		public ReferenceCollection GetReferences(ITreeNode element, ReferenceCollection oldReferences)
		{
			if (!element.Language.Is<T4Language>()) return ReferenceCollection.Empty;
			if (!(element is IT4DirectiveAttribute attribute)) return ReferenceCollection.Empty;
			var path = attribute.Reference;
			if (path == null) return ReferenceCollection.Empty;
			var value = attribute.GetValueToken();
			if (value == null) return ReferenceCollection.Empty;
			return new ReferenceCollection(new T4FileReference(attribute, value, path));
		}

		public bool HasReference(ITreeNode element, IReferenceNameContainer names) => true;
	}
}
