using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.Parsing.Builders
{
	/// <summary>Class containing all potential tokens for an attribute.</summary>
	internal sealed class AttributeInfo
	{
		[NotNull]
		private T4TreeBuilder Builder { get; }

		[CanBeNull]
		public AttributeInfo Next { get; set; }

		[CanBeNull]
		public TreeElement NameToken { get; set; }

		[CanBeNull]
		public TreeElement EqualToken { get; set; }

		[CanBeNull]
		public TreeElement OpeningQuoteToken { get; set; }

		[CanBeNull]
		public TreeElement ClosingQuoteToken { get; set; }

		private FrugalLocalList<TreeElement> ValueTokens;

		public AttributeInfo(T4TreeBuilder builder)
		{
			Builder = builder;
			ValueTokens = new FrugalLocalList<TreeElement>();
		}

		/// <summary>Gets whether the attribute has only a name.</summary>
		public bool HasNameOnly =>
			NameToken != null
			&& EqualToken == null
			&& OpeningQuoteToken == null
			&& ValueTokens.IsEmpty
			&& ClosingQuoteToken == null;

		/// <summary>Creates a new <see cref="T4DirectiveAttribute"/> from the current attribute info.</summary>
		/// <returns>An instance of <see cref="T4DirectiveAttribute"/>.</returns>
		[NotNull]
		public T4DirectiveAttribute ToAttribute()
		{
			var attribute = new T4DirectiveAttribute();

			if (NameToken != null)
			{
				Builder.AppendNewChild(attribute, NameToken);
				if (EqualToken != null)
					Builder.AppendNewChild(attribute, EqualToken);
				else
				{
					if (OpeningQuoteToken == null)
					{
						Assertion.Assert(ValueTokens.IsEmpty,
							"ValueTokens should be null if there's no opening quote.");
						Assertion.Assert(ClosingQuoteToken == null,
							"ClosingQuoteToken should be null if there's no opening quote.");
						Builder.AppendMissingToken(attribute, MissingTokenType.EqualSignAndAttributeValue);
						return attribute;
					}

					Builder.AppendMissingToken(attribute, MissingTokenType.EqualSign);
				}
			}
			else if (EqualToken != null)
			{
				Builder.AppendMissingToken(attribute, MissingTokenType.AttributeName);
				Builder.AppendNewChild(attribute, EqualToken);
			}
			else
				Builder.AppendMissingToken(attribute, MissingTokenType.AttributeNameAndEqualSign);

			if (OpeningQuoteToken == null)
			{
				Assertion.Assert(ValueTokens.IsEmpty, "ValueTokens should be null if there's no opening quote.");
				Assertion.Assert(ClosingQuoteToken == null,
					"ClosingQuoteToken should be null if there's no opening quote.");
				Builder.AppendMissingToken(attribute, MissingTokenType.AttributeValue);
				return attribute;
			}

			Builder.AppendNewChild(attribute, OpeningQuoteToken);
			if (!ValueTokens.IsEmpty)
				Builder.AppendNewChild(attribute, Builder.CreateAttributeValue(ValueTokens));
			if (ClosingQuoteToken != null)
				Builder.AppendNewChild(attribute, ClosingQuoteToken);
			else
				Builder.AppendMissingToken(attribute, MissingTokenType.Quote);

			return attribute;
		}

		public void AppendValue() => ValueTokens.Add(Builder.CreateCurrentToken());
	}
}
