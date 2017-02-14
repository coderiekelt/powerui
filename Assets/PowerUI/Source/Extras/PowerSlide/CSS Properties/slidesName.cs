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
using Css;
using PowerSlide;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the slides-name: css property.
	/// </summary>
	
	public class SlidesName:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"slides-name"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// The name of the slides:
			string name=null;
			
			if(value!=null){
				
				// Get the name:
				name=value.GetText(style.RenderData,this);
				
			}
			
			if(name=="none" || name=="initial" || name=="normal"){
				name=null;
			}
			
			// Already running a slides instance?
			Timeline timeline=Timeline.Get(style);
			
			if(name==null){
				// Clear:
				
				if(timeline!=null){
					// Stop without triggering done:
					timeline.Stop(false);
				}
				
				// Reset matched styles
				style.ApplyMatchedStyles();
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(timeline!=null){
				
				/*
				This blocks interrupts - we allow it through
				Because the user must've got past the values being non-identical check.
				
				if(timeline.src==name){
					// Stop there - already running this slides instance.
					return ApplyState.Ok;
				}
				*/
				
				// Stop it without done:
				timeline.Stop(false);
				
				// Reset matched:
				style.ApplyMatchedStyles();
				
			}
			
			// Create timeline:
			timeline=new Timeline(style);
			timeline.src=name;
			
			// Enqueue it (adds to the update queue, but not the same as a start call):
			timeline.Enqueue();
			
			// Reapply other values:
			Reapply(style,"slides-duration");
			Reapply(style,"slides-timing-function");
			Reapply(style,"slides-delay");
			Reapply(style,"slides-iteration-count");
			Reapply(style,"slides-direction");
			//Reapply(style,"slides-fill-mode");
			//Reapply(style,"slides-play-state");
			
			// Load it now!
			DataPackage package=new DataPackage(name,style.Element.document.basepath);
			
			package.onload=delegate(UIEvent e){
				
				// Check we're still the same instance:
				Timeline tl=Timeline.Get(style);
				
				if(tl!=timeline){
					return;
				}
				
				// Response:
				string slides=package.responseText;
				
				// Load the timeline now:
				timeline.load(Json.JSON.Parse(slides));
				
				// Start it now!
				timeline.Start();
				
			};
			
			// Send:
			package.send();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}