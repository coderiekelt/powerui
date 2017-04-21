using System;
using System.Reflection;
using System.Collections.Generic;
using JavaScript;


namespace JavaScript.Compiler
{
	/// <summary>
	/// Represents a function expression.
	/// </summary>
	internal sealed class FunctionExpression : Expression
	{
		private FunctionMethodGenerator context;
		
		/// <summary>
		/// How this function was declared.
		/// </summary>
		public FunctionDeclarationType DeclarationType;
		
		/// <summary>
		/// If this function expression has a name in the 
		/// parent scope, this is the variable.</summary>
		public Variable ParentVar;
		
		/// <summary>
		/// The requested 'this' value is held in here.
		/// </summary>
		public ILLocalVariable ThisValue=null;
		/// <summary>
		/// The delegate to subscribe the function to.
		/// Note that a fallback is always generated (using JavaScript.JSFunction).
		/// </summary>
		public Type DelegateToSubscribeTo=typeof(JavaScript.JSFunction);
		
		/// <summary>
		/// Creates a new instance of FunctionExpression.
		/// </summary>
		public FunctionExpression()
		{}
		
		internal FunctionMethodGenerator Context{
			get{
				return context;
			}
		}
		
		public override object Evaluate(){
			
			return Context;
			
		}
		
		internal override void ResolveVariables(OptimizationInfo optimizationInfo)
		{
			if(context.AbstractSyntaxTree==null){
				context.Parse();
			}
			
			base.ResolveVariables(optimizationInfo);
		}
		
		/// <param name="functionContext"> The function context to base this expression on. </param>
		internal void SetContext(FunctionMethodGenerator functionContext)
		{
			if (functionContext == null)
				throw new ArgumentNullException("functionContext");
			this.context = functionContext;
		}
		
		/// <summary>
		/// Gets or sets the scope the function is declared within.
		/// </summary>
		public Scope ParentScope{
			get{
				return context.InitialScope;
			}
		}
		
		/// <summary>
		/// Gets the name of the function.
		/// </summary>
		public string FunctionName
		{
			get { return this.context.Name; }
		}

		/// <summary>
		/// Gets a list of argument names.
		/// </summary>
		public IList<ArgVariable> Arguments
		{
			get { return this.context.Arguments; }
		}
		
		/// <summary>
		/// Gets the type that results from evaluating this expression.
		/// </summary>
		public override Type GetResultType(OptimizationInfo optimizationInfo)
		{	
			return DelegateToSubscribeTo;
		}

		/// <summary>
		/// Generates CIL for the expression.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		/// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
		public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
		{
			
			// If the return value isn't being used then we're setting to the 
			// name variable in the parent scope (as a delegate).
			if(optimizationInfo.RootExpression==this){
				
				if(ParentVar==null){
					return;
				}
				
				// E.g. function hello(){} in the global scope.
				// This acts similar to var hello = function(){} by setting a delegate.
				ParentVar.Set(generator, optimizationInfo,false,DelegateToSubscribeTo,delegate(bool rIU){
					
					// Output the delegate:
					GenerateDelegate(generator,optimizationInfo);
					
				});
				
				return;
			}
			
			// Output a delegate to the parent:
			GenerateDelegate(generator,optimizationInfo);
			
		}
		
		/// <summary>Generates a delegate instance of the type DelegateToSubscribeTo.</summary>
		private void GenerateDelegate(ILGenerator generator, OptimizationInfo optimizationInfo){
			
			// Create a scope object for hoisting into a delegate:
			Prototype proto=optimizationInfo.Engine.Prototypes.Create();
			
			// Get my method scope:
			DeclarativeScope scope=ParentScope as DeclarativeScope;
			
			// Get remote vars from the scope:
			List<RemoteVariable> remotes=scope.GetRemotes();
			
			if(remotes!=null){
				
				// For each one, create it as a proto property:
				for (int i = 0; i < remotes.Count; i++){
					
					RemoteVariable hv=remotes[i];
					
					// Get the actual variable type:
					Type type=hv.Original.RemoteType;
					
					// Add the property:
					hv.Property=proto.AddProperty(
						hv.Name,
						PropertyAttributes.FullAccess,
						type
					);
					
				}
				
			}
			
			// The this type is..
			Type thisType;
			
			if(ThisValue==null || DelegateToSubscribeTo == typeof(JSFunction)){
				thisType = typeof(object);
			}else{
				thisType = ThisValue.Type;
			}
			
			// Add the 'this' type next:
			// (as our prototype instance is the actual 'this'):
			PropertyVariable thisProto=proto.AddProperty(
				"__this__",
				PropertyAttributes.FullAccess,
				thisType
			);
			
			// Note that the method generator will pull the field based on the __this__ name.
			
			// Compile the function such that its signature matches DelegateToSubscribeTo.
			// It must be an instance method of proto for that to work!
			UserDefinedFunction udf=context.GetCompiled(DelegateToSubscribeTo,thisType,optimizationInfo,proto);
			
			// Instance the scope object:
			generator.NewObject(proto.TypeConstructor);
			
			// Do nothing if it's a static type (in which case it'll just be a null reference).
			#if NETFX_CORE
			if(ThisValue!=null && !(thisType.GetTypeInfo().IsAbstract && thisType.GetTypeInfo().IsSealed)){
			#else
			if(ThisValue!=null && !(thisType.IsAbstract && thisType.IsSealed)){
			#endif
			
				// Dupe the object:
				generator.Duplicate();
				
				// Apply the this value:
				thisProto.Set(generator,optimizationInfo,false,thisType,delegate(bool rIU){
					
					// (rIU is always false)
					
					// Read the value:
					generator.LoadVariable(ThisValue);
					
				});
				
			}
			
			if(remotes!=null){
				
				// Transfer the variables now:
				for (int i = 0; i < remotes.Count; i++){
					
					// Dupe the object:
					generator.Duplicate();
					
					// Get the remote var:
					RemoteVariable hv=remotes[i];
					
					// Set it:
					hv.Property.Set(generator,optimizationInfo,false,hv.Original.RemoteType,delegate(bool rIU){
						
						// (rIU is always false)
						
						if(hv.Double!=null){
							
							// Double lift! Get the value from the raw property:
							hv.Double.GetDirect(generator);
							
						}else{
							// Read the value *as is*:
							hv.Original.GetDirect(generator);
						}
						
					});
					
				}
				
			}
			
			// - Delegate's 'this' is now on the stack -
			// Create an object of that delegate type next.
			
			// For verification a dup is required here:
			generator.Duplicate();
			
			// Load the method pointer:
			generator.LoadMethodPointer(udf.body);
			
			// Create the delegate object:
			generator.NewObject(DelegateToSubscribeTo.GetConstructors()[0]);
			
		}
		
		/// <summary>
		/// Converts the expression to a string.
		/// </summary>
		/// <returns> A string representing this expression. </returns>
		public override string ToString()
		{
			return context.ToString();
		}
	}

}