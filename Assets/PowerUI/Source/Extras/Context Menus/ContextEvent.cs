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
		
		
		/// <summary>Adds the given option to the list. Typically the markup would be a HTML variable.
		/// Note that you can also create your own option class (inherit from Option) and add that instead.</summary>
		public Option add(string markup,OptionEventMethod method){
			return list.add(markup,method);
		}
		
		/// <summary>Adds the given option to the list. 
		/// Note that you can also create your own option class (inherit from Option) and add that instead.</summary>
		public void add(Option option){
			list.add(option);
		}
		
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
		
		public override void handleEvent(Dom.Event e){
			Listener((ContextEvent)e);
		}
		
	}
	
}


namespace Dom{
	
	public partial class Element{
		
		/// <summary>Called when this element receives a contextmenu request.</summary>
		public ContextMenus.ContextEventDelegate oncontextmenu{
			get{
				return GetFirstDelegate<ContextMenus.ContextEventDelegate>("contextmenu");
			}
			set{
				addEventListener("contextmenu",new ContextMenus.ContextEventListener(value));
			}
		}
		
	}
	
}