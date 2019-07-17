using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public class T4ParameterDescription
	{
		[NotNull]
		public ITreeNode TypeToken { get; }

		[NotNull]
		public ITreeNode NameToken { get; }

		[NotNull]
		public string TypeString { get; }

		[NotNull]
		public string NameString { get; }

		[NotNull]
		public string FieldNameString { get; }

		private T4ParameterDescription(
			[NotNull] ITreeNode typeToken,
			[NotNull] ITreeNode nameToken,
			[NotNull] string typeString,
			[NotNull] string nameString)
		{
			TypeToken = typeToken;
			NameToken = nameToken;
			TypeString = typeString.EscapeKeyword();
			NameString = nameString.EscapeKeyword();
			FieldNameString = $"_{nameString}Field";
		}

		[CanBeNull]
		public static T4ParameterDescription FromDirective(
			[NotNull] IT4Directive directive,
			[NotNull] T4DirectiveInfoManager manager
		)
		{
			var typeToken = directive.GetAttributeValueToken(manager.Parameter.TypeAttribute.Name);
			string typeText = typeToken?.GetText();
			var nameToken = directive.GetAttributeValueToken(manager.Parameter.NameAttribute.Name);
			string nameText = nameToken?.GetText();
			if (typeText?.IsNullOrEmpty() != false) return null;
			if (nameText?.IsNullOrEmpty() != false) return null;
			return new T4ParameterDescription(typeToken, nameToken, typeText, nameText);
		}
	}
}
