using System;
using System.Reflection;


namespace JavaScript.Compiler{
	
	/// <summary>
	/// Represents a variable declared in a scope.
	/// </summary>
	internal class DeclaredVariable : Variable
	{
		
		// The initial value of the variable (used for function declarations only).
		public Expression ValueAtTopOfScope;

		// true if the variable has been set with the initial value.
		public bool Initialized;
		
		// The storage container for the variable.
		public ILLocalVariable Store;
		
		/// <summary>True if this is used by an anonymous function.
		/// We'll set it up as a RemoteLocal<Type_>.</summary>
		public bool UsedRemotely;
		
		// The statically-determined storage type for the variable.
		private Type Type_ = null;
		
		/// <summary>The 'actual' underlying type of this field (set when UsedRemotely is true).</summary>
		private Type RemoteType_ =null;
		/// <summary>The 'Value' field from a remote version of this variables type. 
		/// RemoteLocal<Type_>. Set when UsedRemotely is true.</summary>
		internal FieldInfo RemoteValueField = null;
		
		internal DeclaredVariable(string name){
			Name=name;
		}
		
		/// <summary>Clears the method specific values in this declared variable.</summary>
		internal void Reset(){
			Store=null;
			Type_=null;
			RemoteType_=null;
			RemoteValueField=null;
		}
		
		internal override Type Get(ILGenerator generator)
		{
			
			if(Store==null){
				throw new Exception("Use of a variable called '"+Name+"' which has never been set.");
			}
			
			// Load the value in the variable:
			generator.LoadVariable(Store);
			
			if(UsedRemotely){
				// Pull the underlying variable from it:
				generator.LoadField(RemoteValueField);
			}
			
			return Type_;
			
		}
		
		/// <summary>The 'actual' remote type.</summary>
		public Type RemoteType{
			get{
				if(RemoteType_==null){
					return Type_;
				}
				
				return RemoteType_;
			}
		}
		
		/// <summary>The same as a get only it ignores UsedRemotely and pulls the value as-is.</summary>
		public Type GetDirect(ILGenerator generator){
			
			// Load the value in the variable:
			generator.LoadVariable(Store);
			
			return RemoteType;
		}
		
		internal override void Set(ILGenerator generator, OptimizationInfo optimizationInfo,bool rIU, Type valueType, SetValueMethod value)
		{
			Type=valueType;
			
			// Declare an IL local variable if no storage location has been allocated yet.
			if (Store == null)
			{
				
				if(UsedRemotely){
					
					// Declare it as RemoteLocal<Type_>:
					RemoteType_=typeof(RemoteLocal<>).MakeGenericType(new Type[] { Type_ });
					
					// Get the Value field for quick access:
					RemoteValueField = RemoteType_.GetField("Value");
					
					// Create the local and set the initial value:
					Store = generator.DeclareVariable(RemoteType_, Name);
					
					// Create the container:
					generator.NewObject(RemoteType_.GetConstructors()[0]);
					
					// Store it in the local:
					generator.StoreVariable(Store);
					
				}else{
					Store = generator.DeclareVariable(Type_, Name);
				}
				
			}
			
			if(UsedRemotely){
				
				// Load the local:
				generator.LoadVariable(Store);
				
				// Load the value:
				value(rIU);
				
				// Write it into the value field:
				generator.StoreField(RemoteValueField);
				
			}else{
				
				// Load the value:
				value(rIU);
				
				#if NETFX_CORE
				if(Type_==typeof(object) && valueType.GetTypeInfo().IsValueType){
				#else
				if(Type_==typeof(object) && valueType.IsValueType){
				#endif
					// Box it:
					generator.Box(valueType);
				}
				
				// Store the value in the variable.
				generator.StoreVariable(Store);
				
			}
			
		}
		
		public override Type Type
		{
			get
			{
				return Type_;
			}
			set
			{
				if(value==null){
					// Null has no effect on variable types.
					return;
				}
				
				if(Type_==null || Type_==typeof(JavaScript.Undefined)){
					Type_=value;
				}else if(Type_==typeof(object)){
					// Already collapsed.
					return;
				#if NETFX_CORE
				}else if(!Type_.GetTypeInfo().IsAssignableFrom(value.GetTypeInfo())){
				#else
				}else if(!Type_.IsAssignableFrom(value)){
				#endif
					// The variable has collapsed.
					Type_=typeof(object);
				}
				
			}
		}
		
		
	}
	
}