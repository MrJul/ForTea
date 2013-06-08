#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion


using System;
using System.Globalization;
using System.Linq;
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.Intentions.CreateDeclaration;
using JetBrains.ReSharper.Feature.Services.Intentions.DataProviders;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Intentions.QuickFixes {

	[QuickFix]
	public class CreateTransformTextMethodQuickFix : QuickFixBase {

		private readonly MissingTransformTextMethodHighlighting _highlighting;

		public override bool IsAvailable(IUserDataHolder cache) {
			return GetTargetTypeDeclaration(_highlighting.DeclaredTypeUsage) != null;
		}

		public override string Text {
			get { return String.Format(CultureInfo.InvariantCulture, "Create method '{0}'", T4CSharpCodeGenerator.TransformTextMethodName); }
		}

		[CanBeNull]
		private static ITypeDeclaration GetTargetTypeDeclaration([NotNull] IDeclaredTypeUsage declaredTypeUsage) {
			if (!declaredTypeUsage.IsValid())
				return null;

			ITypeElement typeElement = CSharpTypeFactory.CreateDeclaredType(declaredTypeUsage).GetTypeElement();
			if (typeElement == null)
				return null;

			return typeElement.GetDeclarations()
				.OfType<ITypeDeclaration>()
				.FirstOrDefault(decl => LanguageManager.Instance.TryGetService<ICreateMethodDeclarationIntention>(decl.Language) != null);
		}

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			ITypeDeclaration typeDeclaration = GetTargetTypeDeclaration(_highlighting.DeclaredTypeUsage);
			if (typeDeclaration == null)
				return null;

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
				MethodName = T4CSharpCodeGenerator.TransformTextMethodName,
				SourceReferenceExpressionReference = null,
				Target = target,
			};

			var createMethodDeclarationIntention = LanguageManager.Instance.GetService<ICreateMethodDeclarationIntention>(typeDeclaration.Language);
			IntentionResult intentionResult = createMethodDeclarationIntention.ExecuteEx(context);
			intentionResult.ExecuteTemplate();
			return null;
		}

		[NotNull]
		private static MemberSignature CreateTransformTextSignature([NotNull] ITreeNode node) {
			var signatureProvider = new MemberSignatureProvider(node.GetPsiServices(), node.Language);
			PredefinedType predefinedType = node.GetPsiModule().GetPredefinedType();
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