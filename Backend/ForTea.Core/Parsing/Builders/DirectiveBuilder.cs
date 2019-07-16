using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Parsing.Builders
{
	/// <summary>Smart directive builder that group attributes logically even with missing tokens.</summary>
	internal struct DirectiveBuilder
	{
		[NotNull]
		private T4TreeBuilder TreeBuilder { get; }

		[CanBeNull]
		private AttributeInfo FirstInfo { get; set; }

		[CanBeNull]
		private AttributeInfo CurrentInfo { get; set; }

		/// <summary>Initializes a new instance of the <see cref="DirectiveBuilder"/> struct.</summary>
		/// <param name="treeBuilder">The tree builder owning this builder.</param>
		public DirectiveBuilder([NotNull] T4TreeBuilder treeBuilder)
		{
			TreeBuilder = treeBuilder;
			CurrentInfo = null;
			FirstInfo = null;
		}

		/// <summary>Creates a new <see cref="AttributeInfo"/> linked to the previous one.</summary>
		/// <returns>A new <see cref="AttributeInfo"/>.</returns>
		[NotNull]
		private AttributeInfo CreateNewInfo()
		{
			var info = new AttributeInfo(TreeBuilder);
			if (FirstInfo == null)
				FirstInfo = info;
			if (CurrentInfo != null)
				CurrentInfo.Next = info;
			return info;
		}

		/// <summary>Adds a name token from the current token.</summary>
		public void AddName()
		{
			CurrentInfo = CreateNewInfo();
			CurrentInfo.NameToken = TreeBuilder.CreateCurrentToken();
		}

		/// <summary>Adds an equal token from the current token.</summary>
		public void AddEqual()
		{
			if (CurrentInfo == null || CurrentInfo.EqualToken != null)
				CurrentInfo = CreateNewInfo();
			CurrentInfo.EqualToken = TreeBuilder.CreateCurrentToken();
		}

		/// <summary>Adds a quote token from the current token.</summary>
		public void AddQuote()
		{
			if (CurrentInfo == null || CurrentInfo.ClosingQuoteToken != null)
				CurrentInfo = CreateNewInfo();
			if (CurrentInfo.OpeningQuoteToken == null)
				CurrentInfo.OpeningQuoteToken = TreeBuilder.CreateCurrentToken();
			else
				CurrentInfo.ClosingQuoteToken = TreeBuilder.CreateCurrentToken();
		}

		/// <summary>Adds a value token from the current token.</summary>
		public void AddValue()
		{
			if (CurrentInfo == null)
				CurrentInfo = CreateNewInfo();
			CurrentInfo.AppendValue();
		}

		/// <summary>Finishes building the directive by appending its name and all attributes.</summary>
		/// <param name="parent">The directive.</param>
		public void Complete([NotNull] CompositeElement parent)
		{
			if (FirstInfo == null)
			{
				TreeBuilder.AppendMissingToken(parent, MissingTokenType.DirectiveName);
				return;
			}

			AttributeInfo attrInfo;
			if (FirstInfo.HasNameOnly)
			{
				TreeBuilder.AppendNewChild(parent, FirstInfo.NameToken);
				attrInfo = FirstInfo.Next;
			}
			else
			{
				attrInfo = FirstInfo;
				TreeBuilder.AppendMissingToken(parent, MissingTokenType.DirectiveName);
			}

			while (attrInfo != null)
			{
				TreeBuilder.AppendNewChild(parent, attrInfo.ToAttribute());
				attrInfo = attrInfo.Next;
			}
		}
	}
}
