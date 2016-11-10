//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// A particular script engine. Derive from this if you wish to create your own.
	/// </summary>
	
	public class ScriptEngine{
		
		/// <summary>All code in script tags is buffered and compiled in one go. This is the buffer.</summary>
		public string[] CodeBuffer;
		/// <summary>The document this engine is for (if any).</summary>
		public Document Document;
		
		
		/// <summary>The HTML document this engine is for (if any).</summary>
		public HtmlDocument htmlDocument{
			get{
				return Document as HtmlDocument;
			}
		}
		
		/// <summary>The global scripting scope.</summary>
		public object GlobalScope{
			get{
				return Document.GlobalScope;
			}
		}
		
		/// <summary>Adds the given nitro code to the document. Used by script tags.
		/// Note that this will not work at runtime if the code if this document has already been compiled.</summary>
		/// <param name="code">The nitro code to add to the document.</param>
		public void AddCode(string code){
			AddCode(code,-1);
		}
		
		/// <summary>Adds the given Nitro code to the engine. Used by script tags.
		/// Note that this will not work at runtime if the code if this document has already been compiled.</summary>
		/// <param name="code">The nitro code to add to the document.</param>
		/// <param name="index">The index in the code buffer to add the code into.</param>
		public void AddCode(string code,int index){
			if(index==-1){
				index=GetCodeIndex();
			}
			CodeBuffer[index]=code;
		}
		
		/// <summary>Gets a new code index in the CodeBuffer array.</summary>
		/// <returns>A new index in the CodeBuffer array.</returns>
		public int GetCodeIndex(){
			int length=0;
			if(CodeBuffer!=null){
				length=CodeBuffer.Length;
			}
			string[] newInstances=new string[length+1];
			for(int i=0;i<length;i++){
				newInstances[i]=CodeBuffer[i];
			}
			CodeBuffer=newInstances;
			return length;
		}
		
		/// <summary>The meta types that your engine will handle. E.g. "text/javascript".</summary>
		public virtual string[] GetTypes(){
			return null;
		}
		
		/// <summary>When a type of script is encountered on the document, this is called to instance a script engine.
		/// You can share a single global instance if you wish by returning this. Return null if the engine can't be used
		/// by the given document.</summary>
		public virtual ScriptEngine Instance(Document document){
			return this;
		}
		
		public bool AotDocument{
			get{
				return (Document as PowerUI.HtmlDocument).AotDocument;
			}
		}
		
		/// <summary>Loads the given textual code for the given document. PowerUI ensures order for you 
		/// including when scripts are downloaded from the internet.</summary>
		protected virtual void Compile(string code){
			
		}
		
		/// <summary>Gets or sets script variable values.</summary>
		/// <param name="index">The name of the variable.</param>
		/// <returns>The variable value.</returns>
		public virtual object this[string global]{
			get{
				throw new NotImplementedException("This script engine doesn't support editing globals.");
			}
			set{
				throw new NotImplementedException("This script engine doesn't support editing globals.");
			}
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="context">The context to use for the 'this' value.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public virtual object RunLiteral(string name,object context,object[] args,bool optional){
			throw new NotImplementedException("This script engine doesn't support calling methods.");
		}
		
		/// <summary>Runs a nitro function by name with optional arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">Optional arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object Run(string name,params object[] args){
			return RunLiteral(name,GlobalScope,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object[] args){
			return RunLiteral(name,GlobalScope,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="context">The context to use for the 'this' value.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object context,object[] args){
			return RunLiteral(name,context,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object[] args,bool optional){
			return RunLiteral(name,GlobalScope,args,optional);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="context">The context to use for the 'this' value.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunOptionally(string name,object context,params object[] args){
			return RunLiteral(name,context,args,true);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunOptionally(string name,params object[] args){
			return RunLiteral(name,GlobalScope,args,true);
		}
		
		/// <summary>Attempts to compile the code. It's only successful if there are no nulls in the code buffer.</summary>
		/// <returns>Returns false if we're still waiting on code to download.</returns>
		public bool TryCompile(){
			
			if(CodeBuffer==null){
				return true;
			}
			
			if(CodeBuffer.Length==0){
				CodeBuffer=null;
				return true;
			}
			
			for(int i=0;i<CodeBuffer.Length;i++){
				if(CodeBuffer[i]==null){
					return false;
				}
			}
			
			// Good to go!
			string codeToCompile="";
			
			for(int i=0;i<CodeBuffer.Length;i++){
				codeToCompile+=CodeBuffer[i]+"\n";
			}
			
			CodeBuffer=null;
			
			Compile(codeToCompile);
			
			// We attempted the compilation - all ok:
			return true;
		}
		
	}
	
}