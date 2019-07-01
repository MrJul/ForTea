using System;
using System.Diagnostics;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public abstract class T4InfoCollectorStateBase : IT4InfoCollectorState
	{
		// This property is only required for ensuring correctness of state management code
		private bool IsAlive { get; set; } = true;

		[Conditional("JET_MODE_ASSERT")]
		protected void Die() => IsAlive = false;

		[Conditional("JET_MODE_ASSERT")]
		private void Check() => Assertion.Assert(IsAlive, "Attempted to use dead state");

		public string Produce()
		{
			Check();
			return ProduceSafe();
		}

		public IT4InfoCollectorState GetNextState(ITreeNode element)
		{
			Check();
			return GetNextStateSafe(element);
		}

		public bool FeatureStarted
		{
			get
			{
				Check();
				return FeatureStartedSafe;
			}
		}

		public void ConsumeToken(IT4Token token)
		{
			Check();
			ConsumeTokenSafe(token);
		}

		protected abstract void ConsumeTokenSafe([NotNull] IT4Token token);

		[CanBeNull]
		protected abstract string ProduceSafe();

		protected abstract bool FeatureStartedSafe { get; }

		[NotNull]
		protected abstract IT4InfoCollectorState GetNextStateSafe([NotNull] ITreeNode element);

		[NotNull]
		protected static string Convert([NotNull] IT4Token token) => StringLiteralConverter.EscapeToRegular(
			token.NodeType == T4TokenNodeTypes.NewLine
				? Environment.NewLine // todo: use \n and change it to environmental newline later
				: token.GetText());
	}
}
