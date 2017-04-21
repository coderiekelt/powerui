using System;
using System.Collections;
using System.Collections.Generic;
using JavaScript;
using System.Reflection;


namespace JavaScript.Compiler{
	
	/// <summary>
	/// Represents an argument of a function.
	/// </summary>
	
	public class ArgVariable : Variable
	{
		
		
		public ArgVariable(string name)
			:base(name)
			{}
		
		/// <summary>The function scope it's declared in.</summary>
		public Scope Scope;
		
		/// <summary>
		/// True if this arg is bouncing via an object[].
		/// </summary>
		public bool Generic;
		
		/// <summary>The argument ID.
		/// Note that hoisted variables can push this over; it is not considered constant until after the AST is complete.
		/// </summary>
		public int ArgumentID;
		
		/// <summary>The args type.</summary>
		private Type _Type;
		
		/// <summary>A variable it's been converted to. Use when pulling an arg into an anonymous method.</summary>
		internal DeclaredVariable Converted;
		
		/// <summary>The set of properties on this argument that the function changes (optionally or otherwise).</summary>
		public Dictionary<string,Type> ChangesProperties;
		
		
		internal override Type Get(ILGenerator generator)
		{
			if(Converted!=null){
				return Converted.Get(generator);
			}
			
			if(Generic){
				
				// Load the object[]:
				generator.LoadArgument(1);
				
				if(ArgumentID == 0){
					// We just loaded the 'this' override - check if it's null.
					generator.LoadInt32(0);
					generator.LoadArrayElement(_Type);
					generator.Duplicate();
					
					ILLabel label = generator.CreateLabel();
					generator.BranchIfNotZero(label);
						
						// It was null - remove the other copy and load the normal arg0:
						generator.Pop();
						generator.LoadArgument(0);
						
						if(generator.InstanceThis!=null){
							// Special case for the 'this' keyword - we're 
							// reading the actual value from a field (of the arg 0 object):
							generator.LoadField(generator.InstanceThis);
						}
					
					// Define the non-zero mark:
					generator.DefineLabelPosition(label);
				}else{
					
					// Check if ArgumentID is in range.
					// If not, emit undefined.
					ILLabel undef = generator.CreateLabel();
					ILLabel end = generator.CreateLabel();
					
					generator.Duplicate();
					generator.LoadArrayLength();
					generator.LoadInt32(ArgumentID);
					generator.BranchIfLessThanOrEqual(undef);
						generator.LoadInt32(ArgumentID);
						generator.LoadArrayElement(_Type);
					generator.Branch(end);
					generator.DefineLabelPosition(undef);
						generator.Pop();
						EmitHelpers.EmitUndefined(generator);
					generator.DefineLabelPosition(end);
					
				}
				
			}else if(generator.Method.IsStatic){
				
				// Special case for 'this' - the static ref doesn't exist:
				if(ArgumentID==0){
					// 
					generator.LoadNull();
				}else{
					generator.LoadArgument(ArgumentID-1);
				}
				
			}else{
				
				// Load the value in the variable
				// (note that there's an ArgVariable for the 'this' keyword too, so ArgumentID is aligned already):
				generator.LoadArgument(ArgumentID);
				
				if(ArgumentID==0 && generator.InstanceThis!=null){
					
					// Special case for the 'this' keyword - we're 
					// reading the actual value from a field (of the arg 0 object):
					generator.LoadField(generator.InstanceThis);
					
				}
				
			}
			
			return _Type;
			
		}
		
		/// <summary>Mark the argument as changed by this function.
		/// This function sets the given property to something of the given type on this argument.</summary>
		public void Changed(string property,Type type)
		{
			
			if(ChangesProperties==null)
			{
				ChangesProperties=new Dictionary<string,Type>();
			}
			else
			{
				
				Type result;
				
				if(ChangesProperties.TryGetValue(property,out result))
				{
					
					if(result==type || result==typeof(object))
					{
						return;
					}
					
					// Sets it to more than one type.
					type=typeof(object);
					
				}
				
			}
			
			// Add it:
			ChangesProperties[property]=type;
			
		}
		
		/// <summary>Converts this arg variable to a 'normal' local.
		/// That allows us to pull it up into another scope and (more broadly) change the type.</summary>
		internal DeclaredVariable ConvertToLocal(){
			
			if(Converted==null){
				// Convert it now:
				Converted=Scope.AddVariable(Name,_Type,null) as DeclaredVariable;
			}
			
			return Converted;
		}
		
		internal override void Set(ILGenerator generator, OptimizationInfo optimizationInfo,bool rIU, Type valueType, SetValueMethod value)
		{
			if(Converted!=null){
				Converted.Set(generator,optimizationInfo,rIU,valueType,value);
				return;
			}
			
			if(_Type==null)
			{
				_Type=valueType;
			}
			else if(valueType!=_Type)
			{
				// Declaring a new variable.
				// This overwrites the named reference in the scope to the new variable
				// (and any users of this ArgVariable object will be redirected).
				
				Converted=Scope.AddVariable(Name,valueType,null) as DeclaredVariable;
				Converted.Set(generator,optimizationInfo,rIU,valueType,value);
				
				return;
			}
			
			if(Generic){
				
				// Load the object[]:
				generator.LoadArgument(1);
				
				// Index argID (covers 'this' too):
				generator.LoadInt32(ArgumentID);
				
				// Load the value:
				value(rIU);
				
				// Store!
				generator.StoreArrayElement(_Type);
				
			}else if(generator.Method.IsStatic){
				
				// Special case for 'this' (does nothing):
				if(ArgumentID!=0){
					
					// Load the value:
					value(rIU);
					
					// Store:
					generator.StoreArgument(ArgumentID-1);
				}
				
			}else{
				
				// Load the value:
				value(rIU);
				
				// Store it in the arg:
				generator.StoreArgument(ArgumentID);
				
			}
			
		}
		
		public Type RawType{
			get{
				return _Type;
			}
			set{
				if(value==null){
					// The type is unknown.
					_Type=null;
					return;
				}
				
				_Type=value;
				
			}
		}
		
		public override Type Type
		{
			get
			{
				if(Converted!=null){
					return Converted.Type;
				}
				return _Type;
			}
			set
			{
				if(Converted!=null){
					Converted.Type=value;
					return;
				}
				
				if(value==null){
					// Null has no effect on variable types.
					return;
				}
				
				_Type=value;
			}
		}
		
	}
	
	
}