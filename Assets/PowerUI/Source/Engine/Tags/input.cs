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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using Css;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents the input tag which handles various types of input on forms.
	/// Note that all input tags are expected to be on a form to work correctly.
	/// E.g. radio buttons won't work if they are not on a form.
	/// Supports the type, name, value and checked attributes.
	/// Also supports a 'content' attribute which accepts a value as html; great for buttons.
	/// </summary>
	
	[Dom.TagName("input")]
	public class HtmlInputElement:HtmlElement{
		
		/// <summary>Used by password inputs. True if this input's value is hidden.</summary> 
		public bool Hidden;
		/// <summary>The value text for this input.</summary>
		public string Value;
		/// <summary>For boolean (radio or checkbox) inputs, this is true if this one is checked.</summary>
		private bool Checked_;
		/// <summary>For text or password input boxes, this is the cursor.</summary>
		public HtmlCursorElement Cursor;
		/// <summary>The type of input that this is; e.g. text/password/radio etc.</summary>
		public InputType Type=InputType.Undefined;
		/// <summary>The maximum length of text in this box.</summary>
		public int MaxLength=int.MaxValue;
		/// <summary>A placeholder value.</summary>
		public string Placeholder="";
		
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
		
		
		public HtmlInputElement(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		public override void OnTagLoaded(){
			
			if(Type==InputType.Undefined){
				// Specify as text:
				this["type"]="text";
			}
			
		}
		
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
		
		/// <summary>Looks out for paste events.</summary>
		protected override bool HandleLocalEvent(DomEvent e,bool bubblePhase){
			
			// Handle locally:
			if(base.HandleLocalEvent(e,bubblePhase)){
				
				if(e is ClipboardEvent && IsTextInput() && e.type=="paste"){
					
					// Paste the data at the cursor index (must be text only).
					string textToPaste=(e as ClipboardEvent).text;
					
					if(textToPaste!=null){
						
						string value=Value;
						
						if(value==null){
							value=""+textToPaste;
						}else{
							value=value.Substring(0,CursorIndex)+textToPaste+value.Substring(CursorIndex,value.Length-CursorIndex);
						}
						
						SetValue(value);
						MoveCursor(CursorIndex+textToPaste.Length);
						
					}
					
				}
				
				return true;
				
			}
			
			return false;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.ReconstructFormatting();
				
				lexer.Push(this,false);
				
				
				string type=this["type"];
				
				if(type==null || type=="hidden"){
					
					lexer.FramesetOk=false;
					
				}
				
			}else if(mode==HtmlTreeMode.InTable){
				
				string type=this["type"];
				
				if(type==null || type=="hidden"){
					
					// Go the anything else route.
					lexer.InTableElse(this,null);
					
				}else{
					
					// Add but don't push:
					lexer.Push(this,false);
					
				}
				
			}else if(mode==HtmlTreeMode.InSelect){
				
				lexer.InputOrTextareaInSelect(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="type"){
				string type=this["type"];
				if(type==null){
					type="text";
				}
				
				if(type=="radio"){
					Type=InputType.Radio;
				}else if(type=="checkbox"){
					Type=InputType.Checkbox;
				}else if(type=="submit"){
					Type=InputType.Submit;
				}else if(type=="button"){
					Type=InputType.Button;
				}else if(type=="hidden"){
					Type=InputType.Hidden;
				}else if(type=="number"){
					Type=InputType.Number;
				}else if(type=="url"){
					Type=InputType.Url;
				}else if(type=="email"){
					Type=InputType.Email;
				}else{
					Type=InputType.Text;					
					Hidden=(type=="password");
				}
				
				return true;
			
			}else if(property=="placeholder"){
				
				Placeholder=this["placeholder"];
				innerHTML=Placeholder;
				
				return true;
			
			}else if(property=="size"){
				
				// A rough font size based width:
				int size;
				if(int.TryParse(this["size"],out size)){
					
					// Don't apply if it's a button:
					if(Type!=InputType.Submit && Type!=InputType.Button){
						style.width=(size*10)+"px";
					}
					
				}
				
			}else if(property=="maxlength"){
				
				string value=this["maxlength"];
				
				if(string.IsNullOrEmpty(value)){
					// It's blank - set it to the default.
					MaxLength=int.MaxValue;
				}else{
					// Parse the maximum length from the string:
					if(int.TryParse(value,out MaxLength)){
						// Clip the value if we need to:
						if(Value!=null && Value.Length>MaxLength){
							SetValue(Value);
						}
					}else{
						// Not a number!
						MaxLength=int.MaxValue;
					}
				}
				
				return true;
			}else if(property=="checked"){
				
				// Get the checked state:
				string state=this["checked"];
				
				// Awkwardly, null/ empty is checked.
				// 0 or false are not checked, anything else is!
				
				if( string.IsNullOrEmpty(state) ){
					
					Select();
					
				}else{
					state=state.ToLower().Trim();
					
					if(state=="0" || state=="false"){
						
						Unselect();
						
					}else{
						
						Select();
						
					}
					
				}
				
				// Refresh local selectors:
				Style.Computed.RefreshLocal(true);
				
				RequestLayout();
				return true;
			}else if(property=="value"){
				SetValue(this["value"]);
				return true;
			}else if(property=="content"){
				SetValue(this["content"],true);
				return true;
			}
			return false;
		}
		
		public override KeyboardMode OnShowMobileKeyboard(){
			if(!IsTextInput()){
				return null;
			}
			
			KeyboardMode result=new KeyboardMode();
			result.Secret=Hidden;
			
			#if MOBILE
			
			if(Type==InputType.Number){
				
				// Number keyboard:
				result.Type=TouchScreenKeyboardType.NumbersAndPunctuation;
				
			}else if(Type==InputType.Url){
				
				// Url keyboard:
				result.Type=TouchScreenKeyboardType.URL;
			
			}else if(Type==InputType.Email){
				
				// Email keyboard:
				result.Type=TouchScreenKeyboardType.EmailAddress;
				
			}
			
			#endif
			
			return result;
		}
		
		/// <summary>Used by boolean inputs (checkbox/radio). Unselects this from being the active one.</summary>
		public void Unselect(){
			if(!Checked_){
				return;
			}
			
			Checked_=false;
			
			// Clear checked:
			this["checked"]="0";
			
			if(Type==InputType.Checkbox){
				SetValue(null);
			}
			
		}
		
		/// <summary>Used by boolean inputs (checkbox/radio). Selects this as the active one.</summary>
		public void Select(){
			if(Checked_){
				return;
			}
			Checked_=true;
			
			// Set checked:
			this["checked"]="1";
			
			if(Type==InputType.Radio){
				// Firstly, unselect all other radio elements with this same name:
				string name=this["name"];
				
				HTMLCollection allInputs;
				
				if(form==null){
					
					// Find all inputs with the same name:
					allInputs=document.getElementsByTagName("input");
					
				}else{
					
					allInputs=form.GetAllInputs();
					
				}
				
				if(allInputs!=null){
					
					foreach(Element element in allInputs){
						
						if(element==this){
							// Skip this element
							continue;
						}
						
						if(element["type"]=="radio" && element["name"]==name){
							// Yep; unselect it.
							((HtmlInputElement)element).Unselect();
						}
						
					}
					
				}
				
				DomEvent e=new DomEvent("change");
				e.SetTrusted(true);
				dispatchEvent(e);
				
			}else if(Type==InputType.Checkbox){
				SetValue("1");
			}
			
		}
		
		/// <summary>Sets the value of this input box.</summary>
		/// <param name="value">The value to set.</param>
		public void SetValue(string value){
			SetValue(value,false);
		}
		
		/// <summary>Sets the value of this input box, optionally as a html string.</summary>
		/// <param name="value">The value to set.</param>
		/// <param name="html">True if the value can safely contain html.</param>
		public void SetValue(string value,bool html){
			
			// Trigger onchange:
			DomEvent e=new DomEvent("change");
			e.SetTrusted(false);
			if(!dispatchEvent(e)){
				return;
			}
			
			if(MaxLength!=int.MaxValue){
				// Do we need to clip it?
				if(value!=null && value.Length>MaxLength){
					// Yep!
					value=value.Substring(0,MaxLength);
				}
			}
			
			if(value==null || CursorIndex>value.Length){
				MoveCursor(0);
			}
			
			// Update the value:
			this["value"]=Value=value;
			
			if(!IsBoolInput()){
				if(Hidden){
					// Unfortunately the new string(char,length); constructor isn't reliable.
					// Build the string manually here.
					StringBuilder sb=new StringBuilder("",value.Length);
					for(int i=0;i<value.Length;i++){
						sb.Append('*');
					}
					
					if(html){
						innerHTML=sb.ToString();
					}else{
						textContent=sb.ToString();
					}
				}else{
					if(html){
						innerHTML=value;
					}else{
						textContent=value;
					}
				}
			}
			
		}
		
		/// <summary>Checks if this is a radio or checkbox input box.</summary>
		/// <returns>True if it is; false otherwise.</returns>
		private bool IsBoolInput(){
			return (Type==InputType.Radio||Type==InputType.Checkbox);
		}
		
		/// <summary>Checks if this is a text, number or password input box.</summary>
		/// <returns>True if it is; false otherwise.</returns>
		private bool IsTextInput(){
			return (Type==InputType.Text || Type==InputType.Number || Type==InputType.Url || Type==InputType.Email);
		}
		
		public override void OnKeyPress(KeyboardEvent pressEvent){
			
			if(this["readonly"]!=null){
				return;
			}
			
			if(Type==InputType.Number){
				
				if( !char.IsNumber(pressEvent.character) && pressEvent.character!='.' && !char.IsControl(pressEvent.character) ){
					
					// Not a number, point or control character. Block it:
					return;
					
				}
				
			}
			
			if(pressEvent.heldDown){
				if(IsTextInput()){
					// Add to value if pwd/text, unless it's backspace:
					string value=Value;
					
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
					
					// Grab the keycode:
					KeyCode key=pressEvent.unityKeyCode;
					
					if(key==KeyCode.LeftArrow){
						MoveCursor(CursorIndex-1,true);
					}else if(key==KeyCode.RightArrow){
						MoveCursor(CursorIndex+1,true);
					}else if(key==KeyCode.Backspace){
						// Delete the character before the cursor.
						
						// Got a selection?
						if(Cursor!=null && Cursor.TryDeleteSelection()){
							return;
						}
						
						if(string.IsNullOrEmpty(value)||CursorIndex==0){
							return;
						}
						value=value.Substring(0,CursorIndex-1)+value.Substring(CursorIndex,value.Length-CursorIndex);
						int index=CursorIndex;
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
						
						value=value.Substring(0,CursorIndex)+value.Substring(CursorIndex+1,value.Length-CursorIndex-1);
						SetValue(value);
					}else if(key==KeyCode.Return || key==KeyCode.KeypadEnter){
						// Does the form have a submit button? If so, submit now.
						// Also call a convenience (non-standard) "onenter" method.
						
						HtmlFormElement f=form;
						if(f!=null && f.HasSubmitButton){
							f.submit();
						}
						
						return;
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
						
					}
					
				}else if(Type==InputType.Submit){
					
					// Submit button.
					if(!char.IsControl(pressEvent.character) && pressEvent.character!='\0'){
						return;
					}
					
					// Grab the keycode:
					KeyCode key=pressEvent.unityKeyCode;
					
					if(key==KeyCode.Return || key==KeyCode.KeypadEnter){
						
						// Find the form and then attempt to submit it.
						HtmlFormElement f=form;
						
						if(f!=null){
							f.submit();
						}
						
					}
					
				}
				
			}
			
			base.OnKeyPress(pressEvent);
			
		}
		
		/// <summary>Called when the element is focused.</summary>
		internal override void OnFocusEvent(FocusEvent fe){
			if(!IsTextInput()||Cursor!=null){
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
		
			// Focus it:
			focus();
			
			if(Type==InputType.Submit){
				// Find the form and then attempt to submit it.
				HtmlFormElement f=form;
				if(f!=null){
					f.submit();
				}
			}else if(Type==InputType.Radio){
				Select();
			}else if(Type==InputType.Checkbox){
				
				if(Checked_){
					Unselect();
				}else{
					Select();
				}
				
			}else if(IsTextInput()){
				
				// Move the cursor to the clicked point.
				
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
	
	
	public partial class HtmlElement{
		
		
		/// <summary>Gets or sets the value of this element. Input/Select/Textarea elements only.</summary>
		public virtual string value{
			get{
				return this["value"];
			}
			set{
				this["value"]=value;
			}
		}
		
		/// <summary>Gets or sets the value as html for this element. Input/Select elements only.</summary>
		public virtual string htmlValue{
			get{
				return this["content"];
			}
			set{
				this["content"]=value;
			}
		}
		
	}
	
}