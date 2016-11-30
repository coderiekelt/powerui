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
using Dom;


namespace PowerUI{

	/// <summary>
	/// Handles option tags for dropdowns. Supports the selected and value="" attributes.
	/// </summary>

	[Dom.TagName("option")]
	public class HtmlOptionElement:HtmlElement{
		
		/// <summary>True if this is the selected option.</summary>
		public bool Selected;
		/// <summary>True if this tag has been fully loaded. False if it's still being parsed.</summary>
		private bool FullyLoaded;
		/// <summary>The select dropdown that this option belongs to.</summary>
		public HtmlSelectElement Dropdown;
		
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Normal;
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
			
			if(mode==HtmlTreeMode.InSelect){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
				lexer.Push(this,true);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
				lexer.ReconstructFormatting();
				
				lexer.Push(this,true);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InSelect){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){	
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="selected"){
				string isSelected=this["selected"];
				Selected=(string.IsNullOrEmpty(isSelected) || isSelected=="1" || isSelected=="true" || isSelected=="yes");
				if(FullyLoaded){
					// Tell the select:
					Dropdown.SetSelected(this);
				}	
				return true;
			}else if(property=="text"){
				
				// Write the innerHTML:
				innerHTML=this["text"];
				
				return true;
			}
			
			return false;
		}
		
		public override void OnTagLoaded(){
			FullyLoaded=true;
			Dropdown=GetSelect(parentElement);
		}
		
		/// <summary>Gets or finds the parent select tag that this option belongs to.</summary>
		/// <param name="element">The element to check if it's a select.</param>
		/// <returns>The select tag if found; null otherwise.</returns>
		private HtmlSelectElement GetSelect(Element element){
			if(element==null){
				return null;
			}
			if(element.Tag=="select"){
				return (HtmlSelectElement)(element);
			}
			return GetSelect(element.parentElement);
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			if(Dropdown==null){
				return;
			}
			
			Dropdown.SetSelected(this);
			Dropdown.Hide();
			
		}
		
	}
	
	public partial class HtmlElement{
		
		/// <summary>The text of an option element.</summary>
		public string text{
			get{
				return this["text"];
			}
			set{
				this["text"]=value;
			}
		}
		
	}
	
}