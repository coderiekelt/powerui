using System;
using System.IO;
using JavaScript.Compiler;


namespace WebAssembly{
	
	/// <summary>A WebAssembly 'br_table'.</summary>
	public struct BrTableImmediate{
		
		public LabelledBlock[] Labels;
		public LabelledBlock Default;
		
		
		public BrTableImmediate(LabelledBlock[] table,LabelledBlock defaultTarget){
			Labels = table;
			Default = defaultTarget;
		}
		
	}
	
}