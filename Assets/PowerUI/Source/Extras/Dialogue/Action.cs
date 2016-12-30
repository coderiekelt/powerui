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
using PowerUI;
using Json;
using System.Reflection;
using Nitro;


namespace Dialogue{
	
	/// <summary>
	/// A raw dialogue action. E.g. "start cutscene x" or "open door y". These are mapped as event listeners.
	/// </summary>
	
	public class Action{
		
		/// <summary>The card this is an action for.</summary>
		public Card Card;
		/// <summary>A custom action ID.</summary>
		public string ID;
		/// <summary>The method to run. Must accept a DialogueEvent.</summary>
		public MethodInfo Method;
		
		
		public Action(Card card){
			Card=card;
		}
		
		/// <summary>Loads the action meta from the given JSON data.</summary>
		public Action(Card card,JSObject data){
			Card=card;
			Load(data);
		}
		
		/// <summary>Loads the action meta from the given JSON data.</summary>
		public void Load(JSObject data){
			
			// Method and an ID:
			string methodName=data.String("method");
			ID=data.String("id");
			
			int index=methodName.LastIndexOf('.');
			
			if(index==-1){
				throw new Exception("Class name and method required in dialogue action methods (\"method\":\"Class.Method\").");
			}
			
			// Grab the class name:
			string className=methodName.Substring(0,index);
			
			// Get the type:
			Type type=CodeReference.GetFirstType(className);
			
			if(type==null){
				throw new Exception("Dialogue action method type not found: "+className);
			}
			
			// Update the method name:
			methodName=methodName.Substring(index+1);
			
			// Grab the method info:
			
			#if NETFX_CORE
			Method=type.GetTypeInfo().GetDeclaredMethod(methodName);
			#else
			Method=type.GetMethod(methodName);
			#endif
			
			if(Method==null){
				throw new Exception("Dialogue action method not found in '"+type.ToString()+"': "+methodName);
			}
			
		}
		
	}
	
}