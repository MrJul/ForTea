namespace GammaJul.ForTea.Core.Tree {

	/// <summary>Represents a token role inside a composite element.</summary>
	public enum T4TokenRole : short {
		Unknown = 0,
		BlockStart = 1,
		BlockEnd = 2,
		Code = 3,
		Name = 4,
		Value = 5,
		Attribute = 6,
		Separator = 7
	}

}