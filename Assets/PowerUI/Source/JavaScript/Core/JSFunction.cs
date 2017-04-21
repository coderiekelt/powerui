using System;


namespace JavaScript{

	/// <summary>
	/// A last resort delegate, used when the compiler cannot establish a suitable delegate to subscribe a method to.
	/// Note that the first argument is always a 'this' override (which may be null).
	/// </summary>
	public delegate object JSFunction(params object[] arguments);
	
	
	/// <summary>
	/// An attribute added to JS compiled functions; the compiler emits these.
	/// </summary>
	public class JSFunctionAttribute : Attribute{
		
		/// <summary>Location of this function in the source.</summary>
		public int SourceLocation;
		
		public JSFunctionAttribute(int sourceLocation){
			SourceLocation=sourceLocation;
		}
		
	}
	
}