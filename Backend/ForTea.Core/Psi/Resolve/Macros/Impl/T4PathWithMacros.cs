using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GammaJul.ForTea.Core.Psi.Modules;
using JetBrains.Annotations;
using JetBrains.Collections;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl
{
	public sealed class T4PathWithMacros : IT4PathWithMacros
	{
		[NotNull]
		private static Regex MacroRegex { get; } =
			new Regex(@"\$\((\w+)\)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		private string RawPath { get; }
		
		[NotNull]
		private IPsiSourceFile SourceFile { get; }

		[NotNull]
		private IT4MacroResolver Resolver { get; }

		[NotNull]
		private IT4Environment Environment { get; }
		
		[NotNull]
		private ISolution Solution { get; }

		[CanBeNull]
		private IT4FilePsiModule Module => SourceFile.PsiModule as IT4FilePsiModule;

		public T4PathWithMacros([NotNull] string rawPath, [NotNull] IPsiSourceFile file)
		{
			RawPath = rawPath;
			SourceFile = file;
			Solution = SourceFile.GetSolution();
			Resolver = Solution.GetComponent<IT4MacroResolver>();
			Environment = Solution.GetComponent<IT4Environment>();
		}

		public IPsiSourceFile Resolve() => ResolvePath().FindSourceFileInSolution(Solution);

		public FileSystemPath ResolvePath()
		{
			string expanded = ExpandMacros();

			// search as absolute path
			var asAbsolutePath = FileSystemPath.TryParse(expanded);
			if (asAbsolutePath.IsAbsolute) return asAbsolutePath;

			// search as relative path
			var asRelativePath = SourceFile.GetLocation().Directory.Combine(expanded);
			if (asRelativePath.ExistsFile) return asRelativePath;

			// search in global include paths
			var asGlobalInclude = Environment.IncludePaths
				.Select(includePath => includePath.Combine(expanded))
				.FirstOrDefault(resultPath => resultPath.ExistsFile);

			return asGlobalInclude ?? asRelativePath;
		}

		[NotNull]
		private string ExpandEnvironmentVariables() => System.Environment.ExpandEnvironmentVariables(RawPath);

		[NotNull]
		private string ExpandMacros()
		{
			var module = Module;
			if (string.IsNullOrEmpty(RawPath) || module == null || !ContainsMacros) return RawPath;

			var macroValues = Resolver.Resolve(RawMacros, SourceFile.ToProjectFile().NotNull());

			var result = new StringBuilder(ExpandEnvironmentVariables());
			foreach ((string macro, string value) in macroValues)
			{
				result.Replace(macro, value);
			}

			return result.ToString();
		}

		private bool ContainsMacros
		{
			get
			{
				int lParen = RawPath.IndexOf("$(", StringComparison.Ordinal);
				int rParen = RawPath.IndexOf(")", StringComparison.Ordinal);
				return lParen >= 0 && rParen >= 0 && lParen <= rParen;
			}
		}

		private IEnumerable<string> RawMacros => MacroRegex
			.Matches(RawPath)
			.Cast<Match>()
			.Where(match => match.Success)
			.Select(match => match.Groups[1].Value);

		private bool Equals(T4PathWithMacros other) =>
			string.Equals(RawPath, other.RawPath) && SourceFile.Equals(other.SourceFile);

		public override bool Equals(object obj) =>
			ReferenceEquals(this, obj) || obj is T4PathWithMacros other && Equals(other);

		public override int GetHashCode()
		{
			unchecked
			{
				return ((RawPath != null ? RawPath.GetHashCode() : 0) * 397) ^ SourceFile.GetHashCode();
			}
		}

		public bool IsEmpty => RawPath.IsEmpty();
	}
}
