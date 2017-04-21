using System;
using System.Collections.Generic;

namespace JavaScript.Compiler
{

	/// <summary>
	/// Represents a javascript block statement.
	/// </summary>
	internal class BlockStatement : Statement
	{
		/// <summary>A linked list of statements in this block.</summary>
		private Statement FirstStatement_;
		/// <summary>A linked list of statements in this block.</summary>
		private Statement LastStatement_;
		
		
		/// <summary>The last statement. May be null.</summary>
		public Statement LastStatement{
			get{
				return LastStatement_;
			}
		}
		
		/// <summary>Adds a statement to this block. Functions are hoisted.</summary>
		public void Add(Statement statement){
			
			ExpressionStatement expr = (statement as ExpressionStatement);
			
			if(FirstStatement_==null){
				FirstStatement_=LastStatement_=statement;
			}else if(expr!=null && expr.Expression is FunctionExpression){
				
				// We've got a function! Hoist it by placing the statement at the front:
				expr.Next=FirstStatement_;
				FirstStatement_=expr;
				
			}else{
				LastStatement_.Next=statement;
				LastStatement_=statement;
			}
			
		}
		
		/// <summary>
		/// Generates CIL for the statement.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		/// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
		public override void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
		{
			
			// Parent root:
			Expression rootExpression=optimizationInfo.RootExpression;
			
			Statement current = FirstStatement_;
			
			while(current!=null){
				// Generate code for the statement.
				
				// Apply the root:
				current.SetRoot(optimizationInfo);
				
				// Generate the code:
				current.GenerateCode(generator, optimizationInfo);
				
				current = current.Next;
			}
			
			// Restore root:
			optimizationInfo.RootExpression=rootExpression;
			
		}

		/// <summary>
		/// Gets an enumerable list of child nodes in the abstract syntax tree.
		/// </summary>
		public override IEnumerable<AstNode> ChildNodes
		{
			get
			{
				Statement current = FirstStatement_;
				while(current!=null){
					yield return current;
					current = current.Next;
				}
			}
		}

		/// <summary>
		/// Converts the statement to a string.
		/// </summary>
		/// <param name="indentLevel"> The number of tabs to include before the statement. </param>
		/// <returns> A string representing this statement. </returns>
		public override string ToString(int indentLevel)
		{
			indentLevel = Math.Max(indentLevel - 1, 0);
			var result = new System.Text.StringBuilder();
			result.Append(new string('\t', indentLevel));
			result.AppendLine("{");
				
				Statement current = FirstStatement_;
				
				while(current!=null){
					result.AppendLine(current.ToString(indentLevel + 1));
					current = current.Next;
				}
			
			result.Append(new string('\t', indentLevel));
			result.Append("}");
			return result.ToString();
		}
	}

}