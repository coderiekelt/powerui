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
		public bool Dropped{
			get{
				return Dropdown!=null;
			}
		}
		
		/// <summary>The selected node.</summary>
		private HtmlOptionElement SelectedNode_;
		/// <summary>The currently open dropdown.</summary>
		public HtmlDropdownElement Dropdown;
		/// <summary>The index of the selected option.</summary>
		private int SelectedIndex_=-1;
		/// <summary>The element that displays the selected option (it's a div; the same as Firefox).</summary>
		private HtmlDivElement Placeholder{
			get{
				
				if(RenderData.Virtuals==null){
					return null;
				}
				
				return RenderData.Virtuals.Get(HtmlSelectButtonElement.Priority-1) as HtmlDivElement;
				
			}
		}
		
		/// <summary>The current selected option.</summary>
		public HtmlOptionElement SelectedOption{
			get{
				return SelectedNode_;
			}
		}
		
		/// <summary>The current selected index.</summary>
		public int SelectedIndex{
			get{
				
				if(SelectedIndex_==-2){
					
					// Figure it out now:
					SelectedIndex_=GetOptionIndex(SelectedNode_);
					
				}
				
				return SelectedIndex_;
			}
		}
		
		/// <summary>The button.</summary>
		private HtmlSelectButtonElement Button{
			get{
				
				if(RenderData.Virtuals==null){
					return null;
				}
				
				return RenderData.Virtuals.Get(HtmlSelectButtonElement.Priority) as HtmlSelectButtonElement;
				
			}
		}
		
		/// <summary>The set of options this select provides.</summary>
		public NodeList RawOptions{
			get{
				return childNodes_;
			}
		}
		
		
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
		
		/// <summary>Gets an option by its index.</summary>
		public HtmlOptionElement GetOption(int index){
			if(index<0){
				return null;
			}
			
			// Go through all options that are kids of this select menu:
			return GetOption(this,ref index);
		}
		
		/// <summary>Searches parent (which must be a parent) for the given node.
		/// Each time an option element is encountered, currentIndex is increased.</summary>
		private HtmlOptionElement GetOption(Node parent,ref int targetIndex){
			
			// For each child node..
			for(int i=0;i<parent.childNodes_.length;i++){
				
				// Get it:
				Node child=parent.childNodes_[i];
				
				// Might be an optgroup containing it. Check if it is:
				if(child is HtmlOptionElement){
					
					// Is it the one we're after?
					if(targetIndex<=0){
						// Yes!
						return child as HtmlOptionElement;
					}
					
					// Just decrease index:
					targetIndex--;
					continue;
				}
				
				if(child.childNodes_!=null){
					
					// Go recursive:
					HtmlOptionElement inChild=GetOption(child,ref targetIndex);
					
					if(inChild!=null){
						return inChild;
					}
					
				}
				
			}
			
			// Not found here.
			return null;
		}
		
		/// <summary>Gets the index for the given node.</summary>
		/// <param name="node">The node to look for.</param>
		/// <returns>The index of the node.</returns>
		public int GetOptionIndex(Node node){
			
			// Go through all options that are kids of this select menu.
			if(childNodes_==null){
				return -1;
			}
			
			int currentIndex=0;
			
			if(GetOptionIndex(this,node,ref currentIndex)){
				return currentIndex;
			}
			
			return -1;
		}
		
		/// <summary>Searches parent (which must be a parent) for the given node.
		/// Each time an option element is encountered, currentIndex is increased.</summary>
		private bool GetOptionIndex(Node parent,Node node,ref int currentIndex){
			
			// For each child node..
			for(int i=0;i<parent.childNodes_.length;i++){
				
				// Get it:
				Node child=parent.childNodes_[i];
				
				// Match?
				if(child==node){
					// Got it!
					return true;
				}
				
				// Might be an optgroup containing it. Check if it is:
				if(child is HtmlOptionElement){
					// Got an option - just bump up the index:
					currentIndex++;
					continue;
				}
				
				if(child.childNodes_!=null){
					
					// Go recursive:
					if(GetOptionIndex(child,node,ref currentIndex)){
						return true;
					}
					
				}
				
			}
			
			// Not found here.
			return false;
		}
		
		/// <summary>Adds an option to this dropdown menu. Element.add() calls this.</summary>
		public void AddOption(HtmlElement element){
			
			// Just add as a child:
			appendChild(element);
			
		}
	   
		public override void OnTagLoaded(){
			
			// Append the text,dropdown and button.
			
			// Append the special 'placeholder' and dropdown button (virtual elements):
			ComputedStyle computed=Style.Computed;
			
			computed.GetOrCreateVirtual(HtmlSelectButtonElement.Priority-1,"div");
			computed.GetOrCreateVirtual(HtmlSelectButtonElement.Priority,"selectbutton");
			
			// Selects are unusual in that they don't draw their own childnodes:
			RenderData.Virtuals.AllowDrawKids=false;
			
		}
		
		public override void OnChildrenLoaded(){
			
			if(SelectedNode_==null){
				// Nothing had the selected attribute (<option .. selected ..>).
				// If it did, it would have SetSelected already.
				// Select the first one by default, if it exists.
				// -3 Prompts it to not call onchange, then set index 0.
				SetSelected(-3);
			}
			
		}
		
		/// <summary>Drops this select box down.</summary>
		public void Drop(){
			if(Dropped){
				return;
			}
			
			// Focus if it isn't already:
			focus();
			
			// Get the CS of the HTML node:
			ComputedStyle computed=htmlDocument.html.Style.Computed;
			
			// Get/create it:
			Dropdown=computed.GetOrCreateVirtual(HtmlDropdownElement.Priority,"dropdown") as HtmlDropdownElement;
			
			// Act like the options are kids of the dropdown:
			Dropdown.childNodes_=RawOptions;
			
			// Get my computed style:
			computed=Style.Computed;
			
			// Locate it to the select:
			LayoutBox box=computed.FirstBox;
			
			Dropdown.style.left=box.X+"px";
			Dropdown.style.width=box.Width+"px";
			Dropdown.style.top=(box.Y+box.PaddedHeight)+"px";
			
		}
		
		internal override void OnBlurEvent(FocusEvent fe){
			// Is the new focused element a child of dropdown?
			Dom.Element current=fe.focusing as Dom.Element;
			
			while(current!=null){
				
				if(current.Tag=="dropdown"){
					return;
				}
				
				current=current.parentElement;
			}
			
			Hide();
		}
		
		/// <summary>Closes this dropdown.</summary>
		public void Hide(){
			if(!Dropped){
				return;
			}
			
			// Clear the child nodes (we don't want them to think they've been removed from the DOM):
			Dropdown.childNodes_=null;
			Dropdown=null;
			
			// Get the CS of the HTML node:
			ComputedStyle computed=htmlDocument.html.Style.Computed;
			
			// Remove:
			computed.RemoveVirtual(HtmlDropdownElement.Priority);
			
		}
		
		/// <summary>Gets the currently selected value.</summary>
		/// <returns>The currently selected value.</returns>
		public string GetValue(){
			if(SelectedNode_==null){
				return "";
			}
			
			return SelectedNode_["value"];
		}
		
		/// <summary>Sets the given element as the selected option.</summary>
		/// <param name="element">The option to set as the selected value.</param>
		public void SetSelected(HtmlOptionElement element){
			SetSelected(-2,element,true);
		}
		
		/// <summary>Sets the option at the given index as the selected one.</summary>
		/// <param name="index">The index of the option to select.</param>
		public void SetSelected(int index){
			
			bool runOnChange=true;
			
			if(index==-3){
				// Setup/ auto selected when the tag has just been parsed.
				runOnChange=false;
				index=0;
			}
			
			// Get and set:
			HtmlOptionElement element=GetOption(index);
			SetSelected(index,element,runOnChange);
		}
		
		/// <summary>Sets the option at the given index as the selected one.</summary>
		/// <param name="index">The index of the option to select.</param>
		/// <param name="element">The element at the given index.</param>
		/// <param name="runOnChange">True if the onchange event should run.</param>
		private void SetSelected(int index,HtmlOptionElement element,bool runOnChange){
			
			if(element==SelectedNode_){
				return;
			}
			
			Dom.Event e=new Dom.Event("change");
			e.SetTrusted(false);
			
			// Cache previous:
			int prevIndex=SelectedIndex_;
			HtmlOptionElement prevNode=SelectedNode_;
			
			// Update current (so onchange gets the correct value):
			SelectedNode_=element;
			SelectedIndex_=index;
			
			if(runOnChange){
				
				if(!dispatchEvent(e)){
					
					// Restore to previous:
					SelectedIndex_=prevIndex;
					SelectedNode_=prevNode;
					
					return;
				}
				
			}
			
			// Update placeholder:
			if(SelectedNode_==null){
				// Clear the option text:
				Placeholder.innerHTML="";
				index=-1;
			}else{
				Placeholder.innerHTML=SelectedNode_.innerHTML;
			}
			
		}
		
		public override void OnResetAllVariables(){
			base.OnResetAllVariables();
			
			// Update placeholder:
			if(SelectedNode_!=null){
				Placeholder.innerHTML=SelectedNode_.innerHTML;
			}
			
		}
		
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(e.type=="mousedown" && bubblePhase){
				
				if(Dropped){
					Hide();
				}else{
					Drop();
				}
				
			}
			
			// Handle locally:
			return base.HandleLocalEvent(e,bubblePhase);
			
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
