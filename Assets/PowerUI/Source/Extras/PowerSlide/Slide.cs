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
		
		/// <summary>A global slide ID. Used to obtain a slide from a click event.</summary>
		private static int UniqueID_=1;
		
		/// <summary>Use this and partial class extensions to add custom info loaded from JSON.</summary>
		public static event Action<SlideEvent> OnLoad;
		
		/// <summary>
		/// A unique ID (locally). Used to obtain a slide from e.g. a click event.
		/// </summary>
		internal int uniqueID;
		/// <summary>
		/// True if this slide should be entirely ignored.</summary>
		public bool ignore;
		/// <summary>The json the slide originated from.</summary>
		public JSObject rawJson;
		/// <summary>
		/// The index of this slide in its track. Note that these aren't unique.
		/// </summary>
		public int index;
		/// <summary>
		/// The track this slide belongs to.
		/// </summary>
		public Track track;
		/// <summary>
		/// The timing leader for this slide (if it has one).
		/// </summary>
		public ITimingLeader timing;
		/// <summary>Specified start value.</summary>
		public Css.Value start;
		/// <summary>Specified duration value.</summary>
		public Css.Value duration;
		/// <summary>The computed start value.</summary>
		public float computedStart=0f;
		/// <summary>The computed duration.</summary>
		public float computedDuration=0f;
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
		
		public Slide(){
			uniqueID=UniqueID_++;
		}
		
		/// <summary>Gets a slide by a unique ID.</summary>
		public virtual Slide getSlideByID(int uniqueID){
			
			if(this.uniqueID==uniqueID){
				return this;
			}
			
			return null;
			
		}
		
		/// <summary>Ends this slides timing lead.</summary>
		public void EndTimingLead(){
			
			if(track.timeline.timingLeader==this){
				
				// Clear the leader:
				track.timeline.timingLeader=null;
				
			}
			
			if(timing!=null){
				timing.Stop();
				timing=null;
			}
			
		}
		
		/// <summary>The timeline will now have its timing lead by the given leader.</summary>
		public void TimingLeadBy(ITimingLeader leader){
		
			// Set the leader:
			timing=leader;
			
			// Apply this slide to the timeline:
			track.timeline.timingLeader=this;
			
		}
		
		/// <summary>This slide is now done.</summary>
		internal virtual void End(){
			
			// Dispatch a "slideend" event.
			SlideEvent se=createEvent("end");
			
			// Dispatch here:
			dispatchEvent(se);
			
			// Dispatch to the element too:
			EventTarget et=eventTarget;
			
			if(et!=null){
				et.dispatchEvent(se);
			}
			
			// Quit timing lead:
			EndTimingLead();
			
		}
		
		/// <summary>The event target to use.</summary>
		public EventTarget eventTarget{
			get{
				Element e=element;
				
				if(e==null){
					return timeline.document;
				}
				
				return e;
			}
		}
		
		/// <summary>The element which the timeline is running on.</summary>
		public Element element{
			get{
				if(timeline.Style==null){
					return null;
				}
				
				return timeline.Style.Element as Element;
			}
		}
		
		/// <summary>This slide is now starting.</summary>
		internal virtual void Start(){
			
			// Dispatch a "slidestart" event.
			SlideEvent se=createEvent("start");
			
			// Dispatch here:
			dispatchEvent(se);
			
			// Dispatch to the element too:
			EventTarget et=eventTarget;
			
			if(et!=null){
				et.dispatchEvent(se);
			}
			
		}
		
		/// <summary>The ID of the track that this slide belongs to.</summary>
		public int trackID{
			get{
				return track.id;
			}
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
		
		/// <summary>Ends this slide if it's done.</summary>
		internal void EndIfDone(bool backwards,float progress){
		
			float end=backwards ? (1f-computedStart) : computedEnd;
			
			if(end<=progress){
				
				// Remove from running:
				if(NextRunning==null){
					timeline.LastRunning=PreviousRunning;
				}else{
					NextRunning.PreviousRunning=PreviousRunning;
				}
				
				if(PreviousRunning==null){
					timeline.FirstRunning=NextRunning;
				}else{
					PreviousRunning.NextRunning=NextRunning;
				}
				
				// Done! This can "pause" to make it wait for a cue.
				End();
				
			}
			
		}
		
		/// <summary>The timeline that this slide is in.</summary>
		public Timeline timeline{
			get{
				return track.timeline;
			}
		}
		
		/// <summary>True if this slide has had start called but not end.
		/// I.e. it's actively running.</summary>
		public bool isActive{
			get{
				return timeline.IsActive(this);
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
			
			rawJson=json;
			
			if(OnLoad!=null){
				
				// Dispatch the load event which enables any custom info being added:
				SlideEvent de=createEvent("load");
				OnLoad(de);
				
			}
			
		}
		
		/// <summary>Reads from the JSON. Specify custom values with this.</summary>
		public JSObject this[string index]{
			get{
				if(rawJson==null){
					return null;
				}
				return rawJson[index];
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