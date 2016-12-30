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
	/// A cue card. Contains e.g. the text spoken.
	/// A 'train of thought' is a list of these cards.
	/// </summary>
	
	public partial class Card : EventTarget{
		
		/// <summary>Use this and partial class extensions to add custom info loaded from JSON.</summary>
		public static event DialogueEventDelegate OnLoad;
		
		/// <summary>
		/// Globally unique dialogue card ID.
		/// </summary>
		public int id;
		/// <summary>The json the card originated from. NOTE: Only set if OnLoad is not null.</summary>
		public JSObject Json;
		/// <summary>
		/// The index of this card in its host thought train. Note that these aren't unique.
		/// </summary>
		public int index;
		/// <summary>
		/// One or more speakers who say this. Note that a single 'speaker' object can
		/// be multiple actual speakers. For example, it might be referring to a 'clan member' object.
		/// In the scene, there could be 5 clan members.
		/// </summary>
		public Speaker[] Speakers;
		/// <summary>
		/// The dialogue train this card belongs to.
		/// </summary>
		public Train train;
		/// <summary>
		/// When this card is displayed as an option, the markup to show.
		/// </summary>
		public string option;
		/// <summary>
		/// The text to display when this card is cued. Note that this is actually speech markup (which also supports HTML).
		/// </summary>
		public string markup;
		/// <summary>
		/// Alternative to markup, actions to trigger.
		/// </summary>
		public Action[] Actions;
		
		/// <summary>The ID of the thought train that this card belongs to.</summary>
		public int trainID{
			get{
				return train.id;
			}
		}
		
		public Card(Train train,int index){
			this.train=train;
			this.index=index;
		}
		
		/// <summary>Continues to the next card in the train of thought.</summary>
		public void next(){
			
			// Go to the next card:
			train.go(index+1);
			
		}
		
		/// <summary>Cues this card right now.</summary>
		public void cue(){
			
			// Create the cue event:
			DialogueEvent cue=createEvent("cue");
			
			// Run it now:
			if(dispatchEvent(cue)){
				
				// Cue the text for each speaker.
				
				
				// Cue all actions (always after text):
				if(Actions!=null){
					
					// Param set:
					object[] arr=new object[]{cue};
					
					// Cue! Run each action now:
					for(int i=0;i<Actions.Length;i++){
						
						// Get it:
						Action a=Actions[i];
						
						// Update event:
						cue.action=a;
						
						// Invoke it:
						a.Method.Invoke(null,arr);
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Loads a cue card from the given JSON.</summary>
		public void load(JSObject json){
			
			// ID:
			int.TryParse(json.String("id"),out id);
			
			// Option:
			option=json.String("option");
			
			// Text:
			markup=json.String("markup");
			
			// Action:
			JSArray actions=json["actions"] as JSArray;
			
			if(actions!=null){
				
				if(actions.IsIndexed){
					
					// Multiple actions:
					
					// For each one..
					foreach(KeyValuePair<string,JSObject> kvp in actions.Values){
						
						// index is..
						int index;
						int.TryParse(kvp.Key,out index);
						
						// Set it up:
						LoadAction(index,kvp.Value);
						
					}
					
				}else{
					
					// Should be an array but we'll also accept just one.
					LoadAction(0,actions);
					
				}
				
			}else{
				
				// Check if they mis-named as just 'action':
				actions=json["action"] as JSArray;
				
				if(actions!=null){
					
					LoadAction(0,actions);
					
				}
				
			}
			
			// Speakers:
			JSArray speakers=json["speakers"] as JSArray;
			
			if(speakers!=null){
				
				if(speakers.IsIndexed){
					
					// Multiple speakers:
					
					// For each one..
					foreach(KeyValuePair<string,JSObject> kvp in speakers.Values){
						
						// index is..
						int index;
						int.TryParse(kvp.Key,out index);
						
						// Set it up:
						LoadSpeaker(index,kvp.Value);
						
					}
					
				}else{
					
					// Should be an array but we'll also accept just one.
					LoadSpeaker(0,speakers);
					
				}
				
			}else{
				
				// Check if they mis-named as just 'speaker':
				speakers=json["speaker"] as JSArray;
				
				if(speakers!=null){
					
					LoadSpeaker(0,speakers);
					
				}
				
			}
			
			if(OnLoad!=null){
				
				// Dispatch the load event which enables any custom info being added:
				Json=json;
				DialogueEvent de=createEvent("load");
				OnLoad(de);
				
			}
			
		}
		
		/// <summary>Sets up an action at the given index in the Actions set.</summary>
		internal void LoadAction(int index,JSObject data){
			
			if(Actions==null){
				Actions=new Action[1];
			}
			
			// Create and add:
			Action a=new Action(this,data);
			Actions[index]=a;
			
		}
		
		/// <summary>Sets up a speaker at the given index in the Speakers set.</summary>
		internal void LoadSpeaker(int index,JSObject data){
			
			if(Speakers==null){
				Speakers=new Speaker[1];
			}
			
			// Create and add:
			Speakers[index]=new Speaker(this,data);
			
		}
		
		/// <summary>Creates an event relative to this card.</summary>
		public DialogueEvent createEvent(string type){
			
			DialogueEvent de=train.createEvent(type);
			de.card=this;
			return de;
			
		}
		
	}
	
}