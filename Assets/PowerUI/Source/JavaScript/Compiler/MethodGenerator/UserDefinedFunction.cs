﻿using System;
using System.Collections.Generic;
using System.Reflection;
using JavaScript;


namespace JavaScript.Compiler
{
	/// <summary>
	/// Represents a JavaScript function implemented in javascript.
	/// </summary>
	public partial class UserDefinedFunction : FunctionInstance
	{
		
		/// <summary>The underlying methodInfo.</summary>
		internal MethodInfo body;
		
		/*
		/// <summary>
		/// Creates a new instance of a user-defined function.
		/// </summary>
		/// <param name="prototype"> The next object in the prototype chain. </param>
		/// <param name="name"> The name of the function. </param>
		/// <param name="args"> The names of the arguments. MUST include 'this'.</param>
		/// <param name="bodyText"> The source code for the body of the function. </param>
		internal static UserDefinedFunction Create(ScriptEngine engine, string name, List<ArgVariable> args, string bodyText){
			
			if(args==null){
				args=new List<ArgVariable>();
				args.Add(new ArgVariable("this"));
			}
			
			if (name == null)
				throw new ArgumentNullException("name");
			if (args == null)
				throw new ArgumentNullException("args");
			if (bodyText == null)
				throw new ArgumentNullException("bodyText");

			// Set up a new function scope.
			var scope = DeclarativeScope.CreateFunctionScope(engine.GlobalScope, args);
			
			// Compile the code.
			var context = new FunctionMethodGenerator(engine, scope, name, args, bodyText);
			context.GenerateCode();
			
			return context.GeneratedMethods[0];
		}
		*/
		
		/// <summary>
		/// Creates a new instance of a user-defined function.
		/// </summary>
		/// <param name="prototype"> The next object in the prototype chain. </param>
		/// <param name="name"> The name of the function. </param>
		/// <param name="args"> The names of the arguments. </param>
		/// <param name="bodyText"> The source code for the function body. </param>
		/// <param name="generatedMethod"> A delegate which represents the body of the function. </param>
		/// <param name="strictMode"> <c>true</c> if the function body is strict mode; <c>false</c> otherwise. </param>
		internal UserDefinedFunction(string name, IList<ArgVariable> args, FunctionMethodGenerator gen, bool strictMode)
		{
			Init(name, args, gen, strictMode, true);
		}
		
		/// <summary>
		/// Initializes a user-defined function.
		/// </summary>
		/// <param name="name"> The name of the function. </param>
		/// <param name="args"> The names of the arguments. </param>
		/// <param name="bodyText"> The source code for the function body. </param>
		/// <param name="generatedMethod"> A delegate which represents the body of the function. </param>
		/// <param name="strictMode"> <c>true</c> if the function body is strict mode; <c>false</c> otherwise. </param>
		/// <param name="hasInstancePrototype"> <c>true</c> if the function should have a valid
		/// "prototype" property; <c>false</c> if the "prototype" property should be <c>null</c>. </param>
		private void Init(string name, IList<ArgVariable> args, FunctionMethodGenerator gen, bool strictMode, bool hasInstancePrototype)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (args == null){
				args=new ArgVariable[0];
			}
			this.Arguments = args;
			this.Generator = gen;
			this.StrictMode = strictMode;
			
			Name=name;
			Length=args.Count;
			
		}
		
		//	 PROPERTIES
		//_________________________________________________________________________________________

		/// <summary>
		/// Gets a list containing the names of the function arguments, in order of definition.
		/// This list can contain duplicate names.
		/// </summary>
		internal IList<ArgVariable> Arguments;
		
		/// <summary>
		/// If this is an anonymous function, the instance here is essentially the scope.
		/// It entirely changes the meaning of the 'this' keyword (to a property called __this__).
		/// </summary>
		public Prototype OnInstance;
		
		/// <summary>
		/// Gets a value that indicates whether the function was declared within strict mode code
		/// -or- the function contains a strict mode directive within the function body.
		/// </summary>
		public bool StrictMode;
		
		public bool ArgsEqual(object[] actualValues){
			
			if(actualValues==null){
				return (Arguments==null || Arguments.Count==0);
			}
			
			if(actualValues.Length>Arguments.Count){
				// Giving more args than this compiled version handles.
				return false;
			}
			
			for(int i=0;i<actualValues.Length;i++){
				
				Type type=actualValues[i].GetType();
				
				// Match?
				if(type!=Arguments[i].Type){
					return false;
				}
				
			}
			
			return true;
			
		}

		public bool ArgsEqual(Type[] types){
			
			if(types==null){
				return (Arguments==null || Arguments.Count==0);
			}
			
			if(types.Length>Arguments.Count){
				// Giving more args than this compiled version handles.
				return false;
			}
			
			for(int i=0;i<types.Length;i++){
				
				Type type=types[i];
				
				// Match?
				if(type!=Arguments[i].Type){
					return false;
				}
				
			}
			
			return true;
			
		}

		//	 OVERRIDES
		//_________________________________________________________________________________________

		/// <summary>
		/// Calls this function, passing in the given "this" value and zero or more arguments.
		/// </summary>
		/// <param name="thisAndArgumentValues"> The value of the "this" keyword within the function plus
		/// an array of argument values to pass to the function. </param>
		/// <returns> The value that was returned from the function. </returns>
		public override object CallLateBound(ScriptEngine engine,params object[] thisAndArgumentValues)
		{
			
			if(body.GetType()!=typeof(System.Reflection.MethodInfo)){
				// MethodBuilder - Close the type.
				Close(engine);
			}
			
			// Call the function.
			return body.Invoke(null,thisAndArgumentValues);
		}
		
		/// <summary>Closes this UDF if necessary.</summary>
		public void Done(ScriptEngine engine){
			
			if(body.GetType()!=typeof(System.Reflection.MethodInfo)){
				// Close the type.
				Close(engine);
			}
			
		}
		
		/// <summary>
		/// Returns a string representing this object.
		/// </summary>
		/// <returns> A string representing this object. </returns>
		public override string ToString()
		{
			return Generator.ToString();
		}
	}
}
