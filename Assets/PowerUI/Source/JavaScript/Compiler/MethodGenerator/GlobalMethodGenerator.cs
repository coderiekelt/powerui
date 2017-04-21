using System;
using System.Collections.Generic;

namespace JavaScript.Compiler
{
	/// <summary>
	/// Represents the information needed to compile global code.
	/// </summary>
	public class GlobalMethodGenerator : MethodGenerator
	{
		
		/// <summary>
		/// Creates a new GlobalMethodGenerator instance.
		/// </summary>
		/// <param name="engine"> The script engine. </param>
		/// <param name="source"> The source of javascript code. </param>
		/// <param name="options"> Options that influence the compiler. </param>
		public GlobalMethodGenerator(ScriptEngine engine, string source)
			: base(engine, new ObjectScope(engine,engine.GlobalPrototype))
		{
			
			Source=source;
			
			// Apply global scope:
			Engine.GlobalScope=InitialScope as ObjectScope;
			
			// Set the global 'this':
			Engine.SetGlobalType("this",Engine.GlobalScopeType,PropertyAttributes.Sealed);
			
		}
		
		/// <summary>
		/// Gets a name for the generated method.
		/// </summary>
		/// <returns> A name for the generated method. </returns>
		protected override string GetMethodName()
		{
			return "__.main";
		}

		/// <summary>
		/// Parses the source text into an abstract syntax tree.
		/// </summary>
		public override void Parse()
		{
			using (var lexer = new Lexer(this.Engine, this.Source))
			{
				var parser = new Parser(this.Engine, lexer, this.InitialScope, OptimizationInfo, CodeContext.Global);
				this.AbstractSyntaxTree = parser.Parse();
				this.StrictMode = parser.StrictMode;
				this.MethodOptimizationHints = parser.MethodOptimizationHints;
			}
		}

		/// <summary>
		/// Generates IL for the script.
		/// </summary>
		/// <param name="generator"> The generator to output the CIL to. </param>
		/// <param name="optimizationInfo"> Information about any optimizations that should be performed. </param>
		protected override void GenerateCode(ArgVariable[] arguments,ILGenerator generator)
		{
			// Generate code for the source code.
			this.AbstractSyntaxTree.GenerateCode(generator, OptimizationInfo);
		}
		
	}

}