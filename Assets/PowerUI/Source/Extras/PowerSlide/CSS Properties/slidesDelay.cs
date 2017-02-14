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
	/// Represents the slides-delay: css property.
	/// </summary>
	
	public class SlidesDelay:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"slides-delay"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			Timeline si=Timeline.Get(style);
			
			if(si==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(value==null || value.Type==ValueType.Text){
				si.Delay=0;
			}else{
				si.Delay=value.GetDecimal(style.RenderData,this) / 1000f;
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



