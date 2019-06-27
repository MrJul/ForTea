    /// <summary>
    /// Base class for this transformation
    /// </summary>
    public class TEMPLATE_PLACEHOLDER_BASE_CLASS
    {
    	/// <summary>
    	/// The string builder that generation-time code is using to assemble generated output
    	/// </summary>
    	protected System.Text.StringBuilder GenerationEnvironment { get; set; }

        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors => null;

        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent => null;
    
    	/// <summary>
    	/// Current transformation session
    	/// </summary>
    	public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
	        get => null;
    		set { }
    	}

        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
        }

        /// <summary>
    	/// Write text directly into the generated output
    	/// </summary>
    	public void WriteLine(string textToAppend)
    	{
    	}
    
    	/// <summary>
    	/// Write formatted text directly into the generated output
    	/// </summary>
    	public void Write(string format, params object[] args)
    	{
    	}
    
    	/// <summary>
    	/// Write formatted text directly into the generated output
    	/// </summary>
    	public void WriteLine(string format, params object[] args)
    	{
    	}
    
    	/// <summary>
    	/// Raise an error
    	/// </summary>
    	public void Error(string message)
    	{
    	}
    
    	/// <summary>
    	/// Raise a warning
    	/// </summary>
    	public void Warning(string message)
    	{
    	}
    
    	/// <summary>
    	/// Increase the indent
    	/// </summary>
    	public void PushIndent(string indent)
    	{
    	}

        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent() => null;
    
    	/// <summary>
    	/// Remove any indentation
    	/// </summary>
    	public void ClearIndent()
    	{
    	}

        [__ReSharperSynthetic]
        protected string __ToString(object value) => value.ToString();
    }
