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
	/// A track. They're simply a list of slides. They start at either a slide (in some other track),
	/// an item (typically an NPC) or an 2D element (using the slides CSS property).
	/// </summary>
	
	public partial class Track : EventTarget{
		
		/// <summary>Use this and partial class extensions to add custom info loaded from JSON.</summary>
		public static event SlideEventDelegate OnLoad;
		
		/// <summary>Loads a track from the given track data.</summary>
		public static Track LoadFromJson(Timeline timeline,JSObject json){
			
			// First, is it an indexed array?
			// ["Hello!","I'm Dave"]      <- A basic dialogue track
			var trackData=json as Json.JSIndexedArray;
			Track track=null;
			
			if(trackData==null){
				
				// Must explicitly define which type it is:
				// { "type":"style", "track":TheTrackData }
				string type=json.String("type");
				
				if(string.IsNullOrEmpty(type)){
					
					Timeline.LoadFailed(
						"A track was an object but didn't contain a "+
						"'type' field to state what kind of track it is.",timeline
					);
					
				}
				
				// How about track data:
				var intenalTrackData=json["track"];
				
				if(intenalTrackData==null){
					// Nope!
					Timeline.LoadFailed("A track was an object but didn't contain a 'track' field.",timeline);
				}
				
				// Try and create a track of the specified type:
				type=type.Trim().ToLower();
				
				if(type=="style" || type=="css"){
					
					// Style track.
					track=new StyleTrack();
					
				}else if(type=="dialogue" || type=="speech" || type=="text"){
					
					// Dialogue track.
					track=new DialogueTrack();
					
				}else{
					
					Timeline.LoadFailed("Didn't recognise '"+type+"' as a track type.",timeline);
					
				}
				
				// Set it up:
				track.timeline=timeline;
				
				// Load the track data:
				track.load(intenalTrackData,trackData);
				
				return track;
				
			}else if(trackData.length==0){
				
				// This is an empty track of unknown type.
				// It's a safe situation so we just return null:
				return null;
				
			}
			
			// Detect what kind of track this is 
			// based on the properties of the first entry:
			var firstEntry=trackData[0];
			
			if(firstEntry==null){
				
				// Invalid track.
				Timeline.LoadFailed("invalid track",timeline);
				
			}
			
			// string, "markup", "speakers" => Dialogue track
			if(firstEntry is Json.JSValue || firstEntry["markup"]!=null || firstEntry["goto"]!=null || firstEntry["audio"]!=null || 
				firstEntry["speakers"]!=null || firstEntry["speaker"]!=null || firstEntry["options"]!=null){
				
				// If it contained goto, then we actually have a block of options in one slide.
				if(firstEntry["goto"]!=null){
					
					JSIndexedArray td=new JSIndexedArray();
					JSObject firstSlide=new JSObject();
					firstSlide["options"]=trackData;
					td.push(firstSlide);
					trackData=td;
					
				}
				
				// Dialogue track.
				track=new DialogueTrack();
				
			// "selector","style" => Style track
			}else if(firstEntry["selector"]!=null || firstEntry["style"]!=null || firstEntry["animation"]!=null){
				
				// Style track.
				track=new StyleTrack();
				
			}else if(firstEntry["wait-at"]!=null){
				
				// Cue track.
				track=new CueTrack();
				
			}else{
				
				// Fail otherwise:
				Timeline.LoadFailed("Unable to recognise this as a track.",timeline);
				
			}
			
			track.timeline=timeline;
			
			// Load it now:
			track.load(trackData,null);
			
			return track;
			
		}
		
		/// <summary>Whilst a track is progressing, this is the current slide index that it's at.</summary>
		public int currentSlide=-1;
		/// <summary>The timeline that this track originated from.</summary>
		public Timeline timeline;
		/// <summary>A globally unique ID.</summary>
		public int id;
		/// <summary>The slides which appear one after the other whilst this track is running.</summary>
		public Slide[] slides;
		/// <summary>The json the track originated from. NOTE: Only set if OnLoad is not null.</summary>
		public JSObject rawJson;
		
		public Track(){}
		
		internal virtual void OnStart(){}
		
		/// <summary>The defined duration.</summary>
		internal float definedDuration{
			get{
				float start=0f;
				
				for(int i=0;i<slides.Length;i++){
					
					// Get the slide:
					Slide slide=slides[i];
					
					if(slide.start==null){
						continue;
					}
					
					if(slide.start is Css.Units.PercentUnit){
						continue;
					}
					
					// Get the raw dec:
					float startDec=slide.start.GetRawDecimal();
					
					if(startDec>start){
						start=startDec;
					}
					
					// Get the duration:
					if(slide.duration==null || slide.duration is Css.Units.PercentUnit){
						continue;
					}
					
					// Get the end:
					startDec+=slide.duration.GetRawDecimal();
					
					if(startDec>start){
						start=startDec;
					}
					
				}
				
				return start;
			}
		}
		
		/// <summary>Updates the computedStart and computedDuration values.</summary>
		/// <param name="length">Total track length.</param>
		internal void SetStartAndDuration(float length){
			
			// Go through each slide and setup start/duration.
			if(slides==null){
				return;
			}
			
			int firstNoStart=-1;
			int ignoreSlides=0;
			
			for(int i=0;i<slides.Length;i++){
				
				// Get the slide:
				Slide slide=slides[i];
				
				if(slide.ignore){
					
					if(firstNoStart!=-1){
						ignoreSlides++;
					}
					
					continue;
				}
				
				// Compute start first:
				if(slide.start==null){
					
					if(firstNoStart==-1){
						firstNoStart=i;
					}
					
				}else{
					
					// Get the raw dec so we can resolve %:
					float startDec=slide.start.GetRawDecimal();
					
					if(slide.start is Css.Units.PercentUnit){
						startDec *= length;
					}
					
					slide.computedStart=startDec;
					
					if(firstNoStart!=-1){
						
						// Plug the gap from firstNoStart to i-1.
						PlugStartGaps(firstNoStart,i,ignoreSlides,startDec);
						
						// Clear:
						firstNoStart=-1;
						ignoreSlides=0;
						
					}
					
				}
				
			}
			
			if(firstNoStart!=-1){
				
				// Plug the gap from firstNoStart to length-1.
				PlugStartGaps(firstNoStart,slides.Length,ignoreSlides,length);
				
			}
			
			// Next, apply durations.
			for(int i=0;i<slides.Length;i++){
				
				// Get the slide:
				Slide slide=slides[i];
				
				if(slide.ignore){
					continue;
				}
				
				// Compute duration:
				if(slide.duration==null){
					
					// It's just next non-ignored Start - myStart:
					if(i==slides.Length-1){
						slide.computedDuration=length - slide.computedStart;
					}else{
						
						int nextIndex=i+1;
						Slide next=slides[nextIndex];
						
						while(next.ignore){
							
							nextIndex++;
							
							if(nextIndex==slides.Length){
								next=null;
								break;
							}
							
							next=slides[nextIndex];
							
						}
						
						if(next==null){
							slide.computedDuration=length - slide.computedStart;
						}else{
							slide.computedDuration=next.computedStart - slide.computedStart;
						}
						
					}
					
				}else{
					
					// Get the raw dec so we can resolve %:
					float durDec=slide.duration.GetRawDecimal();
					
					if(slide.duration is Css.Units.PercentUnit){
						durDec *= length;
					}
					
					slide.computedDuration=durDec;
					
				}
				
			}
			
			
		}
		
		/// <summary>Sets start between firstNoStart and max-1 using the given start value for max.</summary>
		private void PlugStartGaps(int firstNoStart,int max,int ignoreSlides,float maxStart){
			
			float gapStart=0f;
			
			if(firstNoStart!=0){
				// The end of the frame before FNS (the nearest non-ignored slide):
				int index=firstNoStart-1;
				Slide prev=null;
				
				while(index>=0){
					
					prev=slides[index];
					
					if(prev.ignore){
						index--;
						prev=null;
					}else{
						break;
					}
					
				}
				
				if(prev!=null){
					gapStart=prev.computedEnd;
				}
				
			}
			
			// The delta between start values:
			float slideDelta=( maxStart-gapStart ) / (float)(max-firstNoStart-ignoreSlides);
			
			// Plug the gap now:
			for(int x=firstNoStart;x<max;x++){
				
				// First one goes at gapStart.
				slides[x].computedStart=gapStart;
				gapStart+=slideDelta;
				
			}
			
		}
		
		/// <summary>The name of this type of track. "style" and "dialogue" are common examples.</summary>
		public virtual string tagName{
			get{
				return "";
			}
		}
		
		/// <summary>Creates an event relative to this track. Prepends "slides" to the given type.</summary>
		public SlideEvent createEvent(string type){
			return createRawEvent("slides"+type);
		}
		
		/// <summary>Creates an event relative to this track. Type is 'as-is'.</summary>
		public SlideEvent createRawEvent(string type){
			
			SlideEvent de=new SlideEvent(type,null);
			de.timeline=timeline;
			de.track=this;
			de.SetTrusted();
			return de;
			
		}
		
		/// <summary>True if this tracks data has been setup.</summary>
		public bool loaded{
			get{
				return (slides!=null);
			}
		}
		
		/// <summary>Creates a slide of the correct type for this track.</summary>
		public virtual Slide createSlide(){
			return new Slide();
		}
		
		/// <summary>Loads a track from some JSON data with an optional header.</summary>
		public virtual void load(JSObject json,JSObject header){
			
			// Got a header? Might contain an ID:
			if(header!=null){
				
				// ID:
				int.TryParse(json.String("id"),out id);
				
			}
			
			// Slides:
			JSArray slides=json as JSArray;
			
			if(slides==null){
				
				// Never null:
				this.slides=new Slide[0];
				return;
				
			}
			
			if(!slides.IsIndexed){
				throw new Exception("Incorrect PowerSlide track: 'slides' must be an indexed array.");
			}
			
			// Create array now:
			this.slides=new Slide[slides.length];
			
			// For each one..
			foreach(KeyValuePair<string,JSObject> kvp in slides.Values){
				
				// index is..
				int index;
				int.TryParse(kvp.Key,out index);
				
				// Create and setup the slide now:
				Slide c=createSlide();
				c.track=this;
				c.index=index;
				
				// Load the info:
				c.load(kvp.Value);
				
				// Apply:
				this.slides[index]=c;
				
			}
			
			if(OnLoad!=null){
				
				// Dispatch the load event which enables any custom info being added:
				rawJson=json;
				SlideEvent de=createEvent("load");
				OnLoad(de);
				
			}
			
		}
		
	}
	
}	