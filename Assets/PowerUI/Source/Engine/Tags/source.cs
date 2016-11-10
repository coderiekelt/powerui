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
	/// Represents HTML5 audio sources.
	/// </summary>

	[Dom.TagName("source")]
	public class HtmlSourceElement:HtmlElement{
		
		// NB: Fine for OnLexerAddNode to use the default.
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
	}
	
}