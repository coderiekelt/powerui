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
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// A map of event names to their event object type.
	/// The data here is loaded from a file called All-Events.txt.
	/// It's primarily used for the 'All Available Events' editor window
	/// as well as for disambiguating addEventListener when used by JavaScript.
	/// </summary>
	public static class EventMap{
		
		/// <summary>All available events. Use GetAll instead.</summary>
		private static Dictionary<string,EventInfo> All;
		
		
		/// <summary>Gets an event by lowercase name (excluding 'on' prefix).</summary>
		public static EventInfo Get(string name){
			
			if(name==null){
				return null;
			}
			
			if(All==null){
				GetAll();
			}
			
			name=name.Trim().ToLower();
			EventInfo result;
			All.TryGetValue(name,out result);
			return result;
		}
		
		/// <summary>Gets all available events.</summary>
		public static Dictionary<string,EventInfo> GetAll(){
			
			if(All!=null){
				return All;
			}
			
			// Create:
			All=new Dictionary<string,EventInfo>();
			
			// Load up now:
			UnityEngine.TextAsset ncData=(UnityEngine.Resources.Load("All-Events") as UnityEngine.TextAsset);
			
			if(ncData==null){
				return All;
			}
			
			byte[] file=ncData.bytes;
			
			// Create a reader:
			BinaryIO.Reader reader=new BinaryIO.Reader(file);
			
			// Starts with 'EVT' then a nul byte (so e.g. PowerTools can identify it):
			if(reader.ReadByte()!='E' || reader.ReadByte()!='V' || 
				reader.ReadByte()!='T' || reader.ReadByte()!='\0'){
				throw new Exception("Invalid event map file.");
			}
			
			// Get all assemblies:
			Assembly[] asm = Nitro.Assemblies.GetAll();
			
			// First, the event type map:
			int eventNames = (int)reader.ReadCompressed();
			
			// List of types:
			Type[] evtTypes = new Type[eventNames];
			
			for(int i=0;i<eventNames;i++){
				
				// Read the type name (e.g. Dom.Event):
				string name=reader.ReadString();
				
				// Try finding it (just null/ silent error if it wasn't found):
				evtTypes[i]=GetType(name,asm);
				
			}
			
			while(reader.More()){
				
				// Read flags byte (just indicates if this is a std event at the moment):
				reader.ReadByte();
				
				// Read the line:
				string name=reader.ReadString();
				int typeID=(int)reader.ReadCompressed();
				
				// Get the type:
				Type type=evtTypes[typeID];
				
				// If it wasn't found, this event isn't available (silent error).
				if(type!=null){
					All[name.ToLower()]=new EventInfo(name,type);
				}
				
			}
			
			
			return All;
		}
		
		/// <summary>Gets the type by the given name (including namespace).</summary>
		private static Type GetType(string typeName,Assembly[] asm){
			
			for(int i=0;i<asm.Length;i++){
                Type type = asm[i].GetType(typeName);
                if (type != null){
                    return type;
				}
            }
			
            return null;
        }
		
	}
	
	/// <summary>Information about an event.</summary>
	public class EventInfo{
		
		/// <summary>The name of the event.</summary>
		public string Name;
		/// <summary>The argument type. E.g. UIEvent.</summary>
		public Type Type;
		
		
		public EventInfo(string name,Type type){
			Name=name;
			Type=type;
		}
		
	}
	
}