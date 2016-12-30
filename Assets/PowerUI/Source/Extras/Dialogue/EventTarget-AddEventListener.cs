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
using Dialogue;


namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		public void addEventListener(string name,DialogueEventDelegate method){
			addEventListener(name,new DialogueEventListener(method));
		}
		
	}
	
}