using System;
using System.Reflection;
using System.Collections.Generic;


namespace JavaScript.Compiler
{
	
	/// <summary>
    /// Represents how the function was defined.
    /// </summary>
    internal enum FunctionDeclarationType
    {
        /// <summary>
        /// The function was declared as a statement.
        /// </summary>
        Declaration,

        /// <summary>
        /// The function was declared as an expression.
        /// </summary>
        Expression,

        /// <summary>
        /// The function is a getter in an object literal.
        /// </summary>
        Getter,

        /// <summary>
        /// The function is a setter in an object literal.
        /// </summary>
        Setter,
	}
	
	/// <summary>
	/// Represents the information needed to compile a function.
	/// </summary>
	public class FunctionMethodGenerator : MethodGenerator
	{	
		
		/// <summary>
		/// Creates a new FunctionMethodGenerator instance.
		/// </summary>
		/// <param name="engine"> The script engine. </param>
		/// <param name="scope"> The function scope. </param>
		/// <param name="functionName"> The name of the function. </param>
		/// <param name="includeNameInScope"> Indicates whether the name should be included in the function scope. </param>
		/// <param name="args"> The names of the arguments. </param>
		/// <param name="bodyText"> The source code of the function. </param>
		/// <param name="body"> The root of the abstract syntax tree for the body of the function. </param>
		/// <param name="scriptPath"> The URL or file system path that the script was sourced from. </param>
		internal FunctionMethodGenerator(ScriptEngine engine, DeclarativeScope scope, string functionName, List<ArgVariable> args, BlockStatement body, string scriptPath)
			: base(engine, scope)
		{
			this.Name = functionName;
			this.Arguments = args;
			this.BodyRoot = body;
		}
		
		/*
		/// <summary>
		/// Creates a new FunctionContext instance.
		/// </summary>
		/// <param name="engine"> The script engine. </param>
		/// <param name="scope"> The function scope. </param>
		/// <param name="functionName"> The name of the function. </param>
		/// <param name="args"> The names of the arguments. </param>
		/// <param name="body"> The source code for the body of the function. </param>
		public FunctionMethodGenerator(ScriptEngine engine, DeclarativeScope scope, string functionName, List<ArgVariable> args, string body)
			: base(engine, scope)
		{
			this.Name = functionName;
			this.Arguments = args;
		}
		*/
		
		/// <summary>
		/// Gets the name of the function.
		/// </summary>
		public string Name;
		
		/// <summary>
		/// Gets a list of arguments and their optional type.
		/// </summary>
		public List<ArgVariable> Arguments;
		
		/// <summary>
		/// Gets the root of the abstract syntax tree for the body of the function.
		/// </summary>
		internal BlockStatement BodyRoot;
		
		/// <summary>
		/// Gets a name for the generated method.
		/// </summary>
		/// <returns> A name for the generated method. </returns>
		protected override string GetMethodName()
		{
			if (string.IsNullOrEmpty(this.Name))
				return "anonymous";
			else
				return this.Name;
		}
		
		/// <summary>True if the given method info is from this generator.</summary>
		internal UserDefinedFunction Find(System.Reflection.MethodInfo mtd){
			
			for(int i=0;i<GeneratedMethods.Count;i++){
				
				if(GeneratedMethods[i].body==mtd){
					return GeneratedMethods[i];
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>
		/// Gets a name for the function, as it appears in the stack trace.
		/// </summary>
		/// <returns> A name for the function, as it appears in the stack trace, or <c>null</c> if
		/// this generator is generating code in the global scope. </returns>
		protected override string GetStackName()
		{
			return GetMethodName();
		}
		
		public override bool HasArgument(string name)
		{
			
			foreach(ArgVariable arg in Arguments)
			{
				if(arg.Name==name)
				{
					return true;
				}
			}
			
			return false;
		}
		
		private Prototype _RawInstancePrototype;
		
		/// <summary>
		/// Gets the prototype of objects constructed using this function.  Equivalent to
		/// the Function.prototype property.
		/// </summary>
		public Prototype GetInstancePrototype(ScriptEngine engine){
			
			if(_RawInstancePrototype==null){
				
				// Create one now!
				_RawInstancePrototype=engine.Prototypes.Create();
				
			}
			
			return _RawInstancePrototype;
			
		}
		
		/// <summary>Gets the compiled function for the given args set. Note that this args set
		/// is the actual types and always includes the 'this' keywords type.</summary>
		public UserDefinedFunction GetCompiled(Type[] args,ScriptEngine engine,bool isConstructor){
			
			int genCount=GeneratedMethods.Count;
			
			for(int i=0;i<genCount;i++){
				
				if(GeneratedMethods[i].ArgsEqual(args)){
					
					// Got a match! Already compiled it.
					return GeneratedMethods[i];
					
				}
				
			}
			
			// Need to compile it now with our given arg types.
			int argCount=args==null?0:args.Length;
			
			// The source contains a fixed number of args; it goes up to this:
			int maxArg=Arguments.Count;
			
			// First, map args to a block of ArgVariable objects:
			ArgVariable[] argsSet=new ArgVariable[argCount];
			
			for(int i=0;i<argCount;i++){
				
				// Setup the arg variable:
				ArgVariable curArg;
				
				if(i<maxArg){
					
					// Use the existing args object so it correctly updates in the scope:
					curArg=Arguments[i];
					curArg.Generic=false;
					
				}else{
					
					// Need to create a fake one:
					curArg=new ArgVariable("@unused-arg-"+i);
					
				}
				
				// Apply type (a little different here as we have to account for null too):
				curArg.RawType=args[i];
				
				// Apply to set:
				argsSet[i]=curArg;
				
			}
			
			// If we're passing less args than the source supports, update the types of those unused args to being 'Undefined':
			for(int i=argCount;i<maxArg;i++){
				
				ArgVariable arg = Arguments[i];
				arg.Type=typeof(JavaScript.Undefined);
				arg.Generic=false;
				
			}
			
			// Update info:
			OptimizationInfo.IsConstructor=isConstructor;
			
			// Generate it!
			return GenerateCode(argsSet,(Prototype)null);
			
		}
		
		/// <summary>Gets the compiled function using the given delegate type.
		/// These always compile as instance methods.</summary>
		public UserDefinedFunction GetCompiled(Type delegateType,Type thisType,OptimizationInfo info,Prototype instanceOf){
			
			// Get the invoke method:
			#if NETFX_CORE
			System.Reflection.MethodInfo invoke=delegateType.GetMethod("Invoke");
			#else
			System.Reflection.MethodInfo invoke=delegateType.GetMethod("Invoke");
			#endif
			
			// Get the type args:
			System.Reflection.ParameterInfo[] parameters=invoke.GetParameters();
			
			// Set the initial return type too:
			OptimizationInfo.SingleReturnType = invoke.ReturnType;
			
			int argCount=parameters.Length;
			
			// The source contains a fixed number of args, including the 'this' arg:
			int maxArg=Arguments.Count;
			
			// First, map args to a block of ArgVariable objects:
			ArgVariable[] argsSet=new ArgVariable[argCount+1];
			
			// Apply 'this':
			argsSet[0] = Arguments[0];
			
			if(delegateType == typeof(JSFunction)){
				
				// This type is always just 'object':
				argsSet[0].RawType = typeof(object);
				
				// Generic form of the function
				// 2 args and the 2nd is always object[].
				argsSet[1]=new ArgVariable("@special-args");
				argsSet[1].RawType = typeof(object[]);
				
				// Set the rest of the real args:
				for(int i=1;i<maxArg;i++){
					
					// Get the arg:
					ArgVariable arg = Arguments[i];
					
					// Mark as generic form/ go via the array:
					arg.Generic = true;
					
					// Unknown type:
					arg.RawType = typeof(object);
					
				}
				
			}else{
				// (Note: The actual args type is instanceOf.Type; 
				// it must be thisType for the var resolve phase)
				argsSet[0].RawType = thisType;
				
				// Starting index (skip 'this' arg):
				int index=1;
				
				for(int i=0;i<argCount;i++){
					
					// Setup the arg variable:
					ArgVariable curArg;
					
					if(index<maxArg){
						
						// Use the existing args object so it correctly updates in the scope:
						curArg=Arguments[index];
						curArg.Generic = false;
						index++;
						
					}else{
						
						// Need to create a fake one:
						curArg=new ArgVariable("@unused-arg-"+i);
						
					}
					
					// Apply type (a little different here as we have to account for null too):
					curArg.RawType=parameters[i].ParameterType;
					
					// Apply to set:
					argsSet[i+1]=curArg;
					
				}
				
				// If we're passing less args than the source supports (+1 to avoid 'this'), 
				// update the types of those unused args to being 'Undefined':
				for(int i=1+argCount;i<maxArg;i++){
					ArgVariable arg=Arguments[i];
					arg.Type=typeof(JavaScript.Undefined);
					arg.Generic=false;
				}
				
			}
			
			// Generate it!
			return GenerateCode(argsSet,instanceOf);
			
		}
		
		/// <summary>Gets the compiled function for the given args set. Note that this args set
		/// is the actual values and always includes the 'this' keywords value.</summary>
		public UserDefinedFunction GetCompiled(ScriptEngine engine,object[] args){
			
			int genCount=GeneratedMethods.Count;
			
			for(int i=0;i<genCount;i++){
				
				if(GeneratedMethods[i].ArgsEqual(args)){
					
					// Got a match! Already compiled it.
					return GeneratedMethods[i];
					
				}
				
			}
			
			// Need to compile it now with our given arg types.
			int argCount=args.Length;
			
			if(argCount>Arguments.Count){
				// The actual source can only handle so many args.
				argCount=Arguments.Count;
			}
			
			// First, map args to Arguments:
			ArgVariable[] argsSet=new ArgVariable[argCount];
			
			for(int i=0;i<argCount;i++){
				
				// Setup the arg variable:
				ArgVariable curArg=new ArgVariable(Arguments[i].Name);
				
				// Apply type:
				curArg.Type=args[i].GetType();
				
				// Apply to set:
				argsSet[i]=curArg;
				
			}
			
			// Generate it!
			return GenerateCode(argsSet,(Prototype)null);
		}
		
		/// <summary>
		/// Parses the source text into an abstract syntax tree.
		/// </summary>
		/// <returns> The root node of the abstract syntax tree. </returns>
		public override void Parse()
		{
			if (this.BodyRoot != null)
			{
				this.AbstractSyntaxTree = this.BodyRoot;
			}
			else
			{
				using (var lexer = new Lexer(this.Engine, this.Source))
				{
					var parser = new Parser(this.Engine, lexer, this.InitialScope, OptimizationInfo, CodeContext.Function);
					this.AbstractSyntaxTree = parser.Parse();
					this.StrictMode = parser.StrictMode;
					this.MethodOptimizationHints = parser.MethodOptimizationHints;
				}
			}
		}
		
		/// <summary>
		/// Generates IL for the script.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		protected override void GenerateCode(ArgVariable[] arguments,ILGenerator generator)
		{
			
			// Transfer the arguments object into the scope.
			if (MethodOptimizationHints.HasArguments && !HasArgument("arguments"))
			{
				
				var argsSet = new NameExpression(this.InitialScope, "arguments");
				
				argsSet.ApplyType(OptimizationInfo,typeof(object));
				
				argsSet.GenerateSet(generator, OptimizationInfo,false, typeof(object), delegate(bool two)
				{
					
					// argumentValues
					
					// Build an object[] from the arg values.
					
					// Define an array:
					int argCount=(arguments==null)?0 : arguments.Length;
					
					// Got a 'this' arg?
					bool hasThis=argCount>0 && arguments[0].Name=="this";
					
					generator.LoadInt32(hasThis ? argCount-1 : argCount);
					generator.NewArray(typeof(object));
					
					for(int a=0;a<argCount;a++)
					{
						
						if(a==0 && hasThis){
							continue;
						}
						
						// One of many args:
						ArgVariable currentArg=arguments[a];
						
						generator.Duplicate();
						generator.LoadInt32(a);
						currentArg.Get(generator);
						EmitConversion.ToAny(generator,currentArg.Type);
						generator.StoreArrayElement(typeof(object));
					}
					
					
					generator.NewObject(ReflectionHelpers.Arguments_Constructor);
					
					if(two){
						generator.Duplicate();
					}
					
				}, false);
			}
			
			// Temp cache return target:
			var retTarg=OptimizationInfo.ReturnTarget;
			OptimizationInfo.ReturnTarget=null;
			
			// Initialize any declarations.
			(this.InitialScope as DeclarativeScope).GenerateDeclarations(generator, OptimizationInfo);
			
			// Generate code for the body of the function.
			this.AbstractSyntaxTree.GenerateCode(generator, OptimizationInfo);
			
			// Define the return target - this is where the return statement jumps to.
			// ReturnTarget can be null if there were no return statements.
			if (OptimizationInfo.ReturnTarget != null)
				generator.DefineLabelPosition(OptimizationInfo.ReturnTarget);
			
			if(OptimizationInfo.ReturnType!=typeof(void)){
			
				// Load the return value.  If the variable is null, there were no return statements.
				if (OptimizationInfo.ReturnVariable != null){
					// Return the value stored in the variable.  Will be null if execution hits the end
					// of the function without encountering any return statements.
					OptimizationInfo.ReturnVariable.Get(generator);
				}else if(OptimizationInfo.ReturnType != null){
					// Check if the last statement is a return:
					if(!(AbstractSyntaxTree.LastStatement is ReturnStatement)){
						// Nope! Emit undefined:
						EmitHelpers.EmitUndefined(generator);
					}
				}
				
			}
			
			// Restore target:
			OptimizationInfo.ReturnTarget=retTarg;
			
		}

		/// <summary>
		/// Converts this object to a string.
		/// </summary>
		/// <returns> A string representing this object. </returns>
		public override string ToString()
		{
			
			string str="function "+Name+"("+StringHelpers.Join(", ", Arguments)+") {\n";
			
			if(BodyRoot==null){
				str+="[Native code]";
			}else{
				str+=this.BodyRoot.ToString();
			}
			
			return str+"\n}";
			
		}
	}

}