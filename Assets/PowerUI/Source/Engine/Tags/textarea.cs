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
		public HtmlElement Cursor;
		/// <summary>The current location of the cursor.</summary>
		public int CursorIndex;
		/// <summary>True if the cursor should be located after the next update.</summary>
		public bool LocateCursor;
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
		private void SetValue(string value){
			SetValue(value,false);
		}
		
		/// <summary>Sets the value of this textarea, optionally as a html string.</summary>
		/// <param name="value">The value to set.</param>
		/// <param name="html">True if the value can safely contain html.</param>
		private void SetValue(string value,bool html){
			
			DomEvent e=new DomEvent("change");
			e.SetTrusted(false);
			if(!dispatchEvent(e)){
				return;
			}
			
			if(MaxLength!=int.MaxValue){
				// Do we need to clip it?
				if(value.Length>MaxLength){
					// Yep!
					value=value.Substring(0,MaxLength);
				}
			}
			
			if(CursorIndex>value.Length){
				MoveCursor(0);
			}
			
			if(value=="" && Placeholder!=""){
				value=Placeholder;
			}
			
			if(html){
				innerHTML=value;
			}else{
				innerHTML=Dom.Text.Escape(value).Replace("\n","<br>");
			}
			
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
					// Delete the character before the cursor.
					if(string.IsNullOrEmpty(value)||CursorIndex==0){
						return;
					}
					value=value.Substring(0,CursorIndex-1)+value.Substring(CursorIndex,value.Length-CursorIndex);
					int index=CursorIndex;
					SetValue(value);
					MoveCursor(index-1);
				}else if(key==KeyCode.Delete){
					// Delete the character after the cursor.
					if(string.IsNullOrEmpty(value)||CursorIndex==value.Length){
						return;
					}
					value=value.Substring(0,CursorIndex)+value.Substring(CursorIndex+1,value.Length-CursorIndex-1);
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
		
		/// <summary>For text and password inputs, this relocates the cursor to the given index.</summary>
		/// <param name="index">The character index to move the cursor to, starting at 0.</param>
		public void MoveCursor(int index){
			MoveCursor(index,false);
		}
		
		/// <summary>For text and password inputs, this relocates the cursor to the given index.</summary>
		/// <param name="index">The character index to move the cursor to, starting at 0.</param>
		/// <param name="immediate">True if the cursor should be moved right now.</param>
		public void MoveCursor(int index,bool immediate){
			
			if(index<0){
				index=0;
			}
			
			if(index==CursorIndex){
				return;
			}
			
			CursorIndex=index;
			
			if(Cursor==null){
				return;
			}
			
			LocateCursor=true;

			if(immediate){
				// We have enough info to place the cursor already.
				// Request a layout.
				RequestLayout();
			}
			
			// Otherwise locating the cursor is delayed until after the new value has been rendered.
			// This is used immediately after we set the value.
		}
		
		/// <summary>Positions the cursor immediately.</summary>
		private void LocateCursorNow(){
			LocateCursor=false;
			// Position the cursor - we need to locate the letter's exact position first:
			// ..Which will be in the text element:
			Vector2 position=Vector2.zero;
			
			int count=childCount;
			
			if(count>1){
				// Note: If it's equal to 1, ele[0] is the cursor.
				int index=CursorIndex;
				
				for(int i=0;i<count;i++){
					
					if(index>0){
						
						// Grab child element:
						Node child=childNodes_[i];
						
						// Get as text ele:
						HtmlTextNode text=child as HtmlTextNode;
						
						if(text==null){
							// Possibly a br.
							HtmlElement ele=child as HtmlElement ;
							
							if(ele!=null && ele.Tag=="br"){
								
								index--;
								
								// Get the br's computed style:
								ComputedStyle computed=ele.Style.Computed;
								
								// Coords are..
								float left=computed.FirstBox.ParentOffsetLeft;
								float top=computed.FirstBox.ParentOffsetTop;
								
								// Create position:
								position=new Vector2(left,top);
								
							}
							
						}else{
							position=text.GetPosition(ref index);
						}
						
					}else{
						break;
					}
					
				}
			}
			
			// Set it in:
			Cursor.Style.Computed.ChangeTagProperty("left",position.x+"px");
			Cursor.Style.Computed.ChangeTagProperty("top",(position.y-Style.Computed.FirstBox.Scroll.Top)+"px");
			
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
			// ::-spark-input-placeholder
			Cursor=document.createElement("div") as HtmlElement ;
			Cursor.className="cursor-textarea";
			appendChild(Cursor);
			CursorIndex=0;
		}
		
		/// <summary>Called when the element is unfocused/blurred.</summary>
		internal override void OnBlurEvent(FocusEvent fe){
			if(Cursor==null){
				return;
			}
			
			// Remove the cursor:
			Cursor.parentNode.removeChild(Cursor);
			Cursor=null;
			
			if(innerHTML=="" && Placeholder!=""){
				innerHTML=Placeholder;
			}
			
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			focus();
			
			// Move the cursor to the click point:
			float localClickX=clickEvent.clientX-Style.Computed.OffsetLeft + Style.Computed.ScrollLeft;
			float localClickY=clickEvent.clientY-Style.Computed.OffsetTop + Style.Computed.ScrollTop;
			
			int index=0;
			
			int count=childCount;
			
			if(count>1){
				// Note: If it's equal to 1, ele[0] is the cursor.
				
				for(int i=0;i<count;i++){
					
					// Grab child element:
					Node child=childNodes_[i];
					
					// Get as text ele:
					HtmlTextNode text=child as HtmlTextNode;
					
					if(text==null){
						// Possibly a br.
						HtmlElement ele=child as HtmlElement ;
						
						if(ele!=null && ele.Tag=="br"){
							
							// Bump up index only if this br is above localClickY.
							ComputedStyle computed=ele.Style.Computed;
							
							// Get the top of the word:
							float line=computed.FirstBox.ParentOffsetTop;
			
							if(localClickY>=line){
								
								// Include the BR:
								index++;
							
							}
							
						}
						
					}else{
						index+=text.LetterIndex(localClickX,localClickY);
					}
					
				}
				
			}
			
			MoveCursor(index,true);
			
		}
		
	}
	
}