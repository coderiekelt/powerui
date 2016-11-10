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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Css;
using Dom;


namespace PowerUI{

	/// <summary>
	/// Select dropdowns. Supports the onchange="nitroMethod" and name attributes.
	/// </summary>

	[Dom.TagName("select")]
	public class HtmlSelectElement:HtmlElement{
		
		/// <summary>True if this select is currently dropped down.</summary>
		public bool Dropped;
		/// <summary>The index of the selected option.</summary>
		public int SelectedIndex=-1;
		/// <summary>The element that displays the selected option.</summary>
		private HtmlElement DisplayText;
		/// <summary>The set of options this select provides.</summary>
		public NodeList Options;
		
		
		public HtmlSelectElement(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InSelect;
			
		}
		
		/// <summary>Closes the currently in scope select element in the given lexer.</summary>
		private void CloseSelect(HtmlLexer lexer){
		
			if(lexer.IsInSelectScope("select")){
				
				lexer.CloseInclusive("select");
				lexer.Reset();
				
			}
			
		}
		
		/// <summary>All the modes the parser can be in to cause the parser to go into the 'In select in table' mode.</summary>
		private const int InBodyTableModes=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow
		| HtmlTreeMode.InCell;
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.ReconstructFormatting();
				
				lexer.Push(this,true);
				
				lexer.FramesetOk=false;
				
				if((mode & InBodyTableModes)!=0){
					
					// In table select.
					lexer.CurrentMode=HtmlTreeMode.InSelectInTable;
					
					
				}else{
					
					// Ordinary in select.
					lexer.CurrentMode=HtmlTreeMode.InSelect;
					
				}
				
			}else if(mode==HtmlTreeMode.InSelect){
				CloseSelect(lexer);
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
				CloseSelect(lexer);
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>Gets the index for the given node.</summary>
		/// <param name="node">The node to look for.</param>
		/// <returns>The index of the node.</returns>
		public int GetSelectID(Node node){
			if(Options==null){
				return -1;
			}
			
			for(int i=0;i<Options.length;i++){
				if(Options[i]==node){
					return i;
				}
			}
			
			return -1;
		}
		
		public override void OnResetVariable(string name){
			if(DisplayText!=null){
				DisplayText.ResetVariable(name);
			}
			
			if(Options==null){
				return;
			}
			
			foreach(Node option in Options){
				(option as HtmlElement ).ResetVariable(name);
			}
		}
		
		public override void OnResetAllVariables(){
			if(DisplayText!=null){
				DisplayText.ResetAllVariables();
			}
			
			if(Options==null){
				return;
			}
			
			foreach(Node option in Options){
				(option as HtmlElement ).ResetAllVariables();
			}
		}
		
		/// <summary>Adds an option to this dropdown menu. Element.add() calls this.</summary>
		public void AddOption(HtmlElement element){
			
			// Get as option:
			HtmlOptionElement optionTag=element as HtmlOptionElement;
			
			if(optionTag!=null){
				
				// Apply dropdown:
				optionTag.Dropdown=this;
				
			}
			
			// Add to the options set:
			Options.push(element);
			
			// Manually update parent:
			element.ParentNode=this;
			
			// And update it's css by telling it the parent changed.
			// This affects inherit, height/width etc.
			element.style.Computed.ParentChanged();
			
			// If the select menu is already open, it has to be dropped again. This makes it redraw:
			if(Dropped){
				Dropped=false;
				Drop();
			}
			
		}
	   
		public override void OnTagLoaded(){
			// Append the text,dropdown and button.
			
			// Grab the options:
			Options=childNodes_;
			
			#warning disabled
			// -> We'll be using virtuals instead
			
			// Clear the innerHTML of our dropdown and write in the new content (e.g. the button/dropdown itself).
			// innerHTML="<span style='height:100%;'></span><ddbutton>";
			// Next, grab the element we want from our new innerHTML.
			// Text, for showing the current selection.
			DisplayText=childNodes_[0] as HtmlElement ;
			
			if(Options!=null){
				// Find the selected option, if there is one:
				for(int i=Options.length-1;i>=0;i--){
					
					Node element=Options[i];
					
					HtmlOptionElement optionTag=element as HtmlOptionElement;
					
					if(optionTag!=null && optionTag.Selected){
						// Found the selected option. The last option is used (thus we go through it backwards).
						// Must be done like this because this tags innerHTML won't be available until this occurs for the select to display.
						SetSelected(optionTag);
						break;
					}
				}
			}
			
			if(SelectedIndex!=-1){
				return;
			}
			// Nothing had the selected attribute (<option .. selected ..>); if it did, it would have SetSelected already.
			// We'll select the first one by default, if it exists.
			// -2 Prompts it to not call onchange, then set index 0.
			SetSelected(-2);
		}
		
		/// <summary>Drops this select box down.</summary>
		public void Drop(){
			if(Dropped){
				return;
			}
			Dropped=true;
			focus();
			HtmlElement ddBox=GetDropdownBox();
			
			if(ddBox==null){
				return;
			}
				
			ddBox.style.display="block";
			
			// Locate it to the select:
			ComputedStyle computed=Style.Computed;
			LayoutBox box=computed.FirstBox;
			
			ddBox.style.left=box.X+"px";
			ddBox.style.width=box.InnerWidth+"px";
			ddBox.style.top=(box.Y+box.PaddedHeight)+"px";
			
			ddBox.childNodes_=null;
			ddBox.innerHTML="";
			
			if(Options!=null){
				
				foreach(Node child in Options){
					ddBox.appendChild(child);
				}
				
			}
			
		}
		
		internal override void OnBlurEvent(FocusEvent fe){
			// Is the new focused element a child of ddbox?
			/*
			Node current=fe.focusing;
			
			while(current!=null){
				
				if(current.Tag=="ddbox"){
					return;
				}
				
				current=current.parentElement;
			}
			*/
			
			Hide();
		}
		
		/// <summary>Closes this dropdown.</summary>
		public void Hide(){
			if(!Dropped){
				return;
			}
			
			Dropped=false;
			HtmlElement ddBox=GetDropdownBox();
			
			if(ddBox!=null){
				ddBox.style.display="none";
			}
			
		}
		
		/// <summary>Attempts to get the box that will show the options.</summary>
		/// <returns>The dropdown box if it was found; null otherwise.</returns>
		private HtmlElement GetDropdownBox(){
			
			return htmlDocument.window.top.document.getElementByTagName("ddbox") as HtmlElement ;
		}
		
		/// <summary>Gets the currently selected value.</summary>
		/// <returns>The currently selected value.</returns>
		public string GetValue(){
			if(SelectedIndex==-1 || Options==null){
				return "";
			}
			
			HtmlElement selected=Options[SelectedIndex] as HtmlElement ;
			
			if(selected==null){
				return "";
			}
			
			string result=selected["value"];
			
			if(result==null){
				return "";
			}
			
			return result;
		}
		
		/// <summary>Sets the given element as the selected option.</summary>
		/// <param name="element">The option to set as the selected value.</param>
		public void SetSelected(HtmlElement element){
			// Find which option # the element is.
			// And set the innerHTML text to the option text. 
			int index=GetSelectID(element);
			
			if(index==SelectedIndex){
				return;
			}
			
			SetSelected(index,element,true);
		}
		
		/// <summary>Sets the option at the given index as the selected one.</summary>
		/// <param name="index">The index of the option to select.</param>
		public void SetSelected(int index){
			if(Options==null || index>=Options.length){
				return;
			}
			
			bool runOnChange=true;
			
			if(index==-2){
				// Setup/ auto selected when the tag has just been parsed.
				runOnChange=false;
				index=0;
			}
			
			HtmlElement element=null;
			if(index>=0){
				element=Options[index] as HtmlElement ;
			}
			SetSelected(index,element,runOnChange);
		}
		
		public override void OnKeyPress(KeyboardEvent pressEvent){
			
			if(this["readonly"]!=null){
				return;
			}
			
			if(pressEvent.heldDown){
				
				// Get the key:
				UnityEngine.KeyCode key=pressEvent.unityKeyCode;
				
				if(key==UnityEngine.KeyCode.Tab){
					
					// Tab - hop to next input:
					htmlDocument.TabNext();
					
				}
				
			}
		}
		
		/// <summary>Sets the option at the given index as the selected one.</summary>
		/// <param name="index">The index of the option to select.</param>
		/// <param name="element">The element at the given index.</param>
		/// <param name="runOnChange">True if the onchange event should run.</param>
		private void SetSelected(int index,HtmlElement element,bool runOnChange){
			if(index==SelectedIndex){
				return;
			}
			
			DomEvent e=new DomEvent("change");
			e.SetTrusted(false);
			
			if(!runOnChange || dispatchEvent(e)){
				
				SelectedIndex=index;
				
				if(index<0||element==null){
					// Clear the option text:
					DisplayText.innerHTML="";
				}else{
					DisplayText.innerHTML=element.innerHTML;
				}
				
			}
			
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			if(Dropped){
				Hide();
			}else{
				Drop();
			}
			
		}
		
		public override void OnMouseMoveEvent(MouseEvent e){
			
			#warning change this - clicking outside a select dropdown closes it
			
			if(e.leftMouseDown){
				// Left mouse button is currently down.
				
				// Grab the dropdown box:
				HtmlElement dropdown=GetDropdownBox();
				
				// Is it outside the select menu and outside the dropdown box?
				if(	!IsMousedOver() && !dropdown.IsMousedOver() ){
					// Yep it is - Clicked outside the dropdown menu.
					
					// Hide it now:
					Hide();
					
				}
				
			}
			
		}
		
		/// <summary>Gets or sets the value of this element. Input/Select elements only.</summary>
		public override string value{
			get{
				return GetValue();
			}
			set{
				base.value=value;
			}
		}
		
		/// <summary>Gets or sets the value as html for this element. Input/Select elements only.</summary>
		public override string htmlValue{
			get{
				return GetValue();
			}
			set{
				base.htmlValue=value;
			}
		}
		
	}
	
	
	public partial class HtmlElement{
		
		/// <summary>Updates the current selected element in a dropdown menu.</summary>
		public int selectedIndex{
			get{
				
				HtmlSelectElement tag=this as HtmlSelectElement;
				
				if(tag==null){
					return -1;
				}
				
				return tag.SelectedIndex;
			}
			set{
				
				HtmlSelectElement tag=this as HtmlSelectElement;
				
				if(tag==null){
					return;
				}
				
				tag.SetSelected(value);
				
			}
		}
		
		/// <summary>Adds a dropdown menu option.</summary>
		public void add(HtmlElement element){
			
			// Get the select tag:
			HtmlSelectElement select=this as HtmlSelectElement;
			
			if(select!=null){
				
				select.AddOption(element);
				
			}
			
		}
		
	}
	
}
