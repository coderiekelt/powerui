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
using Dom;

namespace PowerUI{
	
	/// <summary>
	/// Represents a html form which lets you collect information from the player.
	/// For those new to html, see input and select tags.
	/// Supports onsubmit="nitroMethodName" and the action attributes.
	/// </summary>
	
	[Dom.TagName("form")]
	public class HtmlFormElement:HtmlElement{
		
		/// <summary>The url to post the form to.</summary>
		public string Action;
		
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InTable){
				
				if(lexer.TagCurrentlyOpen("template") || lexer.form!=null){
					
					// Ignore it.
					
				}else{
					
					// Add but don't push:
					lexer.Push(this,false);
					lexer.form=this;
					
				}
				
			}else if(mode==HtmlTreeMode.InBody){
				
				bool openTemplate=lexer.TagCurrentlyOpen("template");
				
				if(lexer.form!=null && !openTemplate){
					
					// Parse error - ignore the token.
					
				}else{
					
					lexer.CloseParagraphButtonScope();
					
					// Add and set form:
					lexer.Push(this,true);
					
					if(!openTemplate){
						lexer.form=this;
					}
					
				}
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				if(lexer.TagCurrentlyOpen("template")){
					
					// Template in scope.
					lexer.GenerateImpliedEndTags();
					
					lexer.CloseInclusive("form");
					
				}else if(lexer.IsInScope("form")){
					
					// No template - ordinary form.
					Element node=lexer.form;
					lexer.form=null;
					
					if(node!=null && lexer.IsInScope("form")){
						
						lexer.GenerateImpliedEndTags();
						
						if(node==lexer.CurrentElement){
							
							lexer.CloseCurrentNode();
							
						}else{
							
							// Fatal parse error.
							throw new DOMException(DOMException.SYNTAX_ERR, (ushort)HtmlParseError.FormClosedWrong);
							
						}
						
					}
					
				}
				
				// Ignore otherwise
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="onsubmit"){
				return true;
			}else if(property=="action"){
				Action=this["action"];
				return true;
			}
			
			return false;
		}
		
		/// <summary>Gets all input elements contained within this form.</summary>
		/// <returns>A list of all input elements.</returns>
		public HTMLCollection GetAllInputs(){
			HTMLCollection results=new HTMLCollection();
			GetAllInputs(results,this);
			return results;
		}
		
		/// <summary>Gets all inputs from the given element, adding the results to the given list.</summary>
		/// <param name="results">The list that all results are added to.</param>
		/// <param name="element">The element to check.</param>
		private void GetAllInputs(INodeList results,HtmlElement element){
			
			NodeList kids=element.childNodes_;
			
			if(kids==null){
				return;
			}
			
			for(int i=0;i<kids.length;i++){
				HtmlElement child=kids[i] as HtmlElement ;
				
				if(child==null){
					continue;
				}
				
				if(child.Tag=="input"||child.Tag=="select"||child.Tag=="textarea"){
					results.push(child);
				}else{
					GetAllInputs(results,child);
				}
				
			}
		}
		
		/// <summary>Gets all input elements contained within this form.</summary>
		public override HTMLCollection elements{
			get{
				return GetAllInputs();
			}
		}
		
		/// <summary>Gets the selected input by the given name attribute.
		/// E.g. there may be more than one input element (such as with radios); this is the active one.</summary>
		public HtmlElement getField(string name){
			
			NodeList allWithName=getElementsByAttribute("name",name);
			if(allWithName.length==0){
				return null;
			}
			
			HtmlInputElement tag=allWithName[0] as HtmlInputElement;
			
			if(allWithName.length==1){
				return tag;
			}
			
			// We have more than one. If it's a radio, return the one which is selected.
			// Otherwise, return the last one.
			
			if(tag==null){
				return null;
			}
			
			if(tag.Type==InputType.Radio){
				
				// Which is selected?
				foreach(HtmlElement radio in allWithName){
					if(((HtmlInputElement)radio).Checked){
						return radio;
					}
				}
			}
			
			return allWithName[allWithName.length-1] as HtmlElement ;
		}
		
		/// <summary>True if this form has a submit button within it.</summary>
		public bool HasSubmitButton{
			get{
				// Get all inputs:
				HTMLCollection allInputs=getElementsByTagName("input");
				
				// Are any a submit?
				foreach(Element element in allInputs){
					if(element["type"]=="submit"){
						return true;
					}
				}
				
				return false;
			}
		}
		
		/// <summary>Submits this form.</summary>
		public override void submit(){
			
			// Generate a nice dictionary of the form contents.
			
			// Step 1: find the unique names of the elements:
			Dictionary<string,string> uniqueValues=new Dictionary<string,string>();
			
			HTMLCollection allInputs=GetAllInputs();
			
			foreach(Element element in allInputs){
				string type=element["type"];
				if(type=="submit"||type=="button"){
					// Don't want buttons in here.
					continue;
				}
				
				string name=element["name"];
				if(name==null){
					name="";
				}
				
				// Step 2: For each one, get their value.
				// We might have a name repeated, in which case we check if they are radio boxes.
				
				if(uniqueValues.ContainsKey(name)){
					// Ok the element is already added - we have two with the same name.
					// If they are radio, then only overwrite value if the new addition is checked.
					// Otherwise, overwrite - furthest down takes priority.
					if(element.Tag=="input"){
						
						HtmlInputElement tag=(HtmlInputElement)element;
						
						if(tag.Type==InputType.Radio&&!tag.Checked){
							// Don't overwrite.
							continue;
						}
					}
				}
				string value=(element as HtmlElement ).value;
				if(value==null){
					value="";
				}
				uniqueValues[name]=value;
			}
			
			FormEvent formData=new FormEvent(uniqueValues);
			formData.SetTrusted(false);
			formData.EventType="submit";
			// Hook up the form element:
			formData.form=this;
			
			if( dispatchEvent(formData) ){
				
				// Get ready to post now!
				DataPackage package=new DataPackage(Action,document.basepath);
				package.AttachForm(formData.ToUnityForm());
				
				// Apply request to the data:
				formData.request=package;
				
				// Apply load event:
				package.onload=package.onerror=delegate(UIEvent e){
					
					// Attempt to run ondone (doesn't bubble):
					formData.Reset();
					formData.EventType="done";
					
					if(dispatchEvent(formData)){
						// Otherwise the ondone function quit the event.
						
						// Load the result into target now.
						HtmlDocument targetDocument=ResolveTarget();
						
						if(targetDocument==null){
							// Posting a form to an external target.
							
							Log.Add("Warning: Unity cannot post forms to external targets. It will be loaded a second time.");
							
							// Open the URL outside of Unity:
							UnityEngine.Application.OpenURL(package.location.absoluteNoHash);
							
						}else{
							
							// Change the location:
							targetDocument.SetRawLocation(package.location);
							
							// History entry required:
							targetDocument.window.history.DocumentNavigated();
							
							// Apply document content:
							targetDocument.GotDocumentContent(package.responseText,package.statusCode,true);
							
						}
						
					}
					
				};
				
				// Send now!
				package.send();
				
			}
		}
		
	}
	
	
	public partial class HtmlElement{
		
		
		/// <summary>Internal use only. <see cref="PowerUI.HtmlElement.formElement"/>.
		/// Scans up the DOM to find the parent form element.</summary>
		/// <returns>The parent form element, if found.</returns>
		public HtmlElement GetForm(){
			
			if(this is HtmlFormElement){
				return this;
			}
			
			HtmlElement parent=parentNode_ as HtmlElement;
			
			if(parent==null){
				return null;
			}
			
			return parent.GetForm();
		}
		
		/// <summary>Submits the form this element is in.</summary>
		public virtual void submit(){
		}
		
		/// <summary>Scans up the DOM to find the parent form element.
		/// Note: <see cref="PowerUI.HtmlElement.form"/> may be more useful than the element iself.</summary>
		public HtmlFormElement formElement{
			get{
				return form;
			}
		}
		
		/// <summary>Scans up the DOM to find the parent form element's handler.
		/// The object returned provides useful methods such as <see cref="PowerUI.HtmlFormElement.submit"/>. </summary>
		public HtmlFormElement form{
			get{
				return GetForm() as HtmlFormElement;
			}
		}
		
		/// <summary>Gets all input elements contained within this form.</summary>
		public virtual HTMLCollection elements{
			get{
				return null;
			}
		}
		
	}
	
}