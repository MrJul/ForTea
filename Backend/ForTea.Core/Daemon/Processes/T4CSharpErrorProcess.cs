using System;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Processes {
	
	public class T4CSharpErrorProcess : CSharpIncrementalDaemonStageProcessBase {

		public override void VisitClassDeclaration(IClassDeclaration classDeclarationParam, IHighlightingConsumer context) {
			base.VisitClassDeclaration(classDeclarationParam, context);

			if (!classDeclarationParam.IsSynthetic()) return;
			if (!T4CSharpCodeBehindIntermediateConverter.GeneratedClassNameString.Equals(
				classDeclarationParam.DeclaredName, StringComparison.Ordinal))
				return;

			ITypeUsage baseClassNode = classDeclarationParam.SuperTypeUsageNodes.FirstOrDefault();
			if (baseClassNode == null) return;

			if (T4CSharpCodeBehindIntermediateConverter.GeneratedBaseClassNameString.Equals(
				baseClassNode.GetText(),
				StringComparison.Ordinal)) return;

			ITypeElement baseClass = classDeclarationParam.SuperTypes.FirstOrDefault()?.GetTypeElement();
			if (baseClass == null) return;

			if (HasTransformTextMethod(baseClass)) return;
			context.AddHighlighting(new MissingTransformTextMethodHighlighting(baseClassNode, baseClass));
		}

		private static bool HasTransformTextMethod([NotNull] ITypeElement typeElement)
			=> typeElement
				.GetAllClassMembers(T4CSharpIntermediateConverterBase.TransformTextMethodName)
				.SelectNotNull(instance => instance.Member as IMethod)
				.Any(IsTransformTextMethod);

		private static bool IsTransformTextMethod([NotNull] IMethod method)
			=> method.ShortName == T4CSharpIntermediateConverterBase.TransformTextMethodName
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
