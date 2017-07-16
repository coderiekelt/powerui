//--------------------------------------
//         Nitro Script Engine
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace JavaScript{
	
	/// <summary>
	/// A security domain which defines what is accessible from JavaScript.
	/// </summary>
	
	public class SecurityDomain{
		
		/// <summary>The default security manager if none is provided.</summary>
		private static SecurityDomain Default=new SecurityDomain();
		
		/// <summary>Gets the default security manager.</summary>
		public static SecurityDomain GetDefaultManager(){
			return Default;
		}
		
		/// <summary>True if this domain allows everything.</summary>
		private bool AllowAll;
		/// <summary>A set of all blocked types by name.</summary>
		private List<string> BlockedNames;
		/// <summary>A set of classes included and allowed by default.</summary>
		private List<CodeReference> DefaultReferences;
		/// <summary>A set of all allowed types by name.</summary>
		private List<string> AllowedNames=new List<string>();
		
		
		/// <summary>Creates a default security manager which allows nothing by default.</summary>
		public SecurityDomain(){
		}
		
		/// <summary>Clears all default references.</summary>
		public void ClearDefaultReferences(){
			DefaultReferences=null;
		}
		
		/// <summary>Gets the default references.</summary>
		/// <returns>The list of default code references.</returns>
		public List<CodeReference> GetDefaultReferences(){
			if(DefaultReferences==null){
				return null;
			}
			List<CodeReference> results=new List<CodeReference>(DefaultReferences.Count);
			foreach(CodeReference reference in DefaultReferences){
				results.Add(reference);
			}
			return results;
		}
		
		/// <summary>Adds the given text as a reference. Must also include the assembly unless it is in 'this' one.</summary>
		/// <param name="text">The reference to add, e.g. "System.System".</param>
		protected void AddReference(string text){
			if(DefaultReferences==null){
				DefaultReferences=new List<CodeReference>();
			}
			CodeReference codeRef=new CodeReference(text);
			DefaultReferences.Add(codeRef);
		}
		
		/// <summary>Searches for an allowed type by its name.</summary>
		public Type Find(string name){
			
			Type type=FindType(name);
			
			if(type==null || !IsAllowed(type)){
				return null;
			}
			
			return type;
			
		}
		
		/// <summary>Searches for the system type with the given name.</summary>
		/// <param name="name">The type name to search for.</param>
		/// <returns>A system type if found; null otherwise.</returns>
		private Type FindType(string name){
			
			// Check if we can straight get the type by this name.
			// Go hunting - can we find typeName anywhere?
			Type type=Type.GetType(name,false,true);
			
			if(type!=null){
				// That was easy!
				return type;
			}
			
			// Start looking around in our Referenced namespaces to find this type.
			// This is done because e.g. Socket is named System.Net.Socket in the System.Net namespace.
			// As the reference knows which assembly to look in, these are quick to handle.
			if(DefaultReferences==null){
				return null;
			}
			
			foreach(CodeReference reference in DefaultReferences){
				type=reference.GetType(name);
				
				if(type!=null){
					return type;
				}
				
			}
			
			// It could also be in an assembly on it's own without a namespace.
			// Lets look for that next.
			// Make sure all available assemblies are setup for use:
			CodeReference.Setup();
			
			// And check in each one:
			foreach(KeyValuePair<string,CodeAssembly> assembly in CodeReference.Assemblies){
				
				if(assembly.Value.Current || assembly.Value.NitroAOT){
					// This was the first thing we checked, or is a Nitro assembly - skip.
					continue;
				}
				
				type=assembly.Value.GetType(name);
				
				if(type!=null){
					return type;
				}
				
			}
			
			return null;
		}
		
		/// <summary>Checks if the named type is allowed by this domain.</summary>
		/// <param name="name">The type to check for.</param>
		/// <returns>True if the type is allowed by this domain.</returns>
		public bool IsAllowed(Type type){
			
			// Check that its not blocked:
			if(Blocked(type)){
				return false;
			}
			
			if(AllowAll){
				return true;
			}
			
			if(type==null || AllowedNames==null){
				return false;
			}
			
			string typeName = type.FullName;
			
			foreach(string allowed in AllowedNames){
				if(allowed==typeName || typeName.StartsWith(allowed+".")){
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Call this to make this domain allow any type except for those in its block list.</summary>
		protected void AllowEverything(){
			AllowAll=true;
		}
		
		/// <summary>Lets this domain allow the given type/ namespace.
		/// You must derive this class and call this from within the constructor.</summary>
		/// <param name="name">The type/ namespace to allow.</param>
		protected void Allow(string name){
			if(AllowedNames==null){
				AllowedNames=new List<string>();
			}
			AllowedNames.Add(name);
		}
		
		/// <summary>Lets this domain block the given type/ namespace. Used when this domain allows everything except a certain few.
		/// You must derive this class and call this from within the constructor.</summary>
		/// <param name="name">The name of the type/ namespace to block.</param>
		protected void Block(string name){
			if(BlockedNames==null){
				BlockedNames=new List<string>();
			}
			BlockedNames.Add(name);
		}
		
		/// <summary>Checks if the given type is blocked by this domain.</summary>
		/// <param name="type">The type to check for.</param>
		/// <returns>True if this domain blocks the named type.</returns>
		private bool Blocked(Type type){
			if(type==null || BlockedNames==null){
				return false;
			}
			
			string typeName = type.FullName;
			
			foreach(string blocked in BlockedNames){
				if(blocked==typeName || typeName.StartsWith(blocked+".")){
					return true;
				}
			}
			return false;
		}
		
		/// <summary>Checks if the given remote location is allowed any access at all.
		/// Note that this can internally update the domain itself if it wants to apply further restrictions.</summary>
		/// <param name="protocol">The protocol:// of the remote location. Lowercase and usually e.g. http(s).</param>
		/// <param name="host">The first part of the location after the protocol. E.g. www.kulestar.com.</param>
		/// <param name="fullPath">The whole path requesting access. E.g. http://www.kulestar.com/.</param>
		public virtual bool AllowAccess(string protocol,string host,string fullPath){
			return false;
		}
		
	}
	
}