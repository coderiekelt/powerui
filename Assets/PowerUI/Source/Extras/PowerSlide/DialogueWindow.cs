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
using System.Collections;
using System.Collections.Generic;


namespace Windows{
	
	/// <summary>
	/// A window which manages dialogue (speech). Used by PowerSlide.
	/// They typically display dialogue, but doing so isn't required.
	/// I.e. they can manage a collection of WorldUI's instead.
	/// </summary>
	
	public class DialogueWindow : Window{
		
		/// <summary>The timeline in use</summary>
		public PowerSlide.Timeline Timeline;
		
		
		/// <summary>Occurs when an option is clicked.</summary>
		public static void RunOption(PowerUI.MouseEvent e){
			
			Dom.Node targetNode=(e.target as Dom.Node);
			
			// Get the unique ID:
			int uniqueId;
			if(!int.TryParse(targetNode["unique-id"],out uniqueId)){
				Dom.Log.Add("A dialogue option did not have an unique-id attribute.");
				return;
			}
			
			// Get the window:
			DialogueWindow dw=e.sparkWindow as DialogueWindow;
			
			if(dw==null){
				Dom.Log.Add("A dialogue option tried to run but it's not inside a DialogueWindow.");
				return;
			}
			
			// Great ok - we can now grab the active options entry.
			// Select the active slide with that unique ID:
			PowerSlide.DialogueSlide slide=dw.getSlide(uniqueId) as PowerSlide.DialogueSlide;
			
			if(slide==null){
				Dom.Log.Add("Unable to resolve a DialogueSlide from a unique-id.");
				return;
			}
			
			// Get the GOTO:
			string gotoUrl=slide.slidesToGoTo;
			
			if(string.IsNullOrEmpty(gotoUrl)){
				
				// Acts just like a continue does.
				// Just cue it:
				dw.Timeline.Cue();
				return;
				
			}
			
			// Load it now (killing existing dialogue):
			dw.Timeline.document.startDialogue(gotoUrl,dw.Timeline.template);
			
			// Kill the event:
			e.stopPropagation();
			
		}
		
		/// <summary>A convenience function for setting up onmousedown and unique-id attributes.
		/// onmousedown points at Windows.DialogueWindow.RunOption.</summary>
		protected string OptionMouseDown(PowerSlide.Slide option){
			
			return "onmousedown='Windows.DialogueWindow.RunOption' unique-id='"+option.uniqueID+"'";
			
		}
		
		/// <summary>Gets an active slide by its unique ID.</summary>
		public PowerSlide.Slide getSlide(int uniqueID){
			return Timeline.getSlide(uniqueID);
		}
		
		/// <summary>Gets all dialogue slides which are currently active.</summary>
		public List<PowerSlide.DialogueSlide> allActive{
			get{
				return Timeline.GetActive<PowerSlide.DialogueSlide>();
			}
		}
		
		/// <summary>Called when the template is ready.</summary>
		internal override void Goto(string url,Dictionary<string,object> globals){
			
			// Just get the timeline:
			object timelineObj;
			if(globals.TryGetValue("timeline",out timelineObj)){
				
				// Get the timeline:
				Timeline=timelineObj as PowerSlide.Timeline;
				
			}
			
		}
		
		/// <summary>The depth that this type of window lives at.</summary>
		public override int Depth{
			get{
				return 1100;
			}
		}
		
		/// <summary>Handles events on the window itself.</summary>
		protected override void OnEvent(Dom.Event e){
			
			// Catch PowerSlide dialogue events and convert them to our virtual methods:
			PowerSlide.SlideEvent se;
			
			if(e.type=="dialoguestart"){
				
				// It's a slide event:
				se=e as PowerSlide.SlideEvent;
				
				// Get the slide:
				Show(se.slide as PowerSlide.DialogueSlide);
				
			}else if(e.type=="dialogueend"){
				
				// It's a slide event:
				se=e as PowerSlide.SlideEvent;
				
				// Hide:
				Hide(se.slide as PowerSlide.DialogueSlide);
				
			}else if(e.type=="slidespause"){
				
				// The slides just paused (and are now waiting for a cue)
				se=e as PowerSlide.SlideEvent;
				
				// Waiting for a cue:
				WaitForCue(se);
				
			}else if(e.type=="slidesplay"){
				
				// The slides just cued (and are now continuing).
				se=e as PowerSlide.SlideEvent;
				
				// Cue received:
				Cued(se);
				
			}
			
			base.OnEvent(e);
			
		}
		
		/// <summary>Called when the dialogue is now waiting for a cue event.</summary>
		protected virtual void WaitForCue(PowerSlide.SlideEvent e){
			
		}
		
		/// <summary>Called when the dialogue got cued.</summary>
		protected virtual void Cued(PowerSlide.SlideEvent e){
			
		}
		
		/// <summary>Called when the given slide requested to display.
		/// Note that multiple slides may request to be visible at the same time.</summary>
		protected virtual void Show(PowerSlide.DialogueSlide dialogue){
			
		}
		
		/// <summary>Called when the given slide requested to hide.
		/// Note that there may be multiple visible slides.</summary>
		protected virtual void Hide(PowerSlide.DialogueSlide dialogue){
			
		}
		
	}
	
	public partial class Window{
		
		/// <summary>Cues dialogue within a window.</summary>
		public static void Cue(PowerUI.MouseEvent me){
			
			// The window:
			Windows.Window window=me.htmlTarget.sparkWindow;
			
			// Get the timeline:
			PowerSlide.Timeline tl=PowerSlide.Timeline.Get(window);
			
			// Cue it:
			if(tl!=null){
				tl.Cue();
			}
			
		}
		
	}
	
}