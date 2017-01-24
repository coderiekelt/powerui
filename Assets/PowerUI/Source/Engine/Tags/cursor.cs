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

using Css;
using Dom;

namespace PowerUI{
	
	/// <summary>
	/// A cursor element. Used internally by textarea's/ input elements.
	/// </summary>
	
	[Dom.TagName("cursor")]
	public class HtmlCursorElement:HtmlElement{
		
		/// <summary>The host input element. Either a textarea or input.</summary>
		public HtmlElement Input{
			get{
				return parentElement as HtmlElement;
			}
		}
		
	}
	
}