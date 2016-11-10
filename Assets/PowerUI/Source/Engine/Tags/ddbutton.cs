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


namespace PowerUI{
	
	/// <summary>
	/// Dropdown button tag. Used by select internally - when clicked, it displays the dropdown menu.
	/// </summary>
	
	[Dom.TagName("ddbutton")]
	public class HtmlDDButtonElement:HtmlElement{
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override void OnTagLoaded(){
			innerHTML="v";
		}
		
	}
	
}