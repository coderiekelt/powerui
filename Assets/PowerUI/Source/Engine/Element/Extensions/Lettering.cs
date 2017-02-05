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
	/// A useful function which splits all letters in an element into their own individual elements.
	/// Great for animating each letter on its own. Similar to lettering.js.
	/// </summary>

	public partial class HtmlElement{
		
		public void Lettering(){
			
			if(childNodes_==null){
				return;
			}
			
			// Cache child nodes:
			NodeList kids=childNodes_;
			
			// Empty:
			empty();
			
			int count=kids.length;
			
			for(int i=0;i<count;i++){
				
				// Grab the child node:
				Node child=kids[i];
				
				// Get as text:
				HtmlTextNode text=child as HtmlTextNode;
				
				if(text==null){
					
					// Direct add:
					appendChild(child);
					
				}else{
					
					// Get the text:
					string characters=text.data;
					
					// How many chars?
					int characterCount=characters.Length;
					
					// Add each letter as a new element:
					for(int c=0;c<characterCount;c++){
						
						// The character(s) as a string:
						string charString;
						
						// Grab the character:
						char character=characters[c];
						
						// Surrogate pair?
						if(char.IsHighSurrogate(character) && c!=characters.Length-1){
							
							// Low surrogate follows:
							char lowChar=characters[c+1];
							
							c++;
							
							// Get the charcode:
							int code=char.ConvertToUtf32(character,lowChar);
							
							// Turn it back into a string:
							charString=char.ConvertFromUtf32(code);
							
						}else{
							
							charString=""+character;
							
						}
						
						// Create a new span:
						Element span=document.createElement("span");
						
						if(charString==" "){
							// NBSP:
							span.textContent="\u00A0";
						}else{
							span.textContent=charString;
						}
						
						// Add it:
						appendChild(span);
						
					}
					
				}
				
			}
			
		}
	
	}
	
}