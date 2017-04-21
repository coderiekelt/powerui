using System;
using System.Collections.Generic;
using JavaScript;

namespace JavaScript{
	
	/// <summary>
	/// During compilation, global variables are not directly settable.
	/// So, we have to create a bunch of 'waiting' set objects which will
	/// set the global values when it's ready.
	/// </summary>
	
	internal class AwaitingStart{
		
		private object Value;
		private PropertyVariable Variable;
		
		
		public AwaitingStart(PropertyVariable variable,object value){
			
			Variable=variable;
			Value=value;
			
		}
		
		public void OnStart(ScriptEngine engine){
			
			// Set now:
			Variable.SetValue(null,Value);
			
		}
		
	}
	
}