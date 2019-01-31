using System;
using System.Linq;
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace GammaJul.ReSharper.ForTea.Daemon {

	public class T4CSharpErrorProcess : CSharpIncrementalDaemonStageProcessBase {

		public override void VisitClassDeclaration(IClassDeclaration classDeclarationParam, IHighlightingConsumer context) {
			base.VisitClassDeclaration(classDeclarationParam, context);

			if (!classDeclarationParam.IsSynthetic()
			|| !T4CSharpCodeGenerator.ClassName.Equals(classDeclarationParam.DeclaredName, StringComparison.Ordinal))
				return;

			IDeclaredTypeUsage superTypeUsage = classDeclarationParam.SuperTypeUsageNodes.FirstOrDefault();
			if (superTypeUsage == null
			|| T4CSharpCodeGenerator.DefaultBaseClassName.Equals(superTypeUsage.GetText(), StringComparison.Ordinal))
				return;

			ITypeElement typeElement = CSharpTypeFactory.CreateDeclaredType(superTypeUsage).GetTypeElement();
			if (typeElement == null)
				return;
			
			if (!HasTransformTextMethod(typeElement))
				context.AddHighlighting(new MissingTransformTextMethodHighlighting(superTypeUsage));
		}

		private static bool HasTransformTextMethod([NotNull] ITypeElement typeElement)
			=> typeElement
				.GetAllClassMembers(T4CSharpCodeGenerator.TransformTextMethodName)
				.SelectNotNull(instance => instance.Member as IMethod)
				.Any(IsTransformTextMethod);

		private static bool IsTransformTextMethod([NotNull] IMethod method)
			=> method.ShortName == T4CSharpCodeGenerator.TransformTextMethodName
			&& (method.IsVirtual || method.IsOverride || method.IsAbstract)
			&& !method.IsSealed
			&& method.GetAccessRights() == AccessRights.PUBLIC
			&& method.ReturnType.IsString()
			&& method.Parameters.Count == 0;

		public T4CSharpErrorProcess([NotNull] IDaemonProcess process, [NotNull] IContextBoundSettingsStore settingsStore, [NotNull] ICSharpFile file)
			: base(process, settingsStore, file) {
		}

	}

}