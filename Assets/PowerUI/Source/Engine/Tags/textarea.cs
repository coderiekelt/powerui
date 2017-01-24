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
using Css;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles textarea tags.
	/// </summary>
	
	[Dom.TagName("textarea")]
	public class HtmlTextareaElement:HtmlElement{
		
		/// <summary>This is the cursor element.</summary>
		public HtmlCursorElement Cursor;
		/// <summary>The maximum length of text in this box.</summary>
		public int MaxLength=int.MaxValue;
		/// <summary>A placeholder value.</summary>
		public string Placeholder="";
		
		
		public HtmlTextareaElement(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		/// <summary>Gets or sets the value of this element. Input/Select/Textarea elements only.</summary>
		public override string value{
			get{
				return textContent;
			}
			set{
				SetValue(value);
			}
		}
		
		/// <summary>Looks out for paste events.</summary>
		protected override bool HandleLocalEvent(DomEvent e,bool bubblePhase){
			
			// Handle locally:
			if(base.HandleLocalEvent(e,bubblePhase)){
				
				if(e is ClipboardEvent && e.type=="paste"){
					
					// Paste the data at the cursor index (must be text only).
					string textToPaste=(e as ClipboardEvent).text;
					
					if(textToPaste!=null){
						
						string value=this.value;
						
						if(value==null){
							value=""+textToPaste;
						}else{
							value=value.Substring(0,CursorIndex)+textToPaste+value.Substring(CursorIndex,value.Length-CursorIndex);
						}
						
						SetValue(value);
						MoveCursor(CursorIndex+textToPaste.Length,true);
						
					}
					
				}
				
				return true;
				
			}
			
			return false;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.SkipNewline();
				
				lexer.FramesetOk=false;
				
				lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.RCData);
				
			}else if(mode==HtmlTreeMode.InSelect){
				
				lexer.InputOrTextareaInSelect(this);
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>The current location of the cursor.</summary>
		public int CursorIndex{
			get{
				if(Cursor==null){
					return 0;
				}
				
				return Cursor.Index;
			}
		}
		
		/// <summary>The container holding the text.</summary>
		public HtmlTextNode TextHolder{
			get{
				return firstChild as HtmlTextNode;
			}
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="placeholder"){
				
				Placeholder=this["placeholder"];
				innerHTML=Placeholder;
				
				return true;
				
			}else if(property=="maxlength"){
				
				string value=this["maxlength"];
				
				if(string.IsNullOrEmpty(value)){
					// It's blank - set it to the default.
					MaxLength=int.MaxValue;
				}else{
					
					// Parse the maximum length from the string:
					if(!int.TryParse(value,out MaxLength)){
						
						// Not a number!
						MaxLength=int.MaxValue;
						
					}
					
				}
				
				return true;
			}
			
			return false;
		}
		
		public override KeyboardMode OnShowMobileKeyboard(){
			KeyboardMode result=new KeyboardMode();
			return result;
		}
		
		/// <summary>Sets the value of this textarea.</summary>
		/// <param name="value">The value to set.</param>
		private void SetValue(string newValue){
			
			DomEvent e=new DomEvent("change");
			e.SetTrusted(false);
			if(!dispatchEvent(e)){
				return;
			}
			
			if(MaxLength!=int.MaxValue){
				// Do we need to clip it?
				if(newValue.Length>MaxLength){
					// Yep!
					newValue=newValue.Substring(0,MaxLength);
				}
			}
			
			if(CursorIndex>newValue.Length){
				MoveCursor(0);
			}
			
			if(newValue=="" && Placeholder!=""){
				newValue=Placeholder;
			}
			
			// Write text content:
			textContent=newValue;
			
			// Redraw:
			RequestLayout();
			
		}
		
		public override void OnKeyPress(KeyboardEvent pressEvent){
			
			if(this["readonly"]!=null){
				return;
			}
			
			if(pressEvent.heldDown){
				// Add to value unless it's backspace:
				string value=this.value;
				
				if(!char.IsControl(pressEvent.character) && pressEvent.character!='\0'){
					
					// Drop the character in the string at cursorIndex
					if(value==null){
						value=""+pressEvent.character;
					}else{
						value=value.Substring(0,CursorIndex)+pressEvent.character+value.Substring(CursorIndex,value.Length-CursorIndex);
					}
					
					SetValue(value);
					MoveCursor(CursorIndex+1);
					
					return;
				}
				
				// It's a command character:
				
				KeyCode key=((KeyCode)pressEvent.keyCode);
				
				if(key==KeyCode.LeftArrow){
					MoveCursor(CursorIndex-1,true);
				}else if(key==KeyCode.RightArrow){
					MoveCursor(CursorIndex+1,true);
				}else if(key==KeyCode.Backspace){
					
					// Delete the character before the cursor (or the selection, if we have one).
					
					// Got a selection?
					if(Cursor!=null && Cursor.TryDeleteSelection()){
						return;
					}
					
					if(string.IsNullOrEmpty(value) || CursorIndex==0){
						return;
					}
					
					int index=CursorIndex;
					value=value.Substring(0,index-1)+value.Substring(index,value.Length-index);
					SetValue(value);
					MoveCursor(index-1);
				}else if(key==KeyCode.Delete){
					// Delete the character after the cursor.
					
					// Got a selection?
					if(Cursor!=null && Cursor.TryDeleteSelection()){
						return;
					}
					
					if(string.IsNullOrEmpty(value)||CursorIndex==value.Length){
						return;
					}
					
					int index=CursorIndex;
					value=value.Substring(0,index)+value.Substring(index+1,value.Length-index-1);
					SetValue(value);
				}else if(key==KeyCode.Home){
					// Hop to the start:
					
					MoveCursor(0,true);
				
				}else if(key==KeyCode.End){
					// Hop to the end:
					
					int maxCursor=0;
					
					if(value!=null){
						maxCursor=value.Length;
					}
					
					MoveCursor(maxCursor,true);
				
				}else if(key==KeyCode.Return || key==KeyCode.KeypadEnter){
					
					// Add a newline
					if(value==null){
						value=""+pressEvent.character;
					}else{
						value=value.Substring(0,CursorIndex)+'\n'+value.Substring(CursorIndex,value.Length-CursorIndex);
					}
					SetValue(value);
					MoveCursor(CursorIndex+1);
					
				}
				
			}
			
			base.OnKeyPress(pressEvent);
			
		}
		
		/// <summary>Called when the element is focused.</summary>
		internal override void OnFocusEvent(FocusEvent fe){
			
			if(Cursor!=null){
				return;
			}
			
			if(innerHTML==Placeholder && Placeholder!=""){
				innerHTML="";
			}
			
			// Add a cursor.
			Cursor=Style.Computed.GetOrCreateVirtual(HtmlCursorElement.Priority,"cursor") as HtmlCursorElement;
			
		}
		
		/// <summary>Called when the element is unfocused/blurred.</summary>
		internal override void OnBlurEvent(FocusEvent fe){
			if(Cursor==null){
				return;
			}
			
			// Remove the cursor:
			Style.Computed.RemoveVirtual(HtmlCursorElement.Priority);
			Cursor=null;
			
			if(innerHTML=="" && Placeholder!=""){
				innerHTML=Placeholder;
			}
			
		}
		
		/// <summary>For text and password inputs, this relocates the cursor to the given index.</summary>
		/// <param name="index">The character index to move the cursor to, starting at 0.</param>
		public void MoveCursor(int index){
			MoveCursor(index,false);
		}
		
		/// <summary>For text and password inputs, this relocates the cursor to the given index.</summary>
		/// <param name="index">The character index to move the cursor to, starting at 0.</param>
		/// <param name="immediate">True if the cursor should be moved right now.</param>
		public void MoveCursor(int index,bool immediate){
			
			if(Cursor==null){
				return;
			}
			
			Cursor.Move(index,immediate);
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Ensure it's focused:
			focus();
			
			// Get the text content:
			HtmlTextNode htn=TextHolder;
			
			if(htn==null){
				// Index is just 0.
				return;
			}
			
			// Get the letter index:
			int index=htn.LetterIndex(clickEvent.clientX,clickEvent.clientY);
			
			// Move the cursor there (requesting a redraw):
			MoveCursor(index,true);
			
		}
		
	}
	
}