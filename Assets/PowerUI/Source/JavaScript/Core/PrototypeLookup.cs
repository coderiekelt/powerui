using System;
using System.Collections;
using System.Collections.Generic;
using JavaScript;
using System.Reflection;


namespace JavaScript{
	
	/// <summary>
	/// A lookup from system type to prototype object.
	/// </summary>
	public class PrototypeLookup{
		
		internal Prototype BasePrototype;
		
		internal Prototype ExtendablePrototype;
		internal Prototype BooleanPrototype;
		internal Prototype NumberPrototype;
		internal Prototype StringPrototype;
		internal Prototype RegExpPrototype;
		internal Prototype ObjectPrototype;
		internal Prototype ArrayPrototype;
		internal Prototype FunctionPrototype;
		
		/// <summary>
		/// Gets the built-in Array constructor.
		/// </summary>
		public MethodBase Array;
		
		/// <summary>
		/// Gets the built-in Date constructor.
		/// </summary>
		public Prototype Date;
		
		/// <summary>
		/// Gets the built-in RegExp constructor.
		/// </summary>
		public MethodBase RegExp;
		
		// The built-in error objects.
		
		/// <summary>
		/// Gets the built-in Error constructor.
		/// </summary>
		public Prototype Error;
		/// <summary>
		/// Gets the built-in RangeError constructor.
		/// </summary>
		public Prototype RangeError;
		/// <summary>
		/// Gets the built-in TypeError constructor.
		/// </summary>
		public Prototype TypeError;
		/// <summary>
		/// Gets the built-in TypeError constructor.
		/// </summary>
		public Prototype SyntaxError;
		/// <summary>
		/// Gets the built-in TypeError constructor.
		/// </summary>
		public Prototype URIError;
		/// <summary>
		/// Gets the built-in TypeError constructor.
		/// </summary>
		public Prototype EvalError;
		/// <summary>
		/// Gets the built-in TypeError constructor.
		/// </summary>
		public Prototype ReferenceError;
		
		/// <summary>
		/// The parent script engine.
		/// </summary>
		public ScriptEngine Engine;
		
		/// <summary>
		/// The instance lookup.
		/// </summary>
		public Dictionary<Type,Prototype> MainLookup=new Dictionary<Type,Prototype>();
		
		/// <summary>
		/// The constructor (static) lookup.
		/// </summary>
		public Dictionary<Type,Prototype> CtrLookup=new Dictionary<Type,Prototype>();
		
		
		public PrototypeLookup(ScriptEngine engine){
			Engine=engine;
		}
		
		/// <summary>Sets up the prototype lookup, optionally 
		/// setting up the global cache (compiling platforms only).</summary>
		internal void Setup(object imports,bool withGlobalCache){
			
			// Create the base of the prototype chain:
			BasePrototype=new Prototype(Engine);
			
			if(withGlobalCache){
				
				// Create the prototype for the global scope:
				Engine.GlobalPrototype = CreateStatic();
				
				// Setup the global cache:
				Engine.ModuleInfo.SetupGlobalCache(Engine.GlobalPrototype,imports);
				
			}
			
			// Setup the object constructor:
			ObjectPrototype = SetConstructor(typeof(object));
			
			// Setup the function constructor:
			FunctionPrototype = SetConstructor(typeof(FunctionProto));
			
			// Initialize the prototypes for the base of the prototype chain.
			BasePrototype.AddConstructor(ObjectPrototype);
			
			ExtendablePrototype=Get(typeof(JavaScript.Extendable));
			BooleanPrototype=Get(typeof(JavaScript.Boolean));
			NumberPrototype=Get(typeof(JavaScript.Number));
			StringPrototype=Get(typeof(JavaScript.String));
			RegExpPrototype=Get(typeof(JavaScript.RegExp));
			ArrayPrototype=Get(typeof(JSArray));
			
			Array=ArrayPrototype.Constructor as MethodBase;
			Date=SetConstructor(typeof(JavaScript.Date));
			RegExp=RegExpPrototype.Constructor as MethodBase;
			
			// Create the error functions.
			Error = SetConstructor(typeof(JavaScript.Error));
			RangeError = SetConstructor(typeof(RangeError));
			TypeError = SetConstructor(typeof(TypeError));
			SyntaxError = SetConstructor(typeof(SyntaxError));
			URIError = SetConstructor(typeof(URIError));
			EvalError = SetConstructor(typeof(EvalError));
			ReferenceError = SetConstructor(typeof(ReferenceError));
			
			// Create the typed array functions.
			Get(typeof(ArrayBuffer));
			Get(typeof(DataView));
			
			SetConstructor(typeof(Int8Array));
			SetConstructor(typeof(Uint8Array));
			SetConstructor(typeof(Uint8ClampedArray));
			SetConstructor(typeof(Int16Array));
			SetConstructor(typeof(Uint16Array));
			SetConstructor(typeof(Int32Array));
			SetConstructor(typeof(Uint32Array));
			SetConstructor(typeof(Float32Array));
			SetConstructor(typeof(Float64Array));
			// SetConstructor(typeof(ArrayBuffer));
			
			// Engine.SetGlobal("JSON",new JSON());
			Engine.SetGlobal("console",new console(Engine));
			Engine.SetGlobal("Math",new MathObject());
			
			if(imports==null){
				// No imported proto:
				Engine.ImportedGlobalPrototype=null;
			}else{
				// Get the proto:
				Engine.ImportedGlobalPrototype=Get(imports.GetType());
				
				// Merge the imported properties into the global scope, 
				// marking them as imported from GlobalImports_:
				foreach(KeyValuePair<string,PropertyVariable> kvp in Engine.ImportedGlobalPrototype.Properties){
					
					// Mark as imported:
					kvp.Value.ImportedFrom=Engine.ModuleInfo.GlobalImports;
					
					// Add it:
					Engine.GlobalPrototype.Properties[kvp.Key]=kvp.Value;
					
				}
				
			}
			
		}
		
		/// <summary>Creates a new prototype, suitable for adding extra properties to.</summary>
		public Prototype CreateStatic(){
			Prototype proto=new Prototype(Engine,"_global",null,true,null);
			MainLookup[proto.Type]=proto;
			CtrLookup[proto.Type]=proto;
			return proto;
		}
		
		/// <summary>Creates a new prototype, suitable for adding extra properties to.</summary>
		public Prototype Create(){
			Prototype proto=new Prototype(Engine,null,null,false,null);
			MainLookup[proto.Type]=proto;
			return proto;
		}
		
		/// <summary>Sets the given type as a global constructor.</summary>
		private Prototype SetConstructor(Type type)
		{
			Prototype prototype;
			return SetConstructor(type,out prototype);
		}
		
		/// <summary>Sets the given type as a global constructor.</summary>
		private Prototype SetConstructor(Type type,out Prototype prototype)
		{
			// Get the prototype:
			prototype=Get(type);
			
			// Build the constructor prototype:
			Prototype ctrProto=prototype.ConstructorPrototype;
			
			if(ctrProto==null)
			{
				throw new Exception("The type '"+type.Name+"' is not constructable.");
			}
			
			// Create the global (as a static type):
			Engine.SetGlobalType(prototype.Name,ctrProto.Type,PropertyAttributes.FullAccess);
			
			// Return it:
			return ctrProto;
		}
		
		/// <summary>
		/// The number of prototypes in this lookup.
		/// </summary>
		public int Count{
			get{
				return MainLookup.Count;
			}
		}
		
		/// <summary>Gets the prototype instance for the given type. Assumes it exists.</summary>
		public Prototype this[Type type]{
			get{
				Prototype result;
				if(!MainLookup.TryGetValue(type,out result)){
					CtrLookup.TryGetValue(type,out result);
				}
				return result;
			}
		}
		
		/// <summary>
		/// Gets or creates the prototype for the given type.
		/// </summary>
		public Prototype Get(string typeName){
			return Get(typeName,null);
		}
		
		/// <summary>
		/// Gets or creates the prototype for the given type.
		/// </summary>
		public Prototype Get(string typeName,Prototype parentProto){
			
			// Get the type:
			Type type=CodeReference.GetFirstType(typeName);
			
			// Resolve the prototype:
			return Get(type,parentProto);
			
		}
		
		/// <summary>
		/// Gets or creates the prototype for the given object.
		/// </summary>
		public Prototype Get(object forObject){
			if(forObject==null){
				return null;
			}
			return Get(forObject.GetType(),null);
		}
		
		/// <summary>
		/// Gets or creates the prototype for the given type.
		/// </summary>
		public Prototype Get(Type type){
			return Get(type,null);
		}
		
		public void CompleteAll(){
			
			foreach(KeyValuePair<Type,Prototype> kvp in MainLookup){
				
				kvp.Value.Complete();
				
			}
			
			foreach(KeyValuePair<Type,Prototype> kvp in CtrLookup){
				
				kvp.Value.Complete();
				
			}
			
		}
		
		/// <summary>
		/// Gets or creates the prototype for the given type with an optional parent prototype.
		/// </summary>
		public Prototype Get(Type type,Prototype parentProto){
			
			if(type==null){
				return null;
			}
			
			string name=type.Name;
			
			JSProperties attribute;
			
			if(type.GetType() == typeof(System.Type)){
				
				#if NETFX_CORE
				attribute = (JSProperties)type.GetTypeInfo().GetCustomAttribute(typeof(JSProperties),false);
				#else
				attribute = (JSProperties)Attribute.GetCustomAttribute(type,typeof(JSProperties),false);
				#endif
				
				if (attribute != null && attribute.Name!=null)
				{
					name=attribute.Name;
				}
				
			}else{
				
				// TypeBuilder.
				attribute=null;
				
			}
			
			Prototype result;
			if(MainLookup.TryGetValue(type,out result)){
				return result;
			}
			
			// Get the parent proto (obj if none):
			if(parentProto==null){
				
				#if NETFX_CORE
				Type baseType=type.GetTypeInfo().BaseType;
				#else
				Type baseType=type.BaseType;
				#endif
				
				if(baseType==typeof(object) || baseType==null){
					
					// Get the very bottom of the proto stack:
					parentProto=Engine.Prototypes.BasePrototype;
					
				}else{
					
					#if NETFX_CORE
					parentProto=Get(type.GetTypeInfo().BaseType);
					#else
					parentProto=Get(type.BaseType);
					#endif
					
				}
			}
			
			// Create and add it:
			result=new Prototype(Engine,name,parentProto,type);
			MainLookup[result.Type]=result;
			
			// Does it declare PrototypeFor?
			FieldInfo protoFor=type.GetField("PrototypeFor");
			
			if(protoFor!=null)
			{
				// Yes - get the type it's a prototype for:
				object value=protoFor.GetValue(null);
				
				// It can actually be a set, so check if it's just one first:
				Type protoForType=value as Type;
				
				if(protoForType!=null)
				{
					// It's just the one.
					MainLookup[protoForType]=result;
				}
				else
				{
					// It's a set - add each one.
					// For example, Function acts as the prototype for both MethodBase and MethodGroup.
					Type[] set=value as Type[];
					
					for(int i=0;i<set.Length;i++)
					{
						MainLookup[set[i]]=result;
					}
				}
			}
			
			// Can this prototype be instanced?
			// - Not a static class
			// - Doesn't have "NoConstructors" set
			// - Isn't a cached global scope
			#if NETFX_CORE
			bool instanceable=(!type.GetTypeInfo().IsAbstract && !(attribute!=null && attribute.NoConstructors) && name!="_global");
			#else
			bool instanceable=(!type.IsAbstract && !(attribute!=null && attribute.NoConstructors) && name!="_global");
			#endif
			
			if(instanceable)
			{
				
				if(type==typeof(object)){
					name="_base_object";
				}
				
				Prototype ctr=null;
				
				// Build the constructor prototype:
				ctr=new Prototype(Engine,name+"_ctor",null,type);
				ctr.PopulateConstructorPrototype(type);
				ctr.AddProperty("constructor",FunctionPrototype,PropertyAttributes.NonEnumerable);
				result.AddProperty("constructor",ctr,PropertyAttributes.NonEnumerable);
				result.ConstructorPrototype=ctr;
				
				// ctr is a function instance, so set its base proto:
				ctr.BasePrototype=FunctionPrototype;
				
				// Add to the constructor lookup:
				CtrLookup[ctr.Type]=ctr;
				
				// Setup the instance proto:
				result.PopulateInstancePrototype(type);
				
			}else{
				
				// Not instanceable - has no constructors or a constructor proto.
				// Init the single prototype now:
				result.PopulatePrototype(type,false);
				
			}
			
			return result;
		}
		
	}
	
}