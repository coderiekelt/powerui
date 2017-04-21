using System;
using System.Collections;
using System.Collections.Generic;
using JavaScript;
using System.Reflection;


namespace JavaScript.Compiler{
	
	/// <summary>
	/// Represents a remote value (from some parent scope into an anonymous function).
	/// These variables are in the scope of the anonymous function itself.
	/// </summary>
	
	public class RemoteVariable : Variable
	{
		
		internal RemoteVariable(string name,Variable original):base(name){
			// Set orig:
			Original = original as DeclaredVariable;
			
			if(Original==null){
				// It's another remote variable. Double lifting:
				Double = original as RemoteVariable;
				Original = Double.Original;
			}else{
				Original.UsedRemotely=true;
			}
			
		}
		
		/// <summary>A remote var for a 'double lift'.
		/// Note that this is always in the direct parent scope.</summary>
		internal RemoteVariable Double;
		/// <summary>The original remote variable.</summary>
		internal DeclaredVariable Original;
		/// <summary>The function scope it's declared in.</summary>
		public Scope Scope;
		/// <summary>The field that is being used (Always a FieldInfo pointing at a RemoteValue<X>).</summary>
		public PropertyVariable Property;
		
		
		/// <summary>Gets the actual RemoteVariable<X> value.</summary>
		internal Type GetDirect(ILGenerator generator){
			
			// Load the 'this' value:
			generator.LoadArgument(0);
			
			// Load the raw RemoteValue<X> type:
			Property.Get(generator);
			
			return Original.RemoteType;
		}
		
		internal override Type Get(ILGenerator generator)
		{
			// Load the 'this' value:
			generator.LoadArgument(0);
			
			// Load the raw RemoteValue<X> type:
			Property.Get(generator);
			
			// Pull from it:
			generator.LoadField(Original.RemoteValueField);
			
			// It's type is the same as originals:
			return Original.Type;
		}
		
		internal override void Set(ILGenerator generator, OptimizationInfo optimizationInfo,bool rIU, Type valueType, SetValueMethod value)
		{
			// Load the 'this' value:
			generator.LoadArgument(0);
			
			// Load the raw RemoteValue<X> type:
			Property.Get(generator);
			
			// Put the value onto the stack now:
			value(rIU);
			
			if(rIU){
				// Duplicate if necessary:
				generator.Duplicate();
			}
			
			// Save into it:
			generator.StoreField(Original.RemoteValueField);
			
		}
		
		public override Type Type
		{
			get
			{
				return Original.Type;
			}
			set{
				Original.Type=value;
			}
		}
		
	}
	
	
}