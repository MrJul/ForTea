﻿#region License
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
using System.Linq;
using GammaJul.ReSharper.ForTea.Daemon.Highlightings;
using GammaJul.ReSharper.ForTea.Psi;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace GammaJul.ReSharper.ForTea.Daemon {

	public class T4CSharpErrorProcess : CSharpIncrementalDaemonStageProcessBase {

		public override void VisitClassDeclaration(IClassDeclaration classDeclarationParam, IHighlightingConsumer context) {
			base.VisitClassDeclaration(classDeclarationParam, context);

			IPsiSourceFile sourceFile = classDeclarationParam.GetSourceFile();
			string generatedClassName = T4CSharpCodeGenerator.GetClassName(sourceFile);
			if (!classDeclarationParam.IsSynthetic()
			|| !generatedClassName.Equals(classDeclarationParam.DeclaredName, StringComparison.Ordinal))
				return;

			IDeclaredTypeUsage superTypeUsage = classDeclarationParam.SuperTypeUsageNodes.FirstOrDefault();
			if (superTypeUsage == null
			|| T4CSharpCodeGenerator.DefaultBaseClassName.Equals(superTypeUsage.GetText(), StringComparison.Ordinal))
				return;

			ITypeElement typeElement = CSharpTypeFactory.CreateDeclaredType(superTypeUsage).GetTypeElement();
			if (typeElement == null)
				return;

			if (!typeElement.Methods.Any(IsTransformTextMethod))
				context.AddHighlighting(new MissingTransformTextMethodHighlighting(superTypeUsage));
		}

		private static bool IsTransformTextMethod([NotNull] IMethod method) {
			return method.ShortName == T4CSharpCodeGenerator.TransformTextMethodName
				&& (method.IsVirtual || method.IsOverride || method.IsAbstract)
				&& !method.IsSealed
				&& method.GetAccessRights() == AccessRights.PUBLIC
				&& method.ReturnType.IsString()
				&& method.Parameters.Count == 0;
		}

		public T4CSharpErrorProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, ICSharpFile file)
			: base(process, settingsStore, file) {
		}

	}

}