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
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles the standard inline label element.
	/// Clicking on them acts just like clicking on the input they target.
	/// </summary>
	
	[Dom.TagName("label")]
	public class HtmlLabelElement:HtmlElement{
		
		/// <summary>The ID of the element the clicks of this get 'directed' at.
		/// If blank/null, the first child of this element that is an 'input' is used.</summary>
		public string ForElement;
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			if(property=="for"){
				ForElement=this["for"];
				return true;
			}
			return false;
		}
		
		/// <summary>Gets the element this label is for. If found, it should always be an input.</summary>
		public HtmlElement GetFor(){
			
			// ForElement is an ID - lets go find the element in the document with that ID.
			return document.getElementById(ForElement) as HtmlElement ;
			
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Who wants the click? That's the for element:
			HtmlElement forElement=GetFor();
			
			if(forElement!=null && clickEvent.isTrusted){
				
				// Click it (note that this click is *not trusted* which blocks it from going recursive):
				forElement.click();
				
			}
			
		}
		
	}
	
}