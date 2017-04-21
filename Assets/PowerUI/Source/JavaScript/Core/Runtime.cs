//--------------------------------------
//         Nitro Script Engine
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace JavaScript{
	
	/// <summary>
	/// A JavaScript runtime engine. Either a ScriptEngine instance or a WebAssembly module.
	/// </summary>
	
	public partial class Runtime{
		
		/// <summary>Either a direct reference to 'this' or the ScriptEngine 
		/// that a WebAssembly module is in.</summary>
		public ScriptEngine Engine;
		
		
		/// <summary>Invoked when WebAssembly hits a trap line.</summary>
		public static void Trap(){
			throw new Exception("The WebAssembly runtime encountered a non-recoverable problem.");
		}
		
		/// <summary>The engine name. Either 'Nitro' for JavaScript or 'WebAssembly'.</summary>
		public virtual string EngineName{
			get{
				throw new NotImplementedException();
			}
		}
		
		/// <summary>Sets up the given assembly.</summary>
		protected virtual void SetupScopes(Assembly asm){
		
		}
		
		/// <summary>Loads an already compiled asm by its unique ID.</summary>
		public Assembly LoadAssembly(string uniqueID){
			
			if(uniqueID==null){
				return null;
			}
			
			// Get the assembly:
			return CodeReference.GetAssembly(EngineName+"-"+uniqueID);
			
		}
		
		/// <summary>Loads an already compiled blob by its unique ID into this engine.</summary>
		public CompiledCode Load(string uniqueID){
			
			// Get the assembly:
			Assembly assembly=LoadAssembly(uniqueID);
			
			// Get the main method:
			return GetMain(assembly);
		}
		
		/// <summary>Gets the main method from the given assembly.</summary>
		protected CompiledCode GetMain(Assembly assembly){
			
			if(assembly==null){
				return null;
			}
			
			// Great - got the assembly. Grab the entry point:
			Type scriptType=assembly.GetType(EngineName+"_EntryPoint");
			
			if(scriptType==null){
				return null;
			}
			
			// Get the (static) entry method called __.main:
			MethodInfo main=scriptType.GetMethod("__.main");
			
			if(main==null){
				return null;
			}
			
			// Next, we'll setup the rest of the scopes:
			SetupScopes(assembly);
			
			// Ok!
			return new CompiledCode(main);
		}
		
	}
	
}