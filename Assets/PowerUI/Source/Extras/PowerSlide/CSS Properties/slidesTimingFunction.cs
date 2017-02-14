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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the slides-timing-function: css property
	/// </summary>
	
	public class SlidesTimingFunction:CssProperty{
		
		public static SlidesTimingFunction GlobalProperty;
		
		public SlidesTimingFunction(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"slides-timing-function"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			Timeline si=Timeline.Get(style);
			
			if(si==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			si.SetTimingFunction(value);
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



