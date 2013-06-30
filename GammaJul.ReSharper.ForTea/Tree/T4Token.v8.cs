namespace GammaJul.ReSharper.ForTea.Tree {

	public partial class T4Token {
		
		public override bool IsFiltered() {
			return GetTokenType().IsFiltered;
		}

	}

}