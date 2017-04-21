using System;
using System.Reflection;
using System.Collections.Generic;
using JavaScript;


namespace JavaScript.Compiler
{
	/// <summary>
	/// Represents a function call expression.
	/// </summary>
	internal class FunctionCallExpression : OperatorExpression
	{
		
		/// <summary>
		/// The UDF, if it is one.
		/// </summary>
		private UserDefinedFunction UserDefined;
		
		/// <summary>
		/// The field to load a delegate from.
		/// </summary>
		private PropertyVariable DelegateFrom;
		/// <summary>
		/// Used when a resolved method is imported from some other scope.
		/// Occurs on non-static global methods.
		/// </summary>
		private FieldInfo Imported;
		/// <summary>
		/// The resolved method being called, if possible to directly resolve it.
		/// This is either a MethodBase or MethodGroup.
		/// </summary>
		public System.Reflection.MethodBase ResolvedMethod;
		
		/// <summary>True if this call uses the 'new' keyword.</summary>
		private bool IsConstructor;
		
		/// <summary>
		/// Creates a new instance of FunctionCallJSExpression.
		/// </summary>
		/// <param name="operator"> The binary operator to base this expression on. </param>
		public FunctionCallExpression(Operator @operator)
			: base(@operator)
		{
		}
		
		/// <summary>If this is a user defined method, the instance type. It's the type used when 
		/// doing e.g. new funcName();</summary>
		public Type InstanceType(ScriptEngine engine){
			return UserDefined.GetInstanceType(engine);
		}
		
		/// <summary>True if this is known to be calling a user defined function.</summary>
		public bool IsUserDefined{
			get{
				return (UserDefined!=null);
			}
		}
		
		/// <summary>
		/// Gets an expression that evaluates to the function instance.
		/// </summary>
		public Expression Target
		{
			get { return this.GetOperand(0); }
		}
		
		/// <summary>Evaluates the argument at the given index.</summary>
		public object Arg(int index){	
			
			if(OperandCount>1)
			{
				var argumentsOperand = this.GetRawOperand(1);
				ListExpression argList = argumentsOperand as ListExpression;
				
				if (argList==null){
					
					// 1 only.
					if(index!=0){
						throw new Exception("Arg "+index+" was not provided.");
					}
					
					return argumentsOperand.Evaluate();
					
				}else{
					// Multiple parameters were recieved.
					return argList.Items[index].Evaluate();
				}
			}
			
			throw new Exception("Arg "+index+" was not provided.");
			
		}
		
		/// <summary>
		/// Gets the type that results from evaluating this expression.
		/// </summary>
		public override Type GetResultType(OptimizationInfo optimizationInfo)
		{
			// Not a new call anymore:
			optimizationInfo.IsConstructCall=false;
			
			if(ResolvedMethod==null)
			{
				// Totally unknown as it's resolved at runtime only.
				return typeof(object);
			}
			
			// Check if it's a constructor:
			System.Reflection.MethodInfo methodInfo=(ResolvedMethod as System.Reflection.MethodInfo);
			
			Type result=null;
			
			if(methodInfo==null)
			{
				// Constructor.
				result=(ResolvedMethod as System.Reflection.ConstructorInfo).DeclaringType;
			}
			else
			{
				// Normal method.
				result=methodInfo.ReturnType;
			}
			
			if(result==null || result==typeof(void))
			{
				return typeof(JavaScript.Undefined);
			}
			
			return result;
		}
		
		/// <summary>
		/// Used to implement function calls without evaluating the left operand twice.
		/// </summary>
		private class TemporaryVariableExpression : Expression
		{
			private ILLocalVariable variable;

			public TemporaryVariableExpression(ILLocalVariable variable)
			{
				this.variable = variable;
			}

			public override Type GetResultType(OptimizationInfo optimizationInfo)
			{
				return typeof(object);
			}

			public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
			{
				generator.LoadVariable(this.variable);
			}
		}
		
		/// <summary>
		/// Resolves which function is being called where possible.
		/// </summary>
		internal override void ResolveVariables(OptimizationInfo optimizationInfo)
		{
			
			// Clear:
			ResolvedMethod=null;
			UserDefined=null;
			
			// Grab if this is a 'new' call:
			IsConstructor=optimizationInfo.IsConstructCall;
			optimizationInfo.IsConstructCall=false;
			
			// Resolve kids:
			base.ResolveVariables(optimizationInfo);
			
			TryResolveMethod(optimizationInfo);
			
		}
		
		private void TryResolveMethod(OptimizationInfo optimizationInfo){
			
			object resolvedMethod=null;
			Imported=null;
			
			if (this.Target is MemberAccessExpression)
			{
				// The function is a member access expression (e.g. "Math.cos()").
				
				// Get the parent of the member (e.g. Math):
				MemberAccessExpression baseExpression = ((MemberAccessExpression)this.Target);
				
				// Get the property:
				JavaScript.PropertyVariable property=baseExpression.GetProperty(optimizationInfo);
				Imported = property.ImportedFrom;
				
				// It should be a callable method:
				#if NETFX_CORE
				if(property.Type==typeof(MethodGroup) || typeof(System.Reflection.MethodBase).GetTypeInfo().IsAssignableFrom(property.Type.GetTypeInfo()))
				#else
				if(property.Type==typeof(MethodGroup) || typeof(System.Reflection.MethodBase).IsAssignableFrom(property.Type))
				#endif
				{
					
					if(property.IsConstant){
						
						// Great, grab the value:
						resolvedMethod=property.ConstantValue;
						
					}else{
						
						// This occurs when the method has collapsed.
						// The property is still a method though, so we know for sure it can be invoked.
						// It's now an instance property on the object.
						optimizationInfo.TypeError("Runtime method in use (not supported at the moment). Internal: #T1");
						
					}
					
				}else if(property.Type==typeof(FunctionMethodGenerator)){
					
					FunctionMethodGenerator fm=property.ConstantValue as FunctionMethodGenerator;
					
					if(fm!=null){
						
						// Get the arg types being passed into the method, including 'this':
						Type[] argTypes=GetArgumentTypes(optimizationInfo,true,IsConstructor?fm:null);
						
						// Get a specific overload:
						UserDefinedFunction udm=fm.GetCompiled(argTypes,optimizationInfo.Engine,IsConstructor);
						
						resolvedMethod=udm.body;
						UserDefined=udm;
						
					}else{
						
						// Runtime resolve (property)
						optimizationInfo.TypeError("Runtime property in use (not supported at the moment). Internal: #T4");
					
					}
				
				#if NETFX_CORE
				}else if(typeof(Delegate).GetTypeInfo().IsAssignableFrom(property.Type.GetTypeInfo())){
				#else
				}else if(typeof(Delegate).IsAssignableFrom(property.Type)){
				#endif
					// Invoking a delegate:
					resolvedMethod = property.Type.GetMethod("Invoke");
					DelegateFrom = property;
					// The property itself deals with the imported scope:
					Imported = null;
					
				}else if(property.Type!=typeof(object))
				{
					throw new JavaScriptException(
						optimizationInfo.Engine,
						"TypeError",
						"Cannot run '"+property.Name+"' as a method because it's known to be a "+property.Type+". Keep in mind that JS is case sensitive and by default it will lowercase the first letter of your methods."
					);
				}
				else
				{
					
					// Similar to above, but this time its something that might not even be a method
					optimizationInfo.TypeError("Runtime method in use (not supported at the moment). Internal: #T2");
					
				}
			}
			
			if(resolvedMethod==null)
			{
				// Get target as a name expression:
				NameExpression nameExpr=Target as NameExpression;
				
				if(nameExpr!=null && nameExpr.Variable!=null){
					
					PropertyVariable prop=(nameExpr.Variable as PropertyVariable);
					
					if(prop!=null){
						Imported = prop.ImportedFrom;
					}
					
					if(nameExpr.Variable.IsConstant){
						
						// Get the constant:
						object constant=nameExpr.Variable.ConstantValue;
						
						// FMG?
						FunctionMethodGenerator fm=constant as FunctionMethodGenerator;
						
						if(fm!=null){
							
							// Get the arg types being passed into the method, including 'this':
							Type[] argTypes=GetArgumentTypes(optimizationInfo,true,IsConstructor?fm:null);
							
							// Get a specific overload:
							UserDefinedFunction udm=fm.GetCompiled(argTypes,optimizationInfo.Engine,IsConstructor);
							
							resolvedMethod=udm.body;
							UserDefined=udm;
							
						}else if(constant is System.Reflection.MethodInfo){
							
							// We've got a built-in method call (e.g. eval, parseInt etc fall through here).
							resolvedMethod=(constant as System.Reflection.MethodInfo);
							UserDefined=null;
							
						}else if(constant is MethodGroup){
							resolvedMethod=(constant as MethodGroup);
							UserDefined=null;
						}else{
							
							// Likely a built in constructor.
							
							// Get the return type:
							Type returnType=Target.GetResultType(optimizationInfo);
							
							// Get the proto for it:
							JavaScript.Prototype proto=optimizationInfo.Engine.Prototypes.Get(returnType);
							
							// If we've got a constructor proto, use that:
							if(proto.ConstructorPrototype!=null){
								proto=proto.ConstructorPrototype;
							}
							
							// Note that these two special methods are always methods
							// or method groups so no checking is necessary.
							if(IsConstructor){
								resolvedMethod=proto.OnConstruct;
							}else{
								resolvedMethod=proto.OnCall;
							}
							
						}
						
					}else{
						
						// Typically invoking a delegate here.
						#if NETFX_CORE
						if(typeof(Delegate).GetTypeInfo().IsAssignableFrom(prop.Type.GetTypeInfo())){
						#else
						if(typeof(Delegate).IsAssignableFrom(prop.Type)){
						#endif
							// Invoking a delegate:
							resolvedMethod = prop.Type.GetMethod("Invoke");
							DelegateFrom = prop;
							// The property itself deals with the imported scope:
							Imported = null;
							
						}else{
							// -> E.g. new varName() or varName()
							
							// Get the return type:
							Type returnType=Target.GetResultType(optimizationInfo);
							
							// Get the proto for it:
							JavaScript.Prototype proto=optimizationInfo.Engine.Prototypes.Get(returnType);
							
							if(proto!=null){
								
								// If we've got a constructor proto, use that:
								if(proto.ConstructorPrototype!=null){
									proto=proto.ConstructorPrototype;
								}
								
								// Note that these two special methods are always methods
								// or method groups so no checking is necessary.
								if(IsConstructor)
								{
									resolvedMethod=proto.OnConstruct;
								}
								else
								{
									resolvedMethod=proto.OnCall;
								}
								
							}
							
						}
						
					}
					
				}else{
					
					// Something else (e.g. "eval()")
					
					// Get the return type:
					Type returnType=Target.GetResultType(optimizationInfo);
					
					// Get the proto for it:
					JavaScript.Prototype proto=optimizationInfo.Engine.Prototypes.Get(returnType);
					
					// If we've got a constructor proto, use that:
					if(proto.ConstructorPrototype!=null){
						proto=proto.ConstructorPrototype;
					}
					
					// Note that these two special methods are always methods
					// or method groups so no checking is necessary.
					if(IsConstructor)
					{
						resolvedMethod=proto.OnConstruct;
					}
					else
					{
						resolvedMethod=proto.OnCall;
					}
					
				}
				
			}
			
			if(resolvedMethod==null)
			{
				// Runtime resolve only.
				ResolvedMethod=null;
				return;
			}
			
			// Note that it may be a MethodGroup, so let's resolve it further if needed.
			MethodGroup group=resolvedMethod as MethodGroup;
			
			if(group==null)
			{
				// It must be MethodBase - it can't be anything else:
				ResolvedMethod=resolvedMethod as System.Reflection.MethodBase;
			}
			else
			{
				
				// We have a group! Find the overload that we're after. It may be ambiguous:
				
				if(group.Disambiguation!=null){
					
					// It's ambiguous. Invoke the disambiguation now:
					ResolvedMethod = (MethodBase) group.Disambiguation.Invoke(null,new object[]{group,this});
					
				}else{
					// (excluding 'this' and it's never a constructor either):
					ResolvedMethod=group.Match( GetArgumentTypes(optimizationInfo,false,null) );
				}
				
			}
			
		}
		
		/// <summary>
		/// Generates CIL for the expression.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		/// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
		public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
		{
			// Clear CC:
			optimizationInfo.IsConstructCall=false;
			
			if(ResolvedMethod==null)
			{
				// Last chance! Try again
				// (this happens in recursive functions 
				// where we knew it was a function but not the actual method info):
				TryResolveMethod(optimizationInfo);
			}
			
			if(ResolvedMethod==null)
			{
				// Either runtime resolve it or it's not actually a callable function
				throw new NotImplementedException("A function was called which was not supported ("+ToString()+")");
			}
			
			// We have a known method!
			Type thisType=null;
			
			if(UserDefined!=null){
				
				// Emit 'this' now:
				EmitThis(generator,optimizationInfo,out thisType);
				
			}
			
			// Load the field if it's a delegate:
			if(DelegateFrom!=null){
				DelegateFrom.Get(generator);
			}
			
			// Emit the rest of the args:
			EmitArguments(generator,optimizationInfo,thisType);
			
			// Got a return type?
			Type returnType=GetResultType(optimizationInfo);
			
			// Then the call!
			if(typeof(System.Reflection.ConstructorInfo).IsAssignableFrom(ResolvedMethod.GetType()))
			{
				// Actual constructor call:
				generator.NewObject(ResolvedMethod as System.Reflection.ConstructorInfo);
			}
			else
			{
				
				// Ordinary method:
				generator.Call(ResolvedMethod);
			}
			
			if(IsConstructor){
				
				// Always a returned value here. Needed?
				if(optimizationInfo.RootExpression==this)
				{
					
					// Remove the return value:
					generator.Pop();
					
				}
				
			}else{
				
				if(returnType==typeof(JavaScript.Undefined))
				{
					
					if(optimizationInfo.RootExpression!=this){
						
						// Put undef on the stack:
						EmitHelpers.EmitUndefined(generator);
						
					}
					
				}else if(optimizationInfo.RootExpression==this){
					
					// Remove the return value:
					generator.Pop();
					
				}
				
			}
			
		}
		
		/// <summary>Emits the value for 'this'.</summary>
		private void EmitThis(ILGenerator generator,OptimizationInfo optimizationInfo,out Type thisType){
			
			if(IsConstructor){
				
				// 'This' inside a constructor is something entirely different.
				
				// Get a special instance type:
				Prototype proto=UserDefined.GetInstancePrototype(optimizationInfo.Engine);
				
				// Type of 'this' is..
				thisType=proto.Type;
				
				// Create the object now:
				generator.NewObject(proto.TypeConstructor);
				
				// Duplicate (which will act as our return value):
				if(optimizationInfo.RootExpression!=this){
					
					generator.Duplicate();
					
				}
				
			}else{
				
				if(Imported!=null){
					// Import it:
					generator.LoadField(Imported);
					thisType=Imported.FieldType;
					return;
				}
				
				// There are three cases for non-constructor calls.
				if (this.Target is NameExpression)
				{
					// 1. The function is a name expression (e.g. "parseInt()").
					//	In this case this = scope.ImplicitThisValue, if there is one, otherwise undefined.
					((NameExpression)this.Target).SetupThis(generator.Runtime.Engine,generator,out thisType);
				}
				else if (this.Target is MemberAccessExpression)
				{
					
					// 2. The function is a member access expression (e.g. "Math.cos()").
					//	In this case this = Math.
					var baseExpression = ((MemberAccessExpression)this.Target).Base;
					
					// Get the 'this' type:
					thisType=baseExpression.GetResultType(optimizationInfo);
					
					baseExpression.GenerateCode(generator, optimizationInfo);
					
					// Box it if needed:
					EmitConversion.ToAny(generator, thisType);
					
				}
				else
				{
					
					// 3. Neither of the above (e.g. "(function() { return 5 })()").
					// Global scope here:
					thisType=generator.Runtime.Engine.GlobalScopeType;
				}
				
			}
			
		}
		
		/// <summary>
		/// Generates an array containing the argument values for a tagged template literal.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		/// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
		/// <param name="templateLiteral"> The template literal expression containing the parameter
		/// values. </param>
		internal void GenerateTemplateArgumentsArray(ILGenerator generator, OptimizationInfo optimizationInfo, TemplateLiteralExpression templateLiteral)
		{
			
			optimizationInfo.TypeError("Template arguments are not currently supported.");
			
			/*
			// Generate an array containing the value of each argument.
			generator.LoadInt32(templateLiteral.Values.Count + 1);
			generator.NewArray(typeof(object));

			// Load the first parameter.
			generator.Duplicate();
			generator.LoadInt32(0);
			
			// The first parameter to the tag function is an array of strings.
			var stringsExpression = new List<Expression>(templateLiteral.Strings.Count);
			foreach (var templateString in templateLiteral.Strings)
			{
				stringsExpression.Add(new LiteralExpression(templateString));
			}
			new LiteralExpression(stringsExpression).GenerateCode(generator, optimizationInfo);
			generator.Duplicate();

			// Now we need the name of the property.
			generator.LoadString("raw");

			// Now generate an array of raw strings.
			var rawStringsExpression = new List<Expression>(templateLiteral.RawStrings.Count);
			foreach (var rawString in templateLiteral.RawStrings)
			{
				rawStringsExpression.Add(new LiteralExpression(rawString));
			}
			new LiteralExpression(rawStringsExpression).GenerateCode(generator, optimizationInfo);
			
			// Now store the raw strings as a property of the base strings array.
			// generator.LoadBoolean(optimizationInfo.StrictMode);
			// generator.Call(ReflectionHelpers.ObjectInstance_SetPropertyValue_Object);
			
			// Store in the array.
			generator.StoreArrayElement(typeof(object));
			
			// Values are passed as subsequent parameters.
			for (int i = 0; i < templateLiteral.Values.Count; i++)
			{
				generator.Duplicate();
				generator.LoadInt32(i + 1);
				templateLiteral.Values[i].GenerateCode(generator, optimizationInfo);
				EmitConversion.ToAny(generator, templateLiteral.Values[i].GetResultType(optimizationInfo));
				generator.StoreArrayElement(typeof(object));
			}
			
			*/
			
		}
		
		/// <summary>
		/// Emits the arguments set. 'thisType' is null if the 'this' value has not been emitted at all.
		/// </summary>
		private void EmitArguments(ILGenerator generator,OptimizationInfo optimizationInfo,Type thisType)
		{
			
			// Get the args:
			IList<Expression> arguments = null;
			Expression argumentsOperand = null;
			
			if(OperandCount>1)
			{
				argumentsOperand = this.GetRawOperand(1);
				ListExpression argList = argumentsOperand as ListExpression;
				
				if (argList!=null)
				{
					// Multiple parameters were recieved.
					arguments = argList.Items;
					
					// Set the operand to null so it doesn't try to emit it as a single arg:
					argumentsOperand=null;
				}
			}
			
			int paraCount=0;
			int parameterOffset=0;
			bool staticMethod=false;
			System.Reflection.ParameterInfo[] paraSet=null;
			IList<ArgVariable> argsSet=null;
			
			
			if(UserDefined==null){
				
				// - Is the first arg ScriptEngine?
				// - Does it have thisObj / does it want an instance object?
				paraSet=ResolvedMethod.GetParameters();
				
				paraCount=paraSet.Length;
				
				staticMethod=ResolvedMethod.IsStatic || ResolvedMethod.IsConstructor;
				
				if(paraSet.Length>0)
				{
					
					if(paraSet[0].ParameterType==typeof(ScriptEngine))
					{
						// Emit an engine reference now:
						EmitHelpers.LoadEngine(generator);
						
						parameterOffset++;
					}
					
					if(paraSet.Length>parameterOffset && paraSet[parameterOffset].Name=="thisObj")
					{
						// It's acting like an instance method.
						parameterOffset++;
						staticMethod=false;
					}
				}
				
				if(!staticMethod)
				{
					// Generate the 'this' ref:
					if(this.Target is MemberAccessExpression){
						var baseExpression = ((MemberAccessExpression)this.Target).Base;
						baseExpression.GenerateCode(generator, optimizationInfo);
					}else if(Imported!=null){
						// Imported global scope:
						generator.LoadField(Imported);
					}else if(DelegateFrom==null){
						// Non-static global (error):
						throw new Exception("Non-static global detected (invalid). They must be imported.");
					}
				}
				
			}else{
				// These are always static.
				paraCount=UserDefined.Arguments.Count;
				
				argsSet=UserDefined.Arguments;
				
				// Skip 'this' - it's emitted separately:
				parameterOffset=1;
				
			}
			
			// A local holding the 'this' value. Created if it's needed for anonymous methods.
			ILLocalVariable thisLocal=null;
			
			// Do we need a local var copy of 'Target'? We will if we have a delegate/ Action:
			for(int i=parameterOffset;i<paraCount;i++){
				
				Type paramType=null;
				
				if(paraSet==null){
					
					// Get the type:
					paramType=argsSet[i].Type;
					
				}else{
					
					// Get the parameter's type:
					paramType=paraSet[i].ParameterType;
					
				}
				
				#if NETFX_CORE
				if(typeof(Delegate).GetTypeInfo().IsAssignableFrom(paramType.GetTypeInfo())){
				#else
				if(typeof(Delegate).IsAssignableFrom(paramType)){
				#endif
					// The delegate will need a copy of Target
					// (as it'll be the functions 'this' value).
					bool thisOnStack=(thisType!=null);
					
					if(!thisOnStack){
						// Emit 'this' straight into our local:
						EmitThis(generator,optimizationInfo,out thisType);
					}
					
					// As we have an unknown number of other args in 
					// the way, we'll need to pass it with a local.
					thisLocal = generator.DeclareVariable(thisType,null);
					
					if(thisOnStack){
						// Duplicate it first (it was already on the stack because it's needed elsewhere):
						generator.Duplicate();
					}
					
					generator.StoreVariable(thisLocal);
					
					break;
				}
				
			}
			
			// Next, we're matching params starting from parameterOffset with the args,
			// type casting if needed.
			for(int i=parameterOffset;i<paraCount;i++)
			{
				
				Expression expression=null;
				object defaultValue=null;
				Type paramType=null;
				
				if(paraSet==null){
					
					// Get the type:
					paramType=argsSet[i].Type;
					
				}else{
					
					// Get the parameter info:
					var param=paraSet[i];
					
					// Get the parameter's type:
					paramType=param.ParameterType;
					
					// Get the default value:
					#if NETFX_CORE
					defaultValue=null;
					#else
					defaultValue=param.RawDefaultValue;
					#endif
					
					// Is it a params array?
					#if NETFX_CORE
					if(param.IsDefined(typeof (ParamArrayAttribute)))
					#else
					if(Attribute.IsDefined(param, typeof (ParamArrayAttribute)))
					#endif
					{
						
						// It's always an array - get the element type:
						paramType=paramType.GetElementType();
						
						// For each of the remaining args..
						int offset=i-parameterOffset;
						
						int argCount=0;
						
						if(arguments!=null)
						{
							// Get the full count:
							argCount=arguments.Count;
							
						}
						else if(argumentsOperand!=null)
						{
							// Just one arg and it's still hanging around.
							argCount=offset+1;
						}
						
						// Define an array:
						generator.LoadInt32(argCount);
						generator.NewArray(paramType);
						
						for(int a=offset;a<argCount;a++)
						{
							
							if(arguments!=null)
							{
								// One of many args:
								expression=arguments[a];
							}
							else
							{
								// Just one arg:
								expression=argumentsOperand;
							}
							
							generator.Duplicate();
							generator.LoadInt32(a-offset);
							expression.GenerateCode(generator, optimizationInfo);
							Type res=expression.GetResultType(optimizationInfo);
							
							EmitConversion.Convert(generator, res,paramType);
							generator.StoreArrayElement(paramType);
						}
						
						// All done - can't be anymore.
						break;
					}
				
				}
				
				if(arguments!=null && (i-parameterOffset)<=arguments.Count)
				{
					// Get one of many args:
					expression=arguments[i-parameterOffset];
				}
				else if(argumentsOperand!=null)
				{
					// Just the one argument.
					expression=argumentsOperand;
					
					// By setting it to null after, it can't get emitted again
					// (in the event that this method actually accepts >1 args)
					argumentsOperand=null;
				}
				
				if(expression==null)
				{
					// Emit whatever the default is for the parameters type:
					if(defaultValue!=null)
					{
						// Emit the default value:
						EmitHelpers.EmitValue(generator,defaultValue);
					}
					#if NETFX_CORE
					else if(paramType.GetTypeInfo().IsValueType)
					#else
					else if(paramType.IsValueType)
					#endif
					{
						// E.g. an integer 0
						EmitHelpers.EmitValue(generator,Activator.CreateInstance(paramType));
					}
					else
					{
						// Just a null (a real one):
						generator.LoadNull();
					}
				}
				else if (expression is TemplateLiteralExpression)
                {
                    // Tagged template literal.
                    TemplateLiteralExpression templateLiteral = (TemplateLiteralExpression)expression;
                    GenerateTemplateArgumentsArray(generator, optimizationInfo, templateLiteral);
					return;
				}
				else if(typeof(Delegate).IsAssignableFrom(paramType))
				{
						
					// We're trying to convert the expr into a delegate.
					
					// Hopefully expression is a function of some kind!
					FunctionExpression fe=expression as FunctionExpression;
					
					if(fe!=null){
						
						// Update it with the delegate that we want:
						fe.DelegateToSubscribeTo=paramType;
						
						// 'this' is always the object our function is being called on.
						fe.ThisValue=thisLocal;
						
						// Output the arg (we'll end up with a scope on the stack):
						expression.GenerateCode(generator, optimizationInfo);
						
					}
					
				}
				else
				{
					
					// Output type is..
					Type fromType=expression.GetResultType(optimizationInfo);
					
					// Output the arg:
					expression.GenerateCode(generator, optimizationInfo);
				
					// Convert:
					EmitConversion.Convert(generator,fromType,paramType);
				}
				
			}
			
		}
		
		/// <summary>
		/// Gets the set of types for each argument.
		/// </summary>
		private Type[] GetArgumentTypes(OptimizationInfo optimizationInfo,bool includeThis,FunctionMethodGenerator methodGenerator)
		{
			
			// Get the args:
			Expression argumentsOperand = null;
			
			if(OperandCount>1)
			{
				argumentsOperand = GetRawOperand(1);
				ListExpression argList = argumentsOperand as ListExpression;
				
				if (argList==null)
				{
					// Just one (or two, if it includes 'this'):
					if(includeThis){
						
						return new Type[]{
							GetThisType(optimizationInfo,methodGenerator),
							argumentsOperand.GetResultType(optimizationInfo)
						};
						
					}
					
					return new Type[]{
						argumentsOperand.GetResultType(optimizationInfo)
					};
					
				}
				
				// Multiple parameters were recieved.
				IList<Expression> arguments = argList.Items;
				
				// Set the operand to null so it doesn't try to emit it as a single arg:
				argumentsOperand=null;
				
				int count=arguments.Count;
				
				// Include 'this' if needed:
				if(includeThis){
					
					count++;
					
				}
				
				// Create the type set:
				Type[] result=new Type[count];
				
				int start=0;
				
				if(includeThis){
					
					// Get the 'this' type:
					start=1;
					result[0]=GetThisType(optimizationInfo,methodGenerator);
					
				}
				
				// Get the result type of each one:
				for(int i=start;i<count;i++)
				{
					result[i]=arguments[i-start].GetResultType(optimizationInfo);
				}
				
				return result;
				
			}
			
			// Possibly just the 'this' keyword, if it's a UserDef:
			if(includeThis){
				
				return new Type[]{GetThisType(optimizationInfo,methodGenerator)};
				
			}
			
			// No arguments.
			return null;
			
		}
		
		/// <summary>Gets the type to use for 'this'.</summary>
		private Type GetThisType(OptimizationInfo optimizationInfo,FunctionMethodGenerator methodGenerator){
			
			if(methodGenerator!=null){
				
				// This is a constructor call. The 'this' type is the same as the generators instance prototype:
				return methodGenerator.GetInstancePrototype(optimizationInfo.Engine).Type;
				
			}
			
			// There are three cases for non-constructor calls.
			if (this.Target is NameExpression){
				// 1. The function is a name expression (e.g. "parseInt()").
				//	In this case this = scope.ImplicitThisValue, if there is one, otherwise undefined.
				Type result;
				((NameExpression)this.Target).SetupThis(optimizationInfo.Engine,null,out result);
				return result;
			}
			
			if(this.Target is MemberAccessExpression){
				
				// 2. The function is a member access expression (e.g. "Math.cos()").
				//	In this case this = Math.
				var baseExpression = ((MemberAccessExpression)this.Target).Base;
				
				// Get the 'this' type:
				return baseExpression.GetResultType(optimizationInfo);
			}else{
				// 3. Neither of the above (e.g. "(function() { return 5 })()")
				return optimizationInfo.Engine.GlobalScopeType;
			}
			
		}
		
	}

}