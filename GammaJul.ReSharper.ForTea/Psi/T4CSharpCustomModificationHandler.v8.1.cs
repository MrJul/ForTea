using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.Psi.CSharp.Impl.CustomHandlers;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ReSharper.ForTea.Psi {

	public partial class T4CSharpCustomModificationHandler {
	    public ThisQualifierStyle GetThisQualifierStyle(ITreeNode context)
	    {
	        return context.GetSettingsStore().GetValue(CSharpCustomModificationHandlerDummy.ThisQualifierStyleAccessor);
	    }
	}
}