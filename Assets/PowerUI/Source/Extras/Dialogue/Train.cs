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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUI;
using Dom;
using Json;


namespace Dialogue{
	
	/// <summary>
	/// A train of thought. They're simply a list of cue cards. They start at either a cue card (in some other train of thought)
	/// or at some item (typically an NPC).
	/// </summary>
	
	public partial class Train : EventTarget{
		
		/// <summary>Use this and partial class extensions to add custom info loaded from JSON.</summary>
		public static event DialogueEventDelegate OnLoad;
		
		/// <summary>A globally unique ID.</summary>
		public int id;
		/// <summary>The cue cards which appear one after the other whilst this train is running.</summary>
		public Card[] cards;
		/// <summary>The json the train originated from. NOTE: Only set if OnLoad is not null.</summary>
		public JSObject Json;
		
		
		public Train(){}
		
		/// <summary>Loads a train from some JSON data.</summary>
		public Train(JSObject json){
			load(json);
		}
		
		/// <summary>Creates an event relative to this train.</summary>
		public DialogueEvent createEvent(string type){
			
			DialogueEvent de=new DialogueEvent(type,null);
			de.train=this;
			de.SetTrusted();
			return de;
			
		}
		
		/// <summary>True if this trains data has been setup.</summary>
		public bool loaded{
			get{
				return (cards!=null);
			}
		}
		
		/// <summary>Advances to the card at the given index. Typically originates from card.Next.</summary>
		public void go(int index){
			
			/*
			
			manager.currentTrain=this;
			
			if(Cards==null || index<0 || index>=Cards.Length){
				
				// End
				
			}else{
				
				// Get the card:
				Card card=Cards[index];
				
				// Set as current:
				manager.currentCard=card;
				
			}
			*/
			
		}
		
		/// <summary>Loads a train from some JSON data.</summary>
		public void load(JSObject json){
			
			// ID:
			int.TryParse(json.String("id"),out id);
			
			// Cards:
			JSArray cards=json["cards"] as JSArray;
			
			if(cards==null){
				
				// Never null:
				this.cards=new Card[0];
				return;
				
			}
			
			if(!cards.IsIndexed){
				throw new Exception("Incorrect train of thought: 'cards' must be an indexed array.");
			}
			
			// Create array now:
			this.cards=new Card[cards.Values.Count];
			
			// For each one..
			foreach(KeyValuePair<string,JSObject> kvp in cards.Values){
				
				// index is..
				int index;
				int.TryParse(kvp.Key,out index);
				
				// Create and setup the card now:
				Card c=new Card(this,index);
				
				// Load the info:
				c.load(kvp.Value);
				
				// Apply:
				this.cards[index]=c;
				
			}
			
			if(OnLoad!=null){
				
				// Dispatch the load event which enables any custom info being added:
				Json=json;
				DialogueEvent de=createEvent("load");
				OnLoad(de);
				
			}
			
		}
		
		/// <summary>Can this train of thought show up as an option? Yes by default.
		/// Add a handler for the 'allow' event and use preventDefault to block.
		/// Used when e.g. the player doesn't have the skill for something.</summary>
		public bool isAllowed{
			get{
				
				// Create the event:
				DialogueEvent e=createEvent("allow");
				
				// Run it:
				return dispatchEvent(e);
				
			}
		}
		
	}
	
}	