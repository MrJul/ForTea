using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl {

	/// <summary>Implementation of <see cref="ErrorElement"/> for a missing block end tag.</summary>
	public class MissingTokenErrorElement : ErrorElement {

		public MissingTokenType MissingTokenType { get; }

		private static string GetErrorMessage(MissingTokenType missingTokenType) {
			switch (missingTokenType) {
				case MissingTokenType.BlockEnd: return "Missing block end";
				case MissingTokenType.DirectiveName: return "Missing directive name";
				case MissingTokenType.AttributeName: return "Missing attribute name";
				case MissingTokenType.AttributeNameAndEqualSign: return "Missing attribute name and equal sign";
				case MissingTokenType.EqualSign: return "Missing equal sign";
				case MissingTokenType.AttributeValue: return "Missing attribute value";
				case MissingTokenType.EqualSignAndAttributeValue: return "Missing equal sign and attribute value";
				case MissingTokenType.Quote: return "Missing quote";
				default: return missingTokenType.ToString();
			}
		}
		
		/// <summary>Initializes a new instance of the <see cref="MissingTokenErrorElement"/> class.</summary>
		public MissingTokenErrorElement(MissingTokenType missingTokenType)
			: base(GetErrorMessage(missingTokenType)) {
			MissingTokenType = missingTokenType;
		}
	}

}
