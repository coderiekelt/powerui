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
		
		/// <summary>The sizes attribute.</summary>
		public string sizes{
			get{
				return this["sizes"];
			}
			set{
				this["sizes"]=value;
			}
		}
		
		/// <summary>The media attribute.</summary>
		public string media{
			get{
				return this["media"];
			}
			set{
				this["media"]=value;
			}
		}
		
		/// <summary>The src attribute.</summary>
		public string src{
			get{
				return this["src"];
			}
			set{
				this["src"]=value;
			}
		}
		
		/// <summary>The srcset attribute.</summary>
		public string srcset{
			get{
				return this["srcset"];
			}
			set{
				this["srcset"]=value;
			}
		}
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return this["type"];
			}
			set{
				this["type"]=value;
			}
		}
		
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