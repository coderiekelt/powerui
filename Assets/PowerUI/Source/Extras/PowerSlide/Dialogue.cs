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
using Json;


namespace PowerSlide{
	
	/// <summary>
	/// Used when a set of slides has been loaded.
	/// </summary>
	public delegate void OnDialogueLoad(Timeline timeline);
	
	/// <summary>
	/// Used to start dialogue (speech).
	/// </summary>
	
	public static class Dialogue{
		
		/// <summary>
		/// Loads a set of dialogue options at the given path.
		/// </summary>
		internal static void Load(string startPath,OnDialogueLoad onLoad){
			
			// Append .json if it's needed:
			if(!startPath.Contains(".") && !startPath.Contains("://")){
				
				startPath+=".json";
				
			}
			
			// Localise it:
			startPath=startPath.Replace("{language}",UI.Language);
			
			// Load the file now:
			DataPackage req=new DataPackage(
				startPath,
				new Dom.Location("resources://Dialogue/",null)
			);
			
			// Delegate:
			req.onload=delegate(UIEvent e){
				
				// Response is a block of options:
				JSObject options=JSON.Parse(req.responseText);
				
				// Load it up and run the callback:
				Timeline tl=new Timeline(null);
				tl.load(options);
				
				onLoad(tl);
				
			};
			
			req.onerror=delegate(UIEvent e){
				Dom.Log.Add("Dialogue '"+startPath+"' failed. Open the PowerUI Network Inspector and try again for more.");
			};
			
			// Send!
			req.send();
			
		}
		
	}
	
}

namespace PowerUI{
	
	public partial class HtmlDocument{
		
		/// <summary>
		/// Starts the dialogue at the given path. It's relative to resources://Dialogue/ by default 
		/// and ".json" is appended if it contains no dot.
		/// E.g. "joey" => "resources://Dialogue/joey.json".
		/// "Hello/joey" => "resources://Dialogue/Hello/joey.json".
		/// "cdn://...joey.json" => as is.
		/// Use {language} in your path to localise the file.
		/// Kills any already running dialogue in the document.
		/// </summary>
		/// <param name="template">The widget template to use. 
		/// Note that the widget doesn't need to be visual - it could, for example,
		/// manage a bunch of WorldUI's instead.</param>
		public void startDialogue(string startPath,string template){
			startDialogue(startPath,template,true);
		}
		
		/// <summary>
		/// Starts the dialogue at the given path. It's relative to resources://Dialogue/ by default 
		/// and ".json" is appended if it contains no dot.
		/// E.g. "joey" => "resources://Dialogue/joey.json".
		/// "Hello/joey" => "resources://Dialogue/Hello/joey.json".
		/// "cdn://...joey.json" => as is.
		/// Use {language} in your path to localise the file.
		/// </summary>
		/// <param name="template">The widget template to use. 
		/// Note that the widget doesn't need to be visual - it could, for example,
		/// manage a bunch of WorldUI's instead.</param>
		/// <param name="killRunning">Kills any open dialogue in the document.</param>
		public void startDialogue(string startPath,string template,bool killRunning){
			
			if(killRunning){
				
				// Find all timelines in the document itself (and marked with 'isDialogue') and stop them:
				PowerSlide.Timeline current=PowerSlide.Timeline.First;
				
				while(current!=null){
					if(current.isDialogue && current.document==this){
						// Kill it!
						current.Stop(false);
					}
					
					current=current.After;
				}
				
			}
			
			// Load the options:
			PowerSlide.Dialogue.Load(startPath,delegate(PowerSlide.Timeline timeline){
				
				// Set the default template/doc:
				timeline.isDialogue=true;
				timeline.template=template;
				timeline.document=this;
				
				// Start it (which may open widgets):
				timeline.Start();
				
			});
			
		}
		
	}
	
}