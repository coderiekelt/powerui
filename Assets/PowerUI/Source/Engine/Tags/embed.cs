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

using Dom;


namespace PowerUI{

	/// <summary>
	/// Handles the embed tag.
	/// </summary>

	[Dom.TagName("embed")]
	public class HtmlEmbedElement:HtmlElement{
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return this["align"];
			}
			set{
				this["align"]=value;
			}
		}
		
		/// <summary>The height attribute.</summary>
		public string height{
			get{
				return this["height"];
			}
			set{
				this["height"]=value;
			}
		}
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return this["name"];
			}
			set{
				this["name"]=value;
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
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return this["type"];
			}
			set{
				this["type"]=value;
			}
		}
		
		/// <summary>The width attribute.</summary>
		public string width{
			get{
				return this["width"];
			}
			set{
				this["width"]=value;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				// Always self closing:
				lexer.Push(this,false);
				
				// Also blocks framesets though:
				lexer.FramesetOk=false;
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>True if this element closes itself.</summary>
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
	}
	
}