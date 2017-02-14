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
		
		/// <summary>The default language for this timeline.</summary>
		public string defaultLanguage="en";
		/// <summary>An optional declared duration. Overriden by the slides-duration value.
		/// If not declared at all then it defaults to 1.</summary>
		public Css.Value duration;
		/// <summary>All the tracks in this timeline.</summary>
		public Track[] tracks;
		/// <summary>Source URL.</summary>
		public string src;
		/// <summary>The ComputedStyle that this was applied to (can be null).</summary>
		public ComputedStyle Style;
		/// <summary>A list of event listeners that *must* be destroyed 
		/// when either this timeline is killed or is un-paused.</summary>
		internal List<CueElementData> CueElements;
		
		public Timeline(ComputedStyle style){
			Style=style;
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
				
				// Default duration '1':
				duration=new Css.Units.DecimalUnit(1f);
				
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
					
				}else{
					duration=null;
				}
				
				if(duration==null){
					
					// Default is '1':
					duration=new Css.Units.DecimalUnit(1f);
					
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