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
using Dom;
using System.Collections;
using System.Collections.Generic;


namespace Dialogue{
	
	/// <summary>
	/// Represents a dialogue event.
	/// </summary>
	
	public class DialogueEvent : DomEvent{
		
		/// <summary>The manager that this originated from. Can be null during e.g. load events when no manager is available.</summary>
		public Manager manager;
		/// <summary>The current train of thought.</summary>
		public Train train;
		/// <summary>The current cue card. Can be null if this is a train event.</summary>
		public Card card;
		/// <summary>The current action. Can be null if this is a train/card event.</summary>
		public Action action;
		/// <summary>An optional set of globals to pass to the target.</summary>
		public Dictionary<string,object> globals;
		
		/// <summary>A convenience approach for getting/setting globals to pass through during a cue event.</summary>
		public object this[string global]{
			get{
				if(globals==null){
					return null;
				}
				
				object result;
				globals.TryGetValue(global,out result);
				return result;
			}
			set{
				if(globals==null){
					globals=new Dictionary<string,object>();
				}
				
				globals[global]=value;
			}
		}
		
		/// <summary>The HTML document that this dialogue event originated from.</summary>
		public PowerUI.HtmlDocument htmlDocument{
			get{
				if(manager==null){
					return null;
				}
				
				return manager.document;
			}
		}
		
		/// <summary>A custom action ref.</summary>
		public string actionID{
			get{
				return action.ID;
			}
		}
		
		public DialogueEvent(string type,object init):base(type,init){}
		
		/// <summary>Opens a window with the given template and URL. Globals originate from this event.
		/// Convenience method for thisEvent.document.sparkWindows.open(template,url,thisEvent.globals);</summary>
		public void open(string template,string url){
			
			htmlDocument.sparkWindows.open(template,url,globals);
			
		}
		
	}
	
	public delegate void DialogueEventDelegate(DialogueEvent e);
	
	/// Handler for DialogueEvent events.
	public class DialogueEventListener : EventListener{
		
		public DialogueEventDelegate Listener;
		
		public DialogueEventListener(DialogueEventDelegate listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(DomEvent e){
			Listener((DialogueEvent)e);
		}
		
	}
	
}