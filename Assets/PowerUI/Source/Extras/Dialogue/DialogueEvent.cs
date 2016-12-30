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
		
		/// <summary>A custom action ref.</summary>
		public string actionID{
			get{
				return action.ID;
			}
		}
		
		public DialogueEvent(string type,object init):base(type,init){}
		
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