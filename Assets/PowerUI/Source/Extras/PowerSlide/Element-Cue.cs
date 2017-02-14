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
using PowerSlide;
using Css;


namespace Dom{
	
	public partial class Element{
		
		/// <summary>Cues the element to advance any paused PowerSlide slides.</summary>
		public void cue(){
			
			// Get the computedStyle (without requiring a HtmlElement):
			ComputedStyle cs=(this as IRenderableNode).ComputedStyle;
			
			// Get the PowerSlide (if any) for that CS:
			Timeline slides=Timeline.Get(cs);
			
			if(slides!=null){
				// Great - unpause it!
				slides.SetPause(false);
			}
			
		}
		
	}
	
}