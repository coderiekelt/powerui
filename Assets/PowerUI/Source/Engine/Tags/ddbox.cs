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
	/// Dropdown box. Used by select internally.
	/// </summary>
	
	[Dom.TagName("ddbox")]
	public class HtmlDDBoxElement:HtmlElement{
		
		public override void OnTagLoaded(){
			// Document.DropdownBox=this;
		}
		
	}
	
}