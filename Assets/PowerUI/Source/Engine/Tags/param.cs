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
	/// Represents object parameters. Although object itself isn't supported
	/// this tag is; a page with an object on it can still load.
	/// </summary>

	[Dom.TagName("param")]
	public class HtmlParamElement:HtmlElement{
		
		// NB: Fine for OnLexerAddNode to use the default.
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return this["name"];
			}
			set{
				this["name"]=value;
			}
		}
		
		/// <summary>The value attribute.</summary>
		public string Value{
			get{
				return this["value"];
			}
			set{
				this["value"]=value;
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
		
		/// <summary>The valuetype attribute.</summary>
		public string valueType{
			get{
				return this["valuetype"];
			}
			set{
				this["valuetype"]=value;
			}
		}
		
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