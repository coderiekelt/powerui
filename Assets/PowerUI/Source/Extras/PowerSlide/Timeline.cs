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
using PowerUI;
using Json;
using System.Collections;
using System.Collections.Generic;
using Css;


namespace PowerSlide{
	
	/// <summary>
	/// A loaded slides file. They contain one or more tracks (usually just one).
	/// A track (e.g a style track or a dialogue track) is a series of slides.
	/// </summary>
	
	public partial class Timeline{
		
		/// <summary>True if this is a dialogue timeline.</summary>
		internal bool isDialogue=false;
		/// <summary>The default language for this timeline.</summary>
		public string defaultLanguage="en";
		/// <summary>An optional declared duration. Overriden by the slides-duration value.
		/// If not declared at all then it defaults to 1.</summary>
		public Css.Value duration;
		/// <summary>All the tracks in this timeline.</summary>
		public Track[] tracks;
		/// <summary>Source URL.</summary>
		public string src;
		/// <summary>Default template to use.</summary>
		public string template;
		/// <summary>A time leading slide (usually one with audio/ video) 
		/// which this timeline will follow for timing purposes.</summary>
		public Slide timingLeader;
		/// <summary>The ComputedStyle that this was applied to (can be null).</summary>
		public ComputedStyle Style;
		/// <summary>A list of event listeners that *must* be destroyed 
		/// when either this timeline is killed or is un-paused.</summary>
		internal List<CueElementData> CueElements;
		/// <summary>If this timeline is using a widget, the widget itself.
		/// Used by dialogue.</summary>
		public Widgets.Widget currentWidget;
		/// <summary>Host HTML document.</summary>
		public HtmlDocument document;
		
		
		public Timeline(ComputedStyle style){
			Style=style;
			
			if(style!=null){
				document=style.document as HtmlDocument;
			}
			
		}
		
		/// <summary>The node that this timeline is on.</summary>
		public Dom.Node node{
			get{
				if(Style!=null){
					return Style.Element;
				}
				
				return document;
			}
		}
		
		/// <summary>Gets a slide by its unique ID.</summary>
		public Slide getSlide(int uniqueID){
			
			// For each track..
			for(int i=0;i<tracks.Length;i++){
			
				Track track=tracks[i];
				
				// For each slide..
				for(int s=0;s<track.slides.Length;s++){
					
					// Get it:
					Slide slide=track.slides[s];
					
					// Get by unique ID:
					Slide res=slide.getSlideByID(uniqueID);
					
					if(res!=null){
						// Got it!
						return res;
					}
					
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Opens a widget, passing this timeline as a global.</summary>
		internal Widgets.Widget OpenWidget(string template){
			
			if(document==null || template==null){
				Dom.Log.Add("PowerSlide requested to open a widget without a document/ template. Request was ignored.");
				return null;
			}
			
			if(currentWidget!=null){
				
				if(currentWidget.Type==template){
					// Unchanged template.
					return currentWidget;
				}
				
				currentWidget.close();
				currentWidget=null;
			}
			
			// Open it now:
			currentWidget=document.widgets.open(template,null,"timeline",this);
			
			return currentWidget;
		}
		
		
		/// <summary>
		/// Tidies up any event handlers added by cue nodes.
		/// </summary>
		public void ClearCues(){
			
			if(CueElements==null){
				return;
			}
			
			foreach(CueElementData ced in CueElements){
				
				Dom.Element node=ced.target;
				
				// Remove it:
				node.removeEventListener(ced.eventName,ced.eventListener);
				
			}
			
			// Ok!
			CueElements=null;
			
		}
		
		/// <summary>Loads a timeline from the given JSON.</summary>
		public void load(Json.JSObject json){
			
			// First, check for the 'tracks' field:
			Json.JSObject trackData=json["tracks"];
			
			if(trackData==null){
				
				// Considered to be a single track.
				// Try loading it now:
				Track track=Track.LoadFromJson(this,json);
				
				if(track==null){
					
					// Empty:
					tracks=new Track[0];
					
				}else{
					// We have at least 1 entry in a track.
					tracks=new Track[1];
					
					// We now need to detect what kind of track it is based on what it provides.
					// If it's a style track, the first element 
					tracks[0]=track;
				}
				
			}else{
				
				// Optional default language (must be before tracks load):
				string lang=json.String("lang");
				
				if(!string.IsNullOrEmpty(lang)){
					defaultLanguage=lang.Trim().ToLower();
				}
				
				// Must be an indexed array:
				var trackArray=trackData as Json.JSIndexedArray;
				
				if(trackArray==null){
					LoadFailed("'tracks' must be an indexed array.",this);
				}
				
				// Fully defined timeline.
				int length=trackArray.length;
				
				// Create track set:
				tracks=new Track[length];
				
				// Load each one:
				for(int i=0;i<length;i++){
					
					// Load the track now:
					tracks[i]=Track.LoadFromJson(this,trackArray[i]);
					
				}
				
				// Optional duration:
				string durationText=json.String("duration");
				
				if(!string.IsNullOrEmpty(durationText)){
					
					// Load it as a CSS value:
					duration=Css.Value.Load(durationText);
					
				}
				
			}
		}
		
		/// <summary>Called when a timeline fails to load.</summary>
		internal static void LoadFailed(string message,Timeline timeline){	
			
			string src=null;
			
			if(timeline!=null){
				src=timeline.src;
			}
			
			message="PowerSlide timeline ";
			
			if(src!=null){
				message+="(at '"+src+"') ";
			}
			
			message+="failed to load: "+message;
			
			throw new Exception(message);
		}
		
		/// <summary>The maximum defined duration.</summary>
		public float maxDefinedDuration{
			get{
				
				float max=0f;
				
				for(int i=0;i<tracks.Length;i++){
					
					// Get the duration:
					float dur=tracks[i].definedDuration;
					
					if(dur>max){
						max=dur;
					}
					
				}
				
				return max;
			}
		}
		
		/// <summary>Gets the first track by the name of the track. "style" or "dialogue" for example.</summary>
		public Track getTrackByTagName(string name){
			
			// Ensure it's lowercase:
			name=name.ToLower();
			
			for(int i=0;i<tracks.Length;i++){
				
				Track track=tracks[i];
				
				if(track.tagName==name){
					return track;
				}
				
			}
			
			return null;
			
		}
		
		
		/// <summary>Gets all tracks by the name of the track. "style" or "dialogue" for example.</summary>
		public List<Track> getTracksByTagName(string name){
			
			List<Track> results=new List<Track>();
			
			getTracksByTagName(name,results);
			
			return results;
			
		}
		
		/// <summary>Gets all tracks by the name of the track. 
		/// "style" or "dialogue" for example. Adds the results to the given set.</summary>
		public void getTracksByTagName(string name,List<Track> results){
			
			// Ensure it's lowercase:
			name=name.ToLower();
			
			for(int i=0;i<tracks.Length;i++){
				
				Track track=tracks[i];
				
				if(track.tagName==name){
					results.Add(track);
				}
				
			}
			
		}
		
	}
	
}