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
using PowerUI;
using JavaScript;
using System.Reflection;
using JavaScript.Compiler;


namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		/// <summary>
		/// Helps the JS compiler optimise addEventListener calls.
		/// </summary>
		public MethodBase AddEventListenerDisambiguation(MethodGroup group,FunctionCallExpression fce){
			
			// Get the first arg:
			string firstArg = fce.Arg(0) as string;
			
			if(firstArg == null){
				// Generic Dom.Event form:
				return group.Match(new Type[]{typeof(string), typeof(Dom.Event)});
			}
			
			// We can potentially optimize this! Use the event map to find what
			// type of event firstArg is.
			var eInfo = EventMap.Get(firstArg);
			
			if(eInfo == null){
				// Generic Dom.Event form:
				return group.Match(new Type[]{typeof(string), typeof(Dom.Event)});
			}
			
			// Match for that event:
			return group.Match(new Type[]{typeof(string), eInfo.Type});
			
		}
		
	}
	
}