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
	/// Handles a table header cell.
	/// </summary>
	
	[Dom.TagName("th")]
	public class HtmlThElement:HtmlElement{
		
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
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Thorough;
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
			
			// Handle it:
			return HandleTableCellZone(this,lexer,mode);
			
		}
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			return HtmlTdElement.HandleClose("th",lexer,mode);
			
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return last ? Dom.HtmlTreeMode.InBody : Dom.HtmlTreeMode.InCell;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode. Shared by th and td.</summary>
		public static bool HandleTableCellZone(HtmlElement node,HtmlLexer lexer,int mode){
			
			// Most common:
			if(mode==HtmlTreeMode.InRow){
				
				lexer.CloseToTableContext();
				
				lexer.Push(node,true);
				
				lexer.CurrentMode=HtmlTreeMode.InCell;
				
				lexer.AddScopeMarker();
			
			}else if(mode==HtmlTreeMode.InTableBody){
				
				lexer.CloseToTableContext();
				
				Element el=lexer.CreateTag("tr",true);
				lexer.Push(el,true);
				
				// Reproc:
				lexer.Process(node,null);
			
			}else if(mode==HtmlTreeMode.InTable){
				
				lexer.CloseToTableContext();
				
				// Create a tbody:
				Element el=lexer.CreateTag("tbody",true);
				lexer.Push(el,true);
				lexer.CurrentMode = HtmlTreeMode.InTableBody;
				
				// Reproc:
				lexer.Process(node,null);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				// [Table component] - Close to table cell if <th> or <td> is in scope and reprocess:
				lexer.CloseIfThOrTr(node,null);
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// [Table component] - Close a select (and reprocess) when it appears:
				lexer.CloseSelect(true,node,null);
				
			}else if(mode==HtmlTreeMode.InTemplate){
				
				// [Table component] - Step the template:
				lexer.TemplateStep(node,null,HtmlTreeMode.InRow);
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				// [Table component] - Close it and reprocess:
				lexer.CloseCaption(node,null);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// [Table component] - Parse error if this appears in the body.
				// Just ignore it.
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}