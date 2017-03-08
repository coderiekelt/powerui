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

using Dom;
using System;
using UnityEngine;
using PowerUI;


namespace Windows{
	
	/// <summary>
	/// Represents a context event.
	/// Extend this (with a partial class) if you want to add custom things 
	/// to pass through to the window which will actually handle the display.
	/// </summary>
	
	public partial class WindowEvent : UIEvent{
		
		/// <summary>The origin window.</summary>
		public Window window;
		
		
		public WindowEvent(string type,object init):base(type,init){
		}
		
	}
	
}

namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		public void addEventListener(string name,Action<Windows.WindowEvent> method){
			addEventListener(name,new EventListener<Windows.WindowEvent>(method));
		}
		
	}
	
}