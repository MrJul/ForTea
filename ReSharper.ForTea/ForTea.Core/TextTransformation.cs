using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[__ReSharperSynthetic]
// ReSharper disable once CheckNamespace
public abstract class TextTransformation : IDisposable
{
	protected CompilerErrorCollection Errors = new CompilerErrorCollection();
	protected IDictionary<string, object> Session { get; } = new Dictionary<string, object>();

	protected TextTransformation()
	{
	}

	protected void Initialize()
	{
	}

	public void Dispose()
	{
	}

	protected void Error(string message)
	{
	}
	
	#region Indentation
	private Stack<string> indents = new Stack<string>();

	protected string CurrentIndent => indents.Aggregate(
		new StringBuilder(),
		(builder, indent) => builder.Append(indent),
		builder => builder.ToString());

	protected void ClearIndent() => indents.Clear();
	protected void PushIndent(string indent) => indents.Push(indent);
	protected void PopIndent() => indents.Pop();
	#endregion Indentation

	#region Writing
	private void AppendIndent() => GenerationEnvironment.Append(CurrentIndent);
	protected StringBuilder GenerationEnvironment { get; } = new StringBuilder();

	protected void Write(string textToAppend)
	{
		AppendIndent();
		GenerationEnvironment.Append(textToAppend);
	}

	protected void WriteLine(string textToAppend)
	{
		AppendIndent();
		GenerationEnvironment.AppendLine(textToAppend);
	}

	protected void Write(string format, params object[] args)
	{
		AppendIndent();
		GenerationEnvironment.AppendFormat(format, args);
	}

	protected void WriteLine(string format, params object[] args)
	{
		AppendIndent();
		GenerationEnvironment.AppendFormat(format, args);
		GenerationEnvironment.AppendLine();
	}
	#endregion Writing

	public abstract string TransformText();
}
