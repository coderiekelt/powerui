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


namespace PowerSlide{
	
	/// <summary>
	/// A slide. Contains e.g. the text spoken or the style to apply.
	/// A 'track' is a list of these slides.
	/// </summary>
	
	public partial class Slide : EventTarget{
		
		/// <summary>Use this and partial class extensions to add custom info loaded from JSON.</summary>
		public static event SlideEventDelegate OnLoad;
		
		/// <summary>
		/// Globally unique slide ID.
		/// </summary>
		public int id;
		/// <summary>The json the slide originated from.
		/// NOTE: Only set if OnLoad is not null.</summary>
		public JSObject rawJson;
		/// <summary>
		/// The index of this slide in its track. Note that these aren't unique.
		/// </summary>
		public int index;
		/// <summary>
		/// The track this slide belongs to.
		/// </summary>
		public Track track;
		/// <summary>Specified start value.</summary>
		public Css.Value start;
		/// <summary>Specified duration value.</summary>
		public Css.Value duration;
		/// <summary>The computed start value.</summary>
		public float computedStart=float.MinValue;
		/// <summary>The computed duration.</summary>
		public float computedDuration=float.MinValue;
		/// <summary>
		/// Actions to trigger.
		/// </summary>
		public Action[] Actions;
		/// <summary>The linked list of running slides.</summary>
		internal Slide NextRunning;
		/// <summary>The linked list of running slides.</summary>
		internal Slide PreviousRunning;
		
		
		/// <summary>The computed end value.</summary>
		public float computedEnd{
			get{
				return (computedStart + computedDuration);
			}
		}
		
		/// <summary>This slide is now done.</summary>
		internal virtual void End(){
			
			// Dispatch a "slideend" event.
			SlideEvent se=createEvent("end");
			
			// Dispatch here:
			dispatchEvent(se);
			
			// Dispatch to the element too:
			element.dispatchEvent(se);
			
		}
		
		/// <summary>The element which the timeline is running on.</summary>
		public Element element{
			get{
				return track.timeline.Style.Element as Element;
			}
		}
		
		/// <summary>This slide is now starting.</summary>
		internal virtual void Start(){
			
			// Dispatch a "slidestart" event.
			SlideEvent se=createEvent("start");
			
			// Dispatch here:
			dispatchEvent(se);
			
			// Dispatch to the element too:
			element.dispatchEvent(se);
			
		}
		
		/// <summary>The ID of the track that this slide belongs to.</summary>
		public int trackID{
			get{
				return track.id;
			}
		}
		
		/// <summary>Continues to the next slide in the track.</summary>
		public void next(){
			
			// Go to the next slide:
			track.go(index+1);
			
		}
		
		/// <summary>Cues this slide right now.</summary>
		public void cue(){
			
			// Create the cue event:
			SlideEvent cue=createEvent("cue");
			
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
		
		/// <summary>Called when the timeline is paused/ resumed and this slide is running.</summary>
		internal virtual void SetPause(bool paused){}
		
		/// <summary>Loads a slide from the given JSON.</summary>
		public virtual void load(JSObject json){
			
			// Start:
			string startText=json.String("start");
			
			if(startText!=null){
				// Load the start value:
				start=Css.Value.Load(startText);
			}
			
			// Duration:
			string durationText=json.String("duration");
			
			if(durationText!=null){
				// Load the start value:
				duration=Css.Value.Load(durationText);
			}
			
			// Global ID (optional):
			int.TryParse(json.String("id"),out id);
			
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
			
			if(OnLoad!=null){
				
				// Dispatch the load event which enables any custom info being added:
				rawJson=json;
				SlideEvent de=createEvent("load");
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
		
		/// <summary>Creates an event relative to this slide.</summary>
		public SlideEvent createEvent(string type){
			
			SlideEvent de=track.createRawEvent("slide"+type);
			de.slide=this;
			return de;
			
		}
		
	}
	
}