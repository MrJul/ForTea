using System;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public abstract class T4InfoCollectorStateBase
	{
		[NotNull]
		public abstract T4InfoCollectorStateBase GetNextState([NotNull] ITreeNode element);

		[CanBeNull]
		public abstract string ConvertForAppending([NotNull] IT4Token token);
		
		public abstract bool FeatureStarted { get; }

		[NotNull]
		protected string Convert([NotNull] IT4Token token) => StringLiteralConverter.EscapeToRegular(token.GetText());
	}
}
