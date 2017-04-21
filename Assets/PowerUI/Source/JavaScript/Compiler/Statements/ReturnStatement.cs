using System;
using System.Reflection;
using System.Collections.Generic;


namespace JavaScript.Compiler
{

	/// <summary>
	/// Represents a return statement.
	/// </summary>
	internal class ReturnStatement : Statement
	{
		
		/// <summary>
		/// Gets or sets the expression to return.  Can be <c>null</c> to return "undefined".
		/// </summary>
		public Expression Value;
		/// <summary>
		/// The return type.
		/// </summary>
		public Type ReturnType;
		
		internal override void ResolveVariables(OptimizationInfo optimizationInfo)
		{
			
			if(optimizationInfo.IsConstructor){
				ReturnType=null;
				ScriptEngine.Log("You've got a return statement inside a function which is being used as a constructor. It's been ignored.");
				return;
			}
			
			if(Value==null){
				
				// No value - this will return void:
				ReturnType=typeof(void);
				
			}else{
				
				// Resolve vars:
				Value.ResolveVariables(optimizationInfo);
				
				// Get return type:
				ReturnType=Value.GetResultType(optimizationInfo);
				
			}
			
			if(optimizationInfo.SingleReturnType==null){
				
				// Only this one.
				optimizationInfo.SingleReturnType=ReturnType;
				
			}else{
				
				if(optimizationInfo.ReturnVariable==null){
					// Create variable with the type from the prev SRT:
					optimizationInfo.ReturnVariable = new DeclaredVariable("__retv");
					optimizationInfo.ReturnVariable.Type=optimizationInfo.SingleReturnType;
				}
				
				// Update type again:
				optimizationInfo.ReturnVariable.Type=ReturnType;
			
			}
			
		}
		
		/// <summary>
		/// Generates CIL for the statement.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		/// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
		public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
		{
			
			if(ReturnType == null){
				return;
			}
			
			// Determine if this is the last statement in the function.
			BlockStatement hostBlock=optimizationInfo.AbstractSyntaxTree as BlockStatement;
			
			bool lastStatement;
			
			if(hostBlock!=null && hostBlock.LastStatement == this){
					lastStatement=true;
			}else{
				lastStatement=false;
			}
			
			// Current return var:
			var returnVar=optimizationInfo.ReturnVariable;
			
			if(returnVar==null){
				
				// Directly output - there can only ever be one of these return statements
				// (Thus we can't possibly collapse the return type).
				if (Value == null){
					
					// If a return type is required, emit undefined:
					if(optimizationInfo.ReturnType!=null && optimizationInfo.ReturnType!=typeof(void)){
						EmitHelpers.EmitUndefined(generator);
					}
					
				}else{
					// Emit the returned value:
					Value.GenerateCode(generator, optimizationInfo);
					
					#if NETFX_CORE
					if(optimizationInfo.ReturnType==typeof(object) && ReturnType.GetTypeInfo().IsValueType){
					#else
					if(optimizationInfo.ReturnType==typeof(object) && ReturnType.IsValueType){
					#endif
						// Box
						generator.Box(ReturnType);
					}
					
				}
				
			}else{
				// the return type here must be a child type of the return type.
				// If it isn't then this function returns different things.
				
				// Store the return value in a variable.
				returnVar.Set(generator,optimizationInfo,false,ReturnType,delegate(bool rIU){
					
					if (Value != null){
						
						// Emit the returned value:
						Value.GenerateCode(generator, optimizationInfo);
						
					}else{
						
						// No value being set - emit undefined:
						EmitHelpers.EmitUndefined(generator);
						
					}
					
				});
				
			}
			
			// There is no need to jump to the end of the function if this is the last statement.
			if (!lastStatement)
			{
				
				// The first return statement that needs to branch creates the return label.  This is
				// defined in FunctionmethodGenerator.GenerateCode at the end of the function.
				if (optimizationInfo.ReturnTarget == null)
					optimizationInfo.ReturnTarget = generator.CreateLabel();

				// Branch to the end of the function.  Note: the return statement might be branching
				// from inside a try { } or finally { } block to outside.  EmitLongJump() handles this.
				optimizationInfo.EmitLongJump(generator, optimizationInfo.ReturnTarget);

			}
			
		}

		/// <summary>
		/// Gets an enumerable list of child nodes in the abstract syntax tree.
		/// </summary>
		public override IEnumerable<AstNode> ChildNodes
		{
			get
			{
				if (this.Value != null)
					yield return this.Value;
			}
		}

		/// <summary>
		/// Converts the statement to a string.
		/// </summary>
		/// <param name="indentLevel"> The number of tabs to include before the statement. </param>
		/// <returns> A string representing this statement. </returns>
		public override string ToString(int indentLevel)
		{
			var result = new System.Text.StringBuilder();
			result.Append(new string('\t', indentLevel));
			result.Append("return");
			if (this.Value != null)
			{
				result.Append(" ");
				result.Append(this.Value);
			}
			result.Append(";");
			return result.ToString();
		}
	}

}