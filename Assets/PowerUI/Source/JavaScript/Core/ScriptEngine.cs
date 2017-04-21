using System;
using System.Collections.Generic;
using JavaScript;

namespace JavaScript{
	
	/// <summary>
	/// Represents the JavaScript script engine.  This is the first object that needs to be
	/// instantiated in order to execute javascript code.
	/// </summary>
	public partial class ScriptEngine : Runtime{
		
		/// <summary>If defined, a directory where compiled JS is cached. (does not end with /).</summary>
		public static string CachePath="Assets/PowerUI/CachedJS";
		
		/// <summary>
		/// True if this engine is allowed full access to all available methods.
		/// I.e. when a global isn't recognised, the engine will attempt to resolve it into
		/// an available C# type. True by default.</summary>
		public bool FullAccess=true;
		
		/// <summary>
		/// A unique identifier for the compiling script.
		/// </summary>
		internal string UniqueID;
		/// <summary>
		/// Gets or sets a value that indicates whether to disassemble any generated IL and store it
		/// in the associated function.
		/// </summary>
		public static bool EnableILAnalysis;
		
		/// <summary>
		/// Gets or sets a value which indicates whether debug information should be generated.  If
		/// this is set to <c>true</c> performance and memory usage are negatively impacted.
		/// </summary>
		public static bool EnableDebugging;
		
		/// <summary>
		/// Gets or sets a value that indicates whether to force ECMAScript 5 strict mode, even if
		/// the code does not contain a strict mode directive ("use strict").  The default is
		/// <c>false</c>.
		/// </summary>
		public static bool ForceStrictMode;
		
		/// <summary>
		/// A value that indicates if warnings about collapsed global variables should be triggered.
		/// A collapsing global has performance implications, so they're best avoided. The default is <c>true</c>.
		/// </summary>
		public static bool CollapseWarning=true;
		
		public PrototypeLookup Prototypes;
		
		/// <summary>
		/// The built-in global prototype.
		/// </summary>
		internal Prototype GlobalPrototype;
		
		/// <summary>
		/// The imported global prototype.
		/// </summary>
		internal Prototype ImportedGlobalPrototype;
		
		/// <summary>The global scope.</summary>
		internal Compiler.ObjectScope GlobalScope;
		
		/// <summary>The type of the global scope.
		/// Globals are typically created as a group of static fields.</summary>
		public Type GlobalScopeType{
			get{
				return GlobalPrototype.Type;
			}
		}
		
		/// <summary>The imported global object.</summary>
		internal object ImportedGlobals;
		
		/// <summary>
		/// Initializes a new scripting environment.
		/// </summary>
		public ScriptEngine(){
			Engine=this;
		}
		
		/// <summary>Imports the given object as a global scope.
		/// Note that you can only import one object and it must be the 
		/// same type if your code is cached.</summary>
		public void ImportGlobals(object scope){
			ImportedGlobals=scope;
		}
		
		/// <summary>
		/// Called when a global wasn't found.
		/// Depending on if the engine has full access
		/// it may search through all available C# methods for something it can use.
		/// </summary>
		public PropertyVariable GlobalNotFound(string name){
			
			if(FullAccess){
				
				// Search now:
				Prototype proto=Prototypes.Get(name,null);
				
				if(proto==null){
					return null;
				}
				
				// Set a null global with that type:
				PropertyVariable pv=SetGlobalType(name,proto.Type,PropertyAttributes.FullAccess);
				
				// It's a type reference:
				pv.TypeReference=true;
				
				return pv;
			}
			
			return null;
			
		}
		
		/// <summary>The engine name. Either 'Nitro' for JavaScript or 'WebAssembly'.</summary>
		public override string EngineName{
			get{
				return "Nitro";
			}
		}
		
		//	 GLOBAL HELPERS
		//_________________________________________________________________________________________

		/// <summary>
		/// Gets a value that indicates whether the global variable with the given name is defined.
		/// </summary>
		/// <param name="variableName"> The name of the variable to check. </param>
		/// <returns> <c>true</c> if the given variable has a value; <c>false</c> otherwise. </returns>
		/// <remarks> Note that a variable that has been set to <c>undefined</c> is still
		/// considered to have a value. </remarks>
		public bool HasGlobalValue(string variableName)
		{
			if (variableName == null)
				throw new ArgumentNullException("variableName");
			
			return (GlobalPrototype[variableName]!=null);
		}

		/// <summary>
		/// Gets the value of the global variable with the given name.
		/// </summary>
		/// <param name="variableName"> The name of the variable to retrieve the value for. </param>
		/// <returns> The value of the global variable, or <c>null</c> otherwise. </returns>
		public object GetGlobal(string variableName)
		{
			if (variableName == null)
				throw new ArgumentNullException("variableName");
			
			PropertyVariable prop=GlobalPrototype[variableName];
			
			if(prop==null){
				throw new ArgumentNullException("'"+variableName+"' is not a global.");
			}
			
			object value=prop.GetValue(null);
			
			return TypeUtilities.NormalizeValue(value);
		}

		/// <summary>
		/// Gets the value of the global variable with the given name and coerces it to the given
		/// type.
		/// </summary>
		/// <typeparam name="T"> The type to coerce the value to. </typeparam>
		/// <param name="variableName"> The name of the variable to retrieve the value for. </param>
		/// <returns> The value of the global variable, or <c>null</c> otherwise. </returns>
		/// <remarks> Note that <c>null</c> is coerced to the following values: <c>false</c> (if
		/// <typeparamref name="T"/> is <c>bool</c>), 0 (if <typeparamref name="T"/> is <c>int</c>
		/// or <c>double</c>), string.Empty (if <typeparamref name="T"/> is <c>string</c>). </remarks>
		public T GetGlobal<T>(string variableName)
		{
			if (variableName == null)
				throw new ArgumentNullException("variableName");
			return TypeConverter.ConvertTo<T>(this,
				TypeUtilities.NormalizeValue(
					GlobalPrototype.GetProperty(variableName).GetValue(null)
				)
			);
		}

		/// <summary>
		/// Sets the value of the global variable with the given name.  If the property does not
		/// exist, it will be created.
		/// </summary>
		/// <param name="variableName"> The name of the variable to set. </param>
		/// <param name="value"> The desired value of the variable.  This must be of a supported
		/// type (bool, int, double, string, Null, Undefined or a ObjectInstance-derived type). </param>
		/// <exception cref="JavaScriptException"> The property is read-only or the property does
		/// not exist and the object is not extensible. </exception>
		public PropertyVariable SetGlobal(string variableName, object value)
		{
			return SetGlobal(variableName, value,PropertyAttributes.FullAccess);
		}
		
		/// <summary>
		/// Sets the value of the global variable with the given name.  If the property does not
		/// exist, it will be created.
		/// </summary>
		/// <param name="variableName"> The name of the variable to set. </param>
		/// <param name="value"> The desired value of the variable. </param>
		/// <param name="attribs">The attributes of the value.</param>
		/// <exception cref="JavaScriptException"> The property is read-only or the property does
		/// not exist and the object is not extensible. </exception>
		public PropertyVariable SetGlobal(string variableName, object value,PropertyAttributes attribs)
		{
			// Try getting the property:
			PropertyVariable pv=GlobalPrototype[variableName];
			
			if(pv==null){
				// Create it:
				pv=GlobalPrototype.AddProperty(variableName,value,attribs);
			}
			
			// Await/ set now:
			AwaitStart(new AwaitingStart(pv,value));
			
			return pv;
			
		}
		
		internal void AwaitStart(AwaitingStart aws){
			
			if(HasStarted){
				
				// Global is currently settable.
				aws.OnStart(this);
				
			}else{
			
				if(AwaitingSet==null){
					AwaitingSet=new List<AwaitingStart>();
				}
				
				// Add to group of vars awaiting set
				// (this happens because we can't actually set the globals value yet - we must wait for it to be fully compiled):
				AwaitingSet.Add(aws);
				
			}
			
		}
		
		/// <summary>Creates a global of the given type.</summary>
		public PropertyVariable SetGlobalType(string variableName,Type type,PropertyAttributes attribs){
			
			// Create it:
			return GlobalPrototype.AddProperty(variableName,attribs,type);
			
		}
		
		/// <summary>True if the code has been compiled and the global scope is ready to go.</summary>
		public bool HasStarted;
		
		public void Started(){
			
			HasStarted=true;
			
			// Run everything in the waiting set:
			if(AwaitingSet==null){
				return;
			}
			
			for(int i=0;i<AwaitingSet.Count;i++){
				
				// Start it:
				AwaitingSet[i].OnStart(this);
				
			}
			
			// Clear set:
			AwaitingSet=null;
			
		}
		
		internal List<AwaitingStart> AwaitingSet;
		
		/// <summary>
		/// Calls a global function and returns the result.
		/// </summary>
		/// <param name="functionName"> The name of the function to call. </param>
		/// <param name="argumentValues"> The argument values to pass to the function.
		/// *must* include your 'this' override as the first one.
		/// Pass null if you don't want to override the 'this' value.</param>
		/// <returns> The return value from the function. </returns>
		public object CallGlobalFunction(string functionName, params object[] argumentValues)
		{
			if (functionName == null)
				throw new ArgumentNullException("functionName");
			if (argumentValues == null)
				throw new ArgumentNullException("argumentValues");
				
			return GlobalPrototype.CallMemberFunctionOn(null,functionName,argumentValues);
			
		}
		
		/// <summary>
		/// Calls a global function and returns the result.
		/// </summary>
		/// <typeparam name="T"> The type to coerce the value to. </typeparam>
		/// <param name="functionName"> The name of the function to call. </param>
		/// <param name="argumentValues"> The argument values to pass to the function.
		/// *must* include your 'this' override as the first one.
		/// Pass null if you don't want to override the 'this' value.</param>
		/// <returns> The return value from the function, coerced to the given type. </returns>
		public T CallGlobalFunction<T>(string functionName,params object[] argumentValues)
		{
			return TypeConverter.ConvertTo<T>(this, CallGlobalFunction(functionName, argumentValues));
		}
		
		/// <summary>
		/// Sets the global variable with the given name to a function implemented by the provided
		/// delegate.
		/// </summary>
		/// <param name="functionName"> The name of the global variable to set. </param>
		/// <param name="functionDelegate"> The delegate that will implement the function. </param>
		public void SetGlobalFunction(string functionName, Delegate functionDelegate)
		{
			if (functionName == null)
				throw new ArgumentNullException("functionName");
			if (functionDelegate == null)
				throw new ArgumentNullException("functionDelegate");
			SetGlobal(functionName, functionDelegate);
		}




		//	 STACK TRACE SUPPORT
		//_________________________________________________________________________________________
		
		/// <summary>
		/// Creates a stack trace.
		/// </summary>
		/// <param name="depth"> The number of stack frames to ignore. </param>
		internal string FormatStackTrace(int depth)
		{
			#if NETFX_CORE
			throw new NotSupportedException("Stack tracing is not available on .NET Core.");
			#else
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			System.Diagnostics.StackTrace trace=new System.Diagnostics.StackTrace(depth+1,true);
			
			for(int f=0; f<trace.FrameCount;f++){
				
				System.Diagnostics.StackFrame frame=trace.GetFrame(f);
				
				string methodName=frame.GetMethod().Name;
				
				AppendStackFrame(result, frame.GetFileName(), methodName, frame.GetFileLineNumber(), frame.GetFileColumnNumber());
				
				if(methodName=="__.main"){
					break;
				}
				
			}
			
			return result.ToString();
			#endif
		}

		/// <summary>
		/// Appends a stack frame to the end of the given StringBuilder instance.
		/// </summary>
		/// <param name="result"> The StringBuilder to append to. </param>
		/// <param name="path"> The path of the javascript source file. </param>
		/// <param name="function"> The name of the function. </param>
		/// <param name="line"> The line number of the statement. </param>
		private void AppendStackFrame(System.Text.StringBuilder result, string path, string function, int line, int column)
		{
			
			if(function=="__.main"){
				
			}else if (!string.IsNullOrEmpty(function))
			{
				result.Append(function);
			}else{
				result.Append("anonymous");
			}
			
			result.Append("@");
			
			result.Append(path ?? "unknown path");
			
			if (line > 0)
			{
				result.Append(":");
				result.Append(line);
				
				if(column > 0){
					result.Append(":");
					result.Append(column);
				}
			}
			
			result.AppendLine();
			
		}
		
		public static void Log(params object[] args){
			
			string msg="";
			
			for(int i=0;i<args.Length;i++){
				
				msg+=args[i]==null ? "[null]" : args[i].ToString();
				
			}
			
			UnityEngine.Debug.Log(msg);
			
		}
		
	}
}
