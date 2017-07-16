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
		public static MethodBase AddEventListenerDisambiguation(MethodGroup group,FunctionCallExpression fce){
			
			// Get the first arg:
			string firstArg = fce.Arg(0) as string;
			
			if(firstArg != null){
				
				// We can potentially optimize this! Use the event map to find what
				// type of event firstArg is.
				var eInfo = EventMap.Get(firstArg);
				
				if(eInfo != null){
					// Match for that event:
					MethodBase mtd = group.ExactMatch(new Type[]{typeof(string), eInfo.ActionType});
					
					if(mtd!=null){
						return mtd;
					}
				}
				
			}
			
			// Generic Dom.Event form:
			return group.ExactMatch(new Type[]{typeof(string), typeof(Action<Dom.Event>)});
		}
		
	}
	
}