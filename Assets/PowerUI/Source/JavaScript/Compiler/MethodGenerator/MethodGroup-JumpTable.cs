using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using JavaScript.Compiler;


namespace JavaScript
{

	public partial class MethodGroup
	{
		
		/// <summary>
		/// Emits a jump table for this method group. The integer index must be on the stack.
		/// Each index in the table is simply a Call instruction.
		/// </summary>
		public void GenerateJumpTable(ILGenerator generator)
		{
			
			// The number of methods:
			int methodCount=Methods.Count;
			
			// Create the set of switch labels, one for each method:
			ILLabel[] switchLabels = new ILLabel[Methods.Count];
			for (int i = 0; i < methodCount; i++)
			{
				switchLabels[i] = generator.CreateLabel();
			}
			
			// Emit the switch instruction now:
			generator.Switch(switchLabels);
			
			// Create the end label:
			ILLabel end=generator.CreateLabel();
			
			// For each method, mark the label and emit the call.
			for (int i = 0; i < methodCount; i++)
			{
				// Mark the label:
				generator.DefineLabelPosition(switchLabels[i]);
				
				// Get the method:
				MethodBase method=Methods[i];
				
				// Emit the call now:
				if(method.IsConstructor)
				{
					// Actual constructor call:
					generator.NewObject(method as System.Reflection.ConstructorInfo);
				}
				else
				{
					// Call it now:
					generator.Call(method);
				}
				
				generator.Branch(end);
			}
			
			generator.DefineLabelPosition(end);
			
		}
		
	}
	
}