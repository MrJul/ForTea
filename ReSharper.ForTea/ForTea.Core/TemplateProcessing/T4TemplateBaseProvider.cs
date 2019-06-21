using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.Application;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	[ShellComponent]
	public class T4TemplateBaseProvider
	{
		[NotNull, Obsolete] internal const string DefaultBaseClassName = "TextTransformation";
		[NotNull] private const string InitialClassNamePlaceholder = "TemplateBase";

		[NotNull]
		private string BaseClassFormat { get; }

		[NotNull]
		private static string ReadBaseClass()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			const string name = "GammaJul.ForTea.Core.TemplateProcessing.TemplateBase.cs";
			using (Stream stream = assembly.GetManifestResourceStream(name))
			{
				if (stream == null) return "class {0} {{ }}}";
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		[NotNull]
		public string CreateTemplateBase([CanBeNull] string baseClassName) =>
			BaseClassFormat.Replace(InitialClassNamePlaceholder, baseClassName);

		public T4TemplateBaseProvider() => BaseClassFormat = ReadBaseClass();
	}
}
