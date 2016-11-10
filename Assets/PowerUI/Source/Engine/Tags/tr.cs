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
	/// Handles a table row.
	/// </summary>
	
	[Dom.TagName("tr")]
	public class HtmlTrElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InRow;
			
		}
		
		/// <summary>True if this element is part of table structure, except for td.</summary>
		public override bool IsTableStructure{
			get{
				return true;
			}
		}
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override bool ImplicitEndAllowed{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Most common (Assumption here; most people don't bother with tbody):
			if(mode==HtmlTreeMode.InTable){
				
				lexer.CloseToTableContext();
				
				// Create a tbody:
				Element el=lexer.CreateTag("tbody",true);
				lexer.Push(el,true);
				lexer.CurrentMode = HtmlTreeMode.InTableBody;
				
				// Reproc:
				lexer.Process(this,null);
				
			}else if(mode==HtmlTreeMode.InTableBody){
				
				lexer.CloseToTableContext();
				
				lexer.Push(this,true);
				
				lexer.CurrentMode=HtmlTreeMode.InRow;
				
			}else if(mode==HtmlTreeMode.InRow){
				
				// [Table component] - Close to table body if <tr> is in scope and reprocess:
				lexer.TableBodyIfTrInScope(this,null);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				// [Table component] - Close to table cell if <th> or <td> is in scope and reprocess:
				lexer.CloseIfThOrTr(this,null);
				
			}else if(mode==HtmlTreeMode.InTemplate){
				
				// Step:
				lexer.TemplateStep(this,null,HtmlTreeMode.InTableBody);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// [Table component] - Parse error if this appears in the body.
				// Just ignore it.
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				// [Table component] - Close it and reprocess:
				lexer.CloseCaption(this,null);
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// [Table component] - Close a select (and reprocess) when it appears:
				lexer.CloseSelect(true,this,null);
				
			}else{
			
				return false;
			
			}
			
			return true;
			
			
		}
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				lexer.CloseSelect(false,null,"tr");
				
			}else if(mode==HtmlTreeMode.InRow){
				
				lexer.TableBodyIfTrInScope(null,null);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				lexer.CloseTableZoneInCell("tr");
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		public override void OnChildrenLoaded(){
			HtmlTableElement table=parentElement as HtmlTableElement;
			if(table==null){
				return;
			}
			
			int columnCount=childElementCount;
			
			if(table.ColumnWidths==null){
				table.ColumnWidths=new List<ComputedStyle>(columnCount);
			}
			
			int tableColumns=table.ColumnWidths.Count;
			
			if(columnCount>tableColumns){
				int delta=columnCount-tableColumns;
				
				for(int i=0;i<delta;i++){
					table.ColumnWidths.Add(null);
				}
			}
			
			#warning disabled
			/*
			for(int i=0;i<columnCount;i++){
				ComputedStyle activeWidest=table.ColumnWidths[i];
				ComputedStyle childColumn=childNodes_[i].Style.Computed;
				
				if(!childColumn.FixedWidth){
					continue;
				}
				
				if(activeWidest==null){
					// Must be widest - this is the first and only row so far.
					table.ColumnWidths[i]=childColumn;
				}else{
					// Is my child column wider?
					if(childColumn.PixelWidth>activeWidest.PixelWidth){
						// New wider column - use that.
						table.ColumnWidths[i]=childColumn;
					}
				}
			}
			*/
			
		}
		
	}
	
}