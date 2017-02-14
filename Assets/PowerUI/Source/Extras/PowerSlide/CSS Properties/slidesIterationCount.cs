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
	/// Represents the slides-iteration-count: css property.
	/// </summary>
	
	public class SlidesIterationCount:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"slides-iteration-count"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			Timeline si=Timeline.Get(style);
			
			if(si==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				si.RepeatCount=1;
			}else if(value.Type==ValueType.Text){
				// Infinite
				si.RepeatCount=-1;
			}else{
				si.RepeatCount=value.GetInteger(style.RenderData,this);
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



