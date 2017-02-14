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
	/// A dialogue slide. Essentially this is like a single cue card in a series of dialogue.
	/// </summary>
	
	public class DialogueSlide : Slide{
		
		/// <summary>
		/// One or more speakers who say this. Note that a single 'speaker' object can
		/// be multiple actual speakers. For example, it might be referring to a 'clan member' object.
		/// In the scene, there could be 5 clan members.
		/// </summary>
		public Speaker[] speakers;
		/// <summary>
		/// The template to use.
		/// Using this will override whatever your default is, allowing you to have left/right style dialogue.
		public string template;
		/// <summary>
		/// When this slide is displayed as an option, the markup to show. Indexed by language (e.g. "en").
		/// </summary>
		internal Dictionary<string,string> option;
		/// <summary>
		/// The markup to show when this slide is visible. Indexed by language (e.g. "en").
		/// </summary>
		internal Dictionary<string,string> markup;
		
		
		/// <summary>Gets the markup for a particular language.</summary>
		public string getMarkup(string language){
			return GetFromSet(language,true,markup);
		}
		
		/// <summary>Gets the markup for a particular language.</summary>
		public string getMarkup(string language,bool fallbackOnDefault){
			
			return GetFromSet(language,fallbackOnDefault,markup);
			
		}
		
		/// <summary>Gets a value from a localised set at the given language code.
		/// Optionally falls back onto the default language choice (usually 'en').</summary>
		private string GetFromSet(string language,bool fallbackOnDefault,Dictionary<string,string> set){
			
			if(language==null){
				// Default language:
				language=track.timeline.defaultLanguage;
			}
			
			language=language.Trim().ToLower();
			
			if(set==null){
				return null;
			}
			
			string value;
			if(!set.TryGetValue(language,out value)){
				
				if(fallbackOnDefault){
					
					// Use default language:
					set.TryGetValue(track.timeline.defaultLanguage,out value);
					
				}
				
			}
			
			return value;
		}
		
		/// <summary>Loads a value into a localised set.</summary>
		private Dictionary<string,string> LoadLocalised(JSObject json){
			
			var result=new Dictionary<string,string>();
			
			if(json==null){
				return result;
			}
			
			// Just a string?
			if(json is Json.JSValue){
				
				// Default language:
				result[track.timeline.defaultLanguage]=json.ToString();
				
			}else{
				
				// Should be a set of languages:
				foreach(KeyValuePair<string,JSObject> kvp in json){
					
					// Get as nice names:
					string langCode=kvp.Key;
					JSObject translation=kvp.Value;
					
					if(translation==null){
						continue;
					}
					
					// Lowercase:
					langCode=langCode.Trim().ToLower();
					
					// Add to set:
					result[langCode]=translation.ToString();
					
				}
				
			}
			
			return result;
			
		}
		
		public override void load(JSObject json){
			
			// If it's just a string, then we've only got markup here:
			// ["Hello","I'm Dave"]
			if(json is JSValue){
				
				// markup only:
				markup=LoadLocalised(json);
				
				return;
			}
			
			// Template:
			template=json.String("template");
			
			// Markup:
			markup=LoadLocalised(json["markup"]);
			
			// Option:
			option=LoadLocalised(json["option"]);
			
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
				
				// Check for just 'speaker':
				speakers=json["speaker"] as JSArray;
				
				if(speakers!=null){
					
					LoadSpeaker(0,speakers);
					
				}
				
			}
			
			// Load everything else:
			base.load(json);
			
		}
		
		/// <summary>Sets up a speaker at the given index in the Speakers set.</summary>
		internal void LoadSpeaker(int index,JSObject data){
			
			if(speakers==null){
				speakers=new Speaker[1];
			}
			
			// Create and add:
			speakers[index]=new Speaker(this,data);
			
		}
		
	}
	
}