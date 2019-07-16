using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.Parsing {
	
	internal sealed partial class T4TreeBuilder {

		/// <summary>Smart directive builder that group attributes logically even with missing tokens.</summary>
		private struct DirectiveBuilder {

			[NotNull] private readonly T4TreeBuilder _treeBuilder;

			[CanBeNull] private AttributeInfo _firstInfo;
			[CanBeNull] private AttributeInfo _currentInfo;

			/// <summary>Class containing all potential tokens for an attribute.</summary>
			private sealed class AttributeInfo {

				[NotNull] private readonly T4TreeBuilder _builder;

				[CanBeNull] public AttributeInfo Next;
				[CanBeNull] public TreeElement NameToken;
				[CanBeNull] public TreeElement EqualToken;
				[CanBeNull] public TreeElement OpeningQuoteToken;
				public FrugalLocalList<TreeElement> ValueTokens;
				[CanBeNull] public TreeElement ClosingQuoteToken;

				/// <summary>Gets whether the attribute has only a name.</summary>
				public bool HasNameOnly
					=> NameToken != null
					&& EqualToken == null
					&& OpeningQuoteToken == null
					&& ValueTokens.IsEmpty
					&& ClosingQuoteToken == null;

				/// <summary>Creates a new <see cref="T4DirectiveAttribute"/> from the current attribute info.</summary>
				/// <returns>An instance of <see cref="T4DirectiveAttribute"/>.</returns>
				[NotNull]
				public T4DirectiveAttribute ToAttribute() {
					var attribute = new T4DirectiveAttribute();

					if (NameToken != null) {
						_builder.AppendNewChild(attribute, NameToken);
						if (EqualToken != null)
							_builder.AppendNewChild(attribute, EqualToken);
						else {
							if (OpeningQuoteToken == null) {
								Assertion.Assert(ValueTokens.IsEmpty, "ValueTokens should be null if there's no opening quote.");
								Assertion.Assert(ClosingQuoteToken == null, "ClosingQuoteToken should be null if there's no opening quote.");
								_builder.AppendMissingToken(attribute, MissingTokenType.EqualSignAndAttributeValue);
								return attribute;
							}
							_builder.AppendMissingToken(attribute, MissingTokenType.EqualSign);
						}
					}
					else if (EqualToken != null) {
						_builder.AppendMissingToken(attribute, MissingTokenType.AttributeName);
						_builder.AppendNewChild(attribute, EqualToken);
					}
					else
						_builder.AppendMissingToken(attribute, MissingTokenType.AttributeNameAndEqualSign);

					if (OpeningQuoteToken == null) {
						Assertion.Assert(ValueTokens.IsEmpty, "ValueTokens should be null if there's no opening quote.");
						Assertion.Assert(ClosingQuoteToken == null, "ClosingQuoteToken should be null if there's no opening quote.");
						_builder.AppendMissingToken(attribute, MissingTokenType.AttributeValue);
						return attribute;
					}

					_builder.AppendNewChild(attribute, OpeningQuoteToken);
					if (!ValueTokens.IsEmpty)
						_builder.AppendNewChild(attribute, ValueTokens);
					if (ClosingQuoteToken != null)
						_builder.AppendNewChild(attribute, ClosingQuoteToken);
					else
						_builder.AppendMissingToken(attribute, MissingTokenType.Quote);

					return attribute;
				}

				public AttributeInfo(T4TreeBuilder builder) {
					_builder = builder;
					ValueTokens = new FrugalLocalList<TreeElement>();
				}

			}

			/// <summary>Creates a new <see cref="AttributeInfo"/> linked to the previous one.</summary>
			/// <returns>A new <see cref="AttributeInfo"/>.</returns>
			[NotNull]
			private AttributeInfo CreateNewInfo() {
				var info = new AttributeInfo(_treeBuilder);
				if (_firstInfo == null)
					_firstInfo = info;
				if (_currentInfo != null)
					_currentInfo.Next = info;
				return info;
			}

			/// <summary>Adds a name token from the current token.</summary>
			public void AddName() {
				_currentInfo = CreateNewInfo();
				_currentInfo.NameToken = _treeBuilder.CreateCurrentToken();
			}

			/// <summary>Adds an equal token from the current token.</summary>
			public void AddEqual() {
				if (_currentInfo == null || _currentInfo.EqualToken != null)
					_currentInfo = CreateNewInfo();
				_currentInfo.EqualToken = _treeBuilder.CreateCurrentToken();
			}

			/// <summary>Adds a quote token from the current token.</summary>
			public void AddQuote() {
				if (_currentInfo == null || _currentInfo.ClosingQuoteToken != null)
					_currentInfo = CreateNewInfo();
				if (_currentInfo.OpeningQuoteToken == null)
					_currentInfo.OpeningQuoteToken = _treeBuilder.CreateCurrentToken();
				else
					_currentInfo.ClosingQuoteToken = _treeBuilder.CreateCurrentToken();
			}

			/// <summary>Adds a value token from the current token.</summary>
			public void AddValue() {
				if (_currentInfo == null)
					_currentInfo = CreateNewInfo();
				_currentInfo.ValueTokens.Add(_treeBuilder.CreateCurrentToken());
			}

			/// <summary>Finishes building the directive by appending its name and all attributes.</summary>
			/// <param name="parent">The directive.</param>
			public void Complete([NotNull] CompositeElement parent) {
				if (_firstInfo == null) {
					_treeBuilder.AppendMissingToken(parent, MissingTokenType.DirectiveName);
					return;
				}

				AttributeInfo attrInfo;
				if (_firstInfo.HasNameOnly) {
					_treeBuilder.AppendNewChild(parent, _firstInfo.NameToken);
					attrInfo = _firstInfo.Next;
				}
				else {
					attrInfo = _firstInfo;
					_treeBuilder.AppendMissingToken(parent, MissingTokenType.DirectiveName);
				}

				while (attrInfo != null) {
					_treeBuilder.AppendNewChild(parent, attrInfo.ToAttribute());
					attrInfo = attrInfo.Next;
				}
			}

			/// <summary>Initializes a new instance of the <see cref="DirectiveBuilder"/> struct.</summary>
			/// <param name="treeBuilder">The tree builder owning this builder.</param>
			public DirectiveBuilder([NotNull] T4TreeBuilder treeBuilder) {
				_treeBuilder = treeBuilder;
				_currentInfo = null;
				_firstInfo = null;
			}

		}

	}

}
