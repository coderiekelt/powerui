using System;
using System.Collections.Generic;
using JavaScript;

namespace JavaScript.Compiler
{
	/// <summary>
	/// Represents the unit of compilation.
	/// </summary>
	public abstract class MethodGenerator
	{
		/// <summary>
		/// Creates a new MethodGenerator instance.
		/// </summary>
		/// <param name="engine"> The script engine. </param>
		/// <param name="scope"> The initial scope. </param>
		/// <param name="source"> The source of javascript code. </param>
		protected MethodGenerator(ScriptEngine engine, Scope scope)
		{
			if (engine == null)
				throw new ArgumentNullException("engine");
			this.Engine = engine;
			this.InitialScope = scope;
			this.StrictMode = false;
			OptimizationInfo=new OptimizationInfo(engine);
		}
		
		/// <summary>
		/// This methods optimization info.
		/// </summary>
		public OptimizationInfo OptimizationInfo;
		
		/// <summary>
		/// True if it's currently compiling.
		/// </summary>
		protected bool Generating;
		
		/// <summary>
		/// The raw source.
		/// </summary>
		public string Source;
		
		/// <summary>
		/// Gets a reference to the script engine.
		/// </summary>
		public ScriptEngine Engine;
		
		/// <summary>
		/// Gets a value that indicates whether strict mode is enabled.
		/// </summary>
		public bool StrictMode;

		/// <summary>
		/// Gets the top-level scope associated with the context.
		/// </summary>
		public Scope InitialScope;
		
		/// <summary>
		/// Gets the root node of the abstract syntax tree.  This will be <c>null</c> until Parse()
		/// is called.
		/// </summary>
		internal BlockStatement AbstractSyntaxTree;

		/// <summary>
		/// Gets or sets optimization information.
		/// </summary>
		public MethodOptimizationHints MethodOptimizationHints;
		
		/// <summary>
		/// The emitted dynamic method(s), plus any dependencies.
		/// These are created when GenerateCode is called with a specific set of args.
		/// </summary>
		public List<UserDefinedFunction> GeneratedMethods=new List<UserDefinedFunction>();
		
		/// <summary>
		/// Gets a name for the generated method.
		/// </summary>
		/// <returns> A name for the generated method. </returns>
		protected abstract string GetMethodName();

		/// <summary>
		/// Gets a name for the function, as it appears in the stack trace.
		/// </summary>
		/// <returns> A name for the function, as it appears in the stack trace, or <c>null</c> if
		/// this generator is generating code in the global scope. </returns>
		protected virtual string GetStackName()
		{
			return null;
		}
		
		/// <summary>
		/// Parses the source text into an abstract syntax tree.
		/// </summary>
		public abstract void Parse();
		
		/// <summary>
		/// Generates IL for a specific variant of the method. Arguments must include the 'this' keyword.
		/// </summary>
		public UserDefinedFunction GenerateCode(ArgVariable[] arguments,Prototype onInstance)
		{	
			
			// Generate the abstract syntax tree if it hasn't already been generated.
			if (this.AbstractSyntaxTree == null)
			{
				Parse();
			}
			
			Type[] parameters;
			
			if(arguments==null){
				parameters=new Type[0];
			}else{
				// Create the param types, which is the args (as they already include 'this'):
				int argCount=arguments.Length;
				
				parameters=new Type[argCount-1];
				
				for(int i=1;i<argCount;i++){
					
					// Transfer the type over:
					parameters[i-1] = arguments[i].Type;
					
				}
			}
			
			string name=GetMethodName();
			
			// Initialize global code-gen information.
			OptimizationInfo.AbstractSyntaxTree = this.AbstractSyntaxTree;
			OptimizationInfo.StrictMode = this.StrictMode;
			OptimizationInfo.MethodOptimizationHints = this.MethodOptimizationHints;
			OptimizationInfo.FunctionName = this.GetStackName();
			
			// Create the UDF now (must be before we generate the code as it would go recursive for recursive functions otherwise):
			UserDefinedFunction mtd=new UserDefinedFunction(name, arguments, this as FunctionMethodGenerator,StrictMode);
			
			// Add to created set:
			GeneratedMethods.Add(mtd);
			
			if(Generating){
				// Queue a generation request.
				throw new Exception("Enqueue request");
			}
			
			Generating=true;
			
			// Is this an instance method?
			bool instanceMethod=(onInstance!=null);
			
			// Get the type to use.
			ILTypeBuilder typeBuilder;
			
			// Attributes:
			System.Reflection.MethodAttributes attribs=System.Reflection.MethodAttributes.HideBySig | System.Reflection.MethodAttributes.Public;
			
			if(instanceMethod){
				
				// Use the instance builder:
				typeBuilder=onInstance.Builder;
				
				// Store the instance in the UDF:
				mtd.OnInstance=onInstance;
				
			}else{
				
				// Use the global type builder
				typeBuilder=Engine.ModuleInfo.MainBuilder;
				
				attribs |= System.Reflection.MethodAttributes.Static;
				
			}
			
			// Reset locals (Resets their IL store/ type etc):
			if(InitialScope!=null){
				InitialScope.Reset();
			}
			
			// Resolve variables now:
			AbstractSyntaxTree.ResolveVariables(OptimizationInfo);
			
			Type returnType = OptimizationInfo.ReturnType;
			
			// Create the method builder now:
			ILGenerator generator = typeBuilder.DefineMethod(name,
				attribs,
				returnType, parameters);
			
			// If we've got any 'converted' arg variables, emit a local var set:
			
			if(arguments!=null){
				
				for(int i=0;i<arguments.Length;i++){
					
					// Converted to a local?
					if(arguments[i].Converted!=null){
						// Yep - emit a local var set:
						
						// Convert:
						arguments[i].Converted.Set(generator,OptimizationInfo,false,arguments[i].Type,delegate(bool rIU){
							
							// Load the args value onto the stack:
							generator.LoadArgument(i);
							
						});
						
					}
					
				}
				
			}
			
			// If we're an instance method, pull the __this__ value:
			if(instanceMethod){
				
				PropertyVariable pv=onInstance["__this__"];
				
				if(pv!=null){
					generator.InstanceThis = pv.GetField();
				}
				
			}
			
			// Apply the body:
			mtd.body=generator.Method;
			
			// OptimizationInfo.MarkSequencePoint(generator, new SourceCodeSpan(1, 1, 1, 1));
			GenerateCode(arguments, generator);
			generator.Complete(name,arguments, returnType);
			
			Generating=false;
			return mtd;
			
		}
		
		public virtual bool HasArgument(string name){
			return false;
		}
		
		/// <summary>
		/// Generates IL for the script.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		/// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
		protected abstract void GenerateCode(ArgVariable[] arguments,ILGenerator generator);
		
	}

}