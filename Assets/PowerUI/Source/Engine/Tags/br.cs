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
using System;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{

	/// <summary>
	/// Represents line breaks.
	/// </summary>

	[Dom.TagName("br")]
	public class HtmlBrElement:HtmlElement{
		
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
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			// If we follow a block element, we have height. Otherwise just act like a block element.
			HtmlElement previousEle=previousSibling as HtmlElement ;
			
			if(previousEle!=null && previousEle.computedStyle.FirstBox.DisplayMode == Css.DisplayMode.Block){
				
				// We follow a block element. Define some height:
				heightUndefined=false;
				box.InnerHeight=Style.Computed.FontSizeX;
				
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
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				// Acts like a self-closed open node:
				Element el=lexer.CreateTag("br",true);
				lexer.Push(el,false);
				
			}else if(mode==HtmlTreeMode.BeforeHtml){
				
				// Allowed to fall through the 'anything else' case:
				lexer.BeforeHtmlElse(null,"br");
				
			}else if(mode==HtmlTreeMode.InHead){
				
				// Use anything else method:
				lexer.InHeadElse(null,"br");
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				// Use anything else method:
				lexer.AfterHeadElse(null,"br");
				
			}else if(mode==HtmlTreeMode.InHeadNoScript){
				
				// Reprocess as in head:
				lexer.CurrentMode = HtmlTreeMode.InHead;
				lexer.Process(null,"br");
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}