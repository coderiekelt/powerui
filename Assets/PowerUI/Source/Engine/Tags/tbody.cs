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
	/// Represents a table body element.
	/// </summary>
	
	[Dom.TagName("tbody")]
	public class HtmlTableBodyElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InTableBody;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			return HandleTableBodyZone(this,lexer,mode);
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			return HandleTableBodyClose("tbody",lexer,mode);
			
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
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable | HtmlTreeMode.InCaption;
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode. Shared by tbody, thead and tfoot.</summary>
		public static bool HandleTableBodyClose(string close,HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Ignore
				
			}else if(mode==HtmlTreeMode.InRow){
				
				if(lexer.IsInTableScope(close) && lexer.IsInTableScope("tr")){
					// Ignore otherwise.
					
					lexer.CloseToTableContext();
					
					lexer.CloseCurrentNode();
					lexer.CurrentMode=HtmlTreeMode.InTableBody;
					
				}
				
			}else if(mode==HtmlTreeMode.InTableBody){
			
				if(lexer.IsInTableScope(close)){
					// Ignore otherwise.
					
					lexer.CloseToTableContext();
					
					lexer.CloseCurrentNode();
					lexer.CurrentMode=HtmlTreeMode.InTable;
					
				}
				
			}else if(mode==HtmlTreeMode.InCell){
				
				lexer.CloseTableZoneInCell(close);
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				lexer.CloseSelect(false,null,close);
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode. Shared by tbody, thead and tfoot.</summary>
		public static bool HandleTableBodyZone(HtmlElement node,HtmlLexer lexer,int mode){
			
			// Most common case:
			if(mode==HtmlTreeMode.InTable){
				
				lexer.CloseToTableContext();
				
				lexer.Push(node,true);
				lexer.CurrentMode = HtmlTreeMode.InTableBody;
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// [Table component] - Parse error if this appears in the body.
				// Just ignore it.
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				// [Table component] - Close it and reprocess:
				lexer.CloseCaption(node,null);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				// [Table component] - Close to table cell if <th> or <td> is in scope and reprocess:
				lexer.CloseIfThOrTr(node,null);
				
			}else if(mode==HtmlTreeMode.InTableBody){
				
				// [Table component] - Close to table if in a table body context and reprocess:
				lexer.CloseToTableIfBody(node,null);
				
			}else if(mode==HtmlTreeMode.InRow){
				
				// [Table component] - Close to table body if <tr> is in scope and reprocess:
				lexer.TableBodyIfTrInScope(node,null);
				
			}else if(mode==HtmlTreeMode.InTemplate){
				
				// [Table component] - Step the template:
				lexer.TemplateStep(node,null,HtmlTreeMode.InTable);
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}