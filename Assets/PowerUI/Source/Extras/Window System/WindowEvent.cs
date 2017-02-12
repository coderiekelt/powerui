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
	
	public delegate void WindowEventDelegate(WindowEvent e);
	
	/// Handler for WindowEvent events.
	public class WindowEventListener : EventListener{
		
		public WindowEventDelegate Listener;
		
		public WindowEventListener(WindowEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(Dom.Event e){
			Listener((WindowEvent)e);
		}
		
	}
	
}