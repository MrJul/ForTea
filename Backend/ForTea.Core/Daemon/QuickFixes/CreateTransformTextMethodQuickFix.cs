using System;
using System.Globalization;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.Intentions.CreateDeclaration;
using JetBrains.ReSharper.Feature.Services.Intentions.DataProviders;
using JetBrains.ReSharper.Feature.Services.Intentions.Impl.DeclarationBuilders;
using JetBrains.ReSharper.Feature.Services.Intentions.Impl.LanguageSpecific;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes {

	[QuickFix]
	public class CreateTransformTextMethodQuickFix : QuickFixBase {

		[NotNull] private readonly MissingTransformTextMethodHighlighting _highlighting;

		public override bool IsAvailable(IUserDataHolder cache)
			=> GetTargetTypeDeclaration(_highlighting.BaseClass) != null;

		public override string Text
			=> string.Format(CultureInfo.InvariantCulture, "Create method '{0}'", T4CSharpIntermediateConverterBase.TransformTextMethodName);

		[CanBeNull]
		private static ITypeDeclaration GetTargetTypeDeclaration([NotNull] ITypeElement baseClass) {
			if (!baseClass.IsValid())
				return null;

			return baseClass
				.GetDeclarations()
				.OfType<ITypeDeclaration>()
				.FirstOrDefault(decl => LanguageManager.Instance.TryGetService<IntentionLanguageSpecific>(decl.Language) != null);
		}

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			ITypeDeclaration typeDeclaration = GetTargetTypeDeclaration(_highlighting.BaseClass);
			if (typeDeclaration == null) return null;

			MemberSignature signature = CreateTransformTextSignature(typeDeclaration);
			TypeTarget target = CreateTarget(typeDeclaration);

			var context = new CreateMethodDeclarationContext {
				AccessRights = AccessRights.PUBLIC,
				ExecuteTemplateOverMemberBody = false,
				ExecuteTemplateOverName = false,
				ExecuteTemplateOverParameters = false,
				ExecuteTemplateOverReturnType = false,
				IsAbstract = true,
				IsStatic = false,
				MethodSignatures = new[] { signature },
				MethodName = T4CSharpIntermediateConverterBase.TransformTextMethodName,
				SourceReferenceExpressionReference = null,
				Target = target,
			};

			IntentionResult intentionResult = MethodDeclarationBuilder.Create(context);
			intentionResult.ExecuteTemplate();
			return null;
		}

		[NotNull]
		private static MemberSignature CreateTransformTextSignature([NotNull] ITreeNode node) {
			var signatureProvider = new MemberSignatureProvider(node.GetPsiServices(), node.Language);
			PredefinedType predefinedType = node.GetPredefinedType();
			return signatureProvider.CreateFromTypes(predefinedType.String, EmptyList<IType>.InstanceList, node.GetSourceFile());
		}

		[NotNull]
		private static TypeTarget CreateTarget([NotNull] ITypeDeclaration typeDeclaration) {
			ITypeElement typeElement = typeDeclaration.DeclaredElement;
			Assertion.AssertNotNull(typeElement, "typeDeclaration.DeclaredElement != null");
			var target = new TypeTarget(typeElement);
			target.SetPart(typeDeclaration);
			return target;
		}

		public CreateTransformTextMethodQuickFix([NotNull] MissingTransformTextMethodHighlighting highlighting) {
			_highlighting = highlighting;
		}

	}

}
