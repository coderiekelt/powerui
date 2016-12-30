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


namespace ContextMenus{
	
	/// <summary>
	/// Represents a context event.
	/// Extend this (with a partial class) if you want to add custom things 
	/// to pass through to the window which will actually handle the display.
	/// </summary>
	
	public partial class ContextEvent : UIEvent{
		
		/// <summary>The document that will host the context window.</summary>
		private Document contextDocument_;
		/// <summary>The document that will host the context window. Defaults to the main UI.
		/// You can e.g. replace this with a WorldUI if you want.</summary>
		public Document contextDocument{
			get{
				if(contextDocument_==null){
					return PowerUI.UI.document;
				}
				
				return contextDocument_;
			}
			set{
				contextDocument_=value;
			}
		}
		/// <summary>The list which represents the group of options.</summary>
		public OptionList list;
		/// <summary>The name of the window which will display the context options. Set this during the oncontextmenu event.</summary>
		public string window="menulist";
		
		
		public ContextEvent(string type,object init):base(type,init){
		}
		
	}
	
	public delegate void ContextEventDelegate(ContextEvent e);
	
	/// Handler for ContextEvent events.
	public class ContextEventListener : EventListener{
		
		public ContextEventDelegate Listener;
		
		public ContextEventListener(ContextEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((ContextEvent)e);
		}
		
	}
	
}