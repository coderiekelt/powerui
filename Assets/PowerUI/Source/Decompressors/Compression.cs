//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Nitro;
using Dom;


namespace PowerUI.Compression{
	
	/// <summary>
	/// Various compression algorithms available for direct use. You can also delete the ones you don't want.
	/// Keep in mind that zlib is used by WOFF and brotli is used by WOFF2.
	/// Try Compression.Get("zlib").Decompress(aStream,aBlockOfBytes);
	/// </summary>
	
	public static class Compression{
		
		/// <summary>All available compression types. e.g. 'zlib' or 'brotli' are available by default.</summary>
		public static Dictionary<string,Compressor> All;
		
		#if NETFX_CORE
		
		/// <summary>Sets up the global lookup by searching for any classes which inherit from Compressor.</summary>
		public static void Setup(){
			if(All!=null){
				return;
			}
			
			// Create the set:
			All=new Dictionary<string,Compressor>();
			
			// For each type..
			foreach(TypeInfo type in typeof(Compression).GetTypeInfo().Assembly.DefinedTypes){
				
				if(type.IsGenericType){
					continue;
				}
				
				// Is it a CSS function?
				if(type.IsSubclassOf(typeof(Compressor))){
					// Yes - add it:
					Add((Compressor)Activator.CreateInstance(type.AsType()));
				}
				
			}
			
		}
		
		#else
		
		/// <summary>Sets up the global lookup by searching for any classes which inherit from Compressor.</summary>
		public static void Setup(Type[] allTypes){
			if(All!=null){
				return;
			}
			
			// Create the set:
			All=new Dictionary<string,Compressor>();
			
			// For each type..
			for(int i=allTypes.Length-1;i>=0;i--){
				// Grab it:
				Type type=allTypes[i];
				
				if(type.IsGenericType){
					continue;
				}
				
				// Is it a CSS at rule?
				if(TypeData.IsSubclassOf(type,typeof(Compressor))){
					// Yes - add it:
					Add((Compressor)Activator.CreateInstance(type));
				}
				
			}
			
		}
		
		#endif
		
		/// <summary>Adds a CSS at rule to the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="cssRule">The at rule to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(Compressor cssRule){
			
			string[] names=cssRule.GetNames();
			
			if(names==null||names.Length==0){
				return false;
			}
			
			for(int i=0;i<names.Length;i++){
				
				// Grab the name:
				string name=names[i];
				
				if(name==null){
					continue;
				}
				
				// Add it to functions:
				All[name.ToLower()]=cssRule;
				
			}
			
			return true;
		}
		
		/// <summary>Attempts to find the named at rule, returning the global instance if it's found.</summary>
		/// <param name="name">The rule to look for.</param>
		/// <returns>The global Compressor if the rule was found; Null otherwise.</returns>
		public static Compressor Get(string name){
			Compressor globalFunction=null;
			All.TryGetValue(name,out globalFunction);
			return globalFunction;
		}
		
	}
	
}