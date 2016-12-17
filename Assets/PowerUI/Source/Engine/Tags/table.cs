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
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles a table.
	/// </summary>
	
	[Dom.TagName("table")]
	public class HtmlTableElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphButtonScope();
				lexer.Push(this,true);
				lexer.FramesetOk=false;
				lexer.CurrentMode=HtmlTreeMode.InTable;
				
			}else if(mode==HtmlTreeMode.InTable){
				
				
				if(lexer.IsInTableScope("table")){
					// Ignore otherwise
					
					lexer.CloseInclusive("table");
					
					// Reset mode:
					lexer.Reset();
					
					// Reprocess:
					lexer.Process(this,null);
					
				}
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// [Table component] - Close a select (and reprocess) when it appears:
				lexer.CloseSelect(true,this,null);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InTable){
				
				// Close it
				
				if(lexer.IsInTableScope("table")){
					// Ignore otherwise
					
					lexer.CloseInclusive("table");
					
					// Reset mode:
					lexer.Reset();
					
				}
				
			}else if(mode==HtmlTreeMode.InTableBody){
				
				// Close to table if in a table body context and reprocess:
				lexer.CloseToTableBodyIfBody(null,"table");
				
			}else if(mode==HtmlTreeMode.InRow){
				
				lexer.TableBodyIfTrInScope(null,"table");
				
			}else if(mode==HtmlTreeMode.InCell){
				
				lexer.CloseTableZoneInCell("table");
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				lexer.CloseCaption(null,"table");
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				lexer.CloseSelect(false,null,"table");
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>True if this element is part of table structure, except for td.</summary>
		public override bool IsTableStructure{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element is a table context.</summary>
		public override bool IsTableContext{
			get{
				return true;
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InTable;
			
		}
		
	}
	
}