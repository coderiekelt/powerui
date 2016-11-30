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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Css;
using Dom;
using Nitro;


namespace PowerUI{
	
	/// <summary>
	/// This delegate is used for hooking up c# methods with mouse and keyboard events.
	/// Note that in general Nitro is best used for handling these.
	/// </summary>
	public delegate void OnDomEvent(UIEvent uiEvent);
	
	/// <summary>
	/// This represents a html element in the DOM.
	/// </summary>
	[HtmlNamespace]
	[Dom.TagName("Default")]
	public partial class HtmlElement:Element, HtmlNode, IRenderableNode{
		
		/// <summary>Internal use only. The style of this element. Use <see cref="PowerUI.HtmlElement.style"/> instead.</summary>
		public ElementStyle Style;
		/// <summary>Internal use only. Children being rendered are set here. This allows multiple threads to access the DOM.</summary>
		public NodeList KidsToRender;
		/// <summary>This is true if the childNodes are being rebuilt. True for a tiny amount of time, but prevents collisions with the renderer thread.</summary>
		public bool IsRebuildingChildren;
		
		
		public HtmlElement(){
			Style=new ElementStyle(this);
		}
		
		/// <summary>The ownerDocument as a Html document.</summary>
		public HtmlDocument htmlDocument{
			get{
				return document_ as HtmlDocument;
			}
		}
		
		/// <summary>This nodes computed style.</summary>
		public ComputedStyle ComputedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>This nodes render data.</summary>
		public RenderableData RenderData{
			get{
				return Style.Computed.RenderData;
			}
		}
		
		/// <summary>Called by some tags when their content is loaded. E.g. img tag or iframe.</summary>
		/// <param name="e">The load event.</param>
		public virtual void OnLoadEvent(DomEvent e){
		}
		
		public override void OnChildrenLoaded(){
			
			// Construct the selector structure now:
			Style.Computed.RefreshStructure();
			
		}
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.</summary>
		public void GetWidthBounds(out float min,out float max){
			
			min=0f;
			max=0f;
			
			// For each child, get its width bounds too.
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				
				float cMin;
				float cMax;
				IRenderableNode renderable=(childNodes_[i] as IRenderableNode);
				
				if(renderable==null){
					continue;
				}
				
				renderable.GetWidthBounds(out cMin,out cMax);
				
				if(cMin>min){
					min=cMin;
				}
				
				if(cMax>max){
					max=cMax;
				}
				
			}
			
		}
		
		/// <summary>Focuses this element so it receives events such as keypresses.</summary>
		public void focus(){
			
			HtmlDocument doc=htmlDocument;
			
			if(doc.activeElement==this){
				return;
			}
			
			FocusEvent fe=new FocusEvent("blur");
			// It's trusted but doesn't bubble:
			fe.SetTrusted(false);
			fe.focusing=this;
			PowerUI.Input.LastFocusedDocument=doc;
			
			if(doc.activeElement!=null){
				(doc.activeElement as HtmlElement).Unfocus(fe);
			}
			
			doc.activeElement=this;
			
			#if MOBILE
			// Should we pop up the mobile keyboard?
			KeyboardMode mobile=OnShowMobileKeyboard();
			if(Input.HandleKeyboard(mobile)){
				Input.KeyboardText=value;
			}
			
			#endif
			
			// Reset so we can recycle it:
			fe.Reset();
			fe.EventType="focus";
			
			// Dispatch and check if it got cancelled:
			if(dispatchEvent(fe)){
				
				// Run the default for this element:
				OnFocusEvent(fe);
				
			}
			
		}
		
		/// <summary>Unfocuses this element.</summary>
		public void blur(){
			FocusEvent fe=new FocusEvent("blur");
			fe.SetTrusted(false);
			Unfocus(fe);
		}
		
		/// <summary>Unfocuses this element so it will no longer receive events like keypresses.</summary>
		private void Unfocus(FocusEvent fe){
			
			HtmlDocument doc=htmlDocument;
			
			if(doc.activeElement!=this){
				return;
			}
			
			doc.activeElement=null;
			
			#if MOBILE
			// Attempt to hide the keyboard.
			Input.HandleKeyboard(null);
			#endif
			
			htmlDocument.window.Event=null;
			
			if(dispatchEvent(fe)){
				
				// Run the default:
				OnBlurEvent(fe);
				
			}
			
		}
		
		/// <summary>Called when a default scrollwheel event occurs.</summary>
		/// <param name="clickEvent">The event that represents the wheel scroll.</param>
		public virtual void OnWheelEvent(WheelEvent e){
			
			#warning scrollwheel scroll.
			// Got scrollbars or is a scrollbar? Move them.
			
		}
		
		/// <summary>Called when a default key press occurs.</summary>
		/// <param name="clickEvent">The event that represents the key press.</param>
		public virtual void OnKeyPress(KeyboardEvent pressEvent){
			
		}
		
		/// <summary>True if this element has some form of background applied to it.</summary>
		public bool HasBackground{
			get{
				return RenderData.HasBackground;
			}
		}
		
		/// <summary>Gets the first element which matches the given selector.</summary>
		public Element querySelector(string selector){
			HTMLCollection results=querySelectorAll(selector,true);
			
			if(results==null || results.length==0){
				return null;
			}
			
			return results[0] as Element;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector){
			return querySelectorAll(selector,false);
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector,bool one){
		
			// Create results set:
			HTMLCollection results=new HTMLCollection();
			
			if(string.IsNullOrEmpty(selector)){
				// Empty set:
				return results;
			}
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selector,this);
			
			// Read a value:
			Css.Value value=lexer.ReadValue();
			
			// Read the selectors from the value:
			List<Selector> selectors=new List<Selector>();
			Css.CssLexer.ReadSelectors(null,value,selectors);
			
			// Create a blank event to store the targets, if any:
			CssEvent e=new CssEvent();
			
			// Perform the selection process:
			querySelectorAll(selectors.ToArray(),results,e,false);
			
			return results;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selectors">The selectors to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public void querySelectorAll(Selector[] selectors,INodeList results,CssEvent e,bool one){
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				Node node=childNodes_[i];
				IRenderableNode child=node as IRenderableNode;
				
				if(child==null){
					continue;
				}
				
				RenderableData renderData=child.RenderData;
				
				for(int s=0;s<selectors.Length;s++){
					
					// Match?
					if(selectors[s].Test(renderData,e)!=null){
						// Yep!
						results.push(node);
						
						if(one){
							return;
						}
					}
					
				}
				
				child.querySelectorAll(selectors,results,e,one);
				
				if(one && results.length==1){
					return;
				}
			}
			
		}
		
		/// <summary>Inserts html before this element.</summary>
		public void before(string html){
			
			HtmlElement parent=parentElement as HtmlElement ;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Insert:
			parent.insertInnerHTML(childIndex,html);
			
		}
		
		/// <summary>Inserts html after this element.</summary>
		public void after(string html){
			
			HtmlElement parent=parentElement as HtmlElement ;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Insert:
			parent.insertInnerHTML(childIndex+1,html);
			
		}
		
		/// <summary>Inserts HTML at the given position name.</summary>
		public void insertAdjacentHTML(string positionName,string html){
			
			positionName=positionName.ToLower().Trim();
			
			switch(positionName){
				
				case "beforebegin":
					// Before this element
					before(html);
				break;
				case "afterbegin":
					// New first child
					prependInnerHTML(html);
				break;
				case "beforeend":
					// New last child
					appendInnerHTML(html);
				break;
				case "afterend":
					// Immediately after
					after(html);
				break;
			}
			
		}
		
		internal override void ResetVariable(string name){
			OnResetAllVariables();
			base.ResetVariable(name);
		}
		
		/// <summary>Requests all child elements to reload their &variables; if they have any.</summary>
		internal override void ResetAllVariables(){
			OnResetAllVariables();
			base.ResetAllVariables();
		}
		
		/// <summary>The html of this element including the element itself.</summary>
		public string outerHTML{
			get{
				return ToString();
			}
			set{
				
				HtmlElement parent=parentElement as HtmlElement ;
				
				if(parent==null){
					throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				// Get child index:
				int index=childIndex;
				
				// Remove this element:
				parent.removeChild(this);
				
				// Insert at index:
				parent.insertInnerHTML(index,value);
				
			}
		}
		
		public override string ToString(){
			return "[object "+GetType().Name+"]";
		}
		
		/// <summary>Appends the given literal text to the content of this element.
		/// This is good for preventing html injection as the text will be taken literally.</summary>
		/// <param name="text">The literal text to append.</param>
		public void appendTextContent(string text){
			
			if(string.IsNullOrEmpty(text)){
				return;
			}
			
			// Parse now:
			HtmlLexer lexer=new HtmlLexer(text,this);
			
			// Plaintext state:
			lexer.State=HtmlParseMode.Plaintext;
			
			// Ok!
			lexer.Parse();
			
		}
		
		/// <summary>Appends the given html text to the content of this element.</summary>
		/// <param name="text">The html text to append.</param>
		public void append(string html){
			if(string.IsNullOrEmpty(html)){
				return;
			}
			
			// Parse now:
			HtmlLexer lexer=new HtmlLexer(html,this);
			
			// PCData is fine here.
			
			// Ok!
			lexer.Parse();
			
		}
		
		/// <summary>Appends the given child element to the content of this element.</summary>
		/// <param name="child">The child to append.</param>
		public void append(HtmlElement child){
			if(child==null){
				return;
			}
			appendChild(child);
		}
		
		/// <summary>Appends the given html text to the content of this element.</summary>
		/// <param name="text">The html text to append.</param>
		public void appendInnerHTML(string text){
			append(text);
		}
		
		/// <summary>Inserts HTML into this element at the given index. Pushes any elements at the given index over.</summary>
		public void insertInnerHTML(int index,string text){
			if(string.IsNullOrEmpty(text)){
				return;
			}
			
			// Cache child nodes:
			NodeList children=childNodes_;
			
			int childCount=children==null ? 0 : children.length;
			
			if(index>=childCount){
				// Append:
				appendInnerHTML(text);
				return;
			}
			
			// Create new nodes set:
			childNodes_=new NodeList();
			
			// Transfer up to but not including index:
			if(children!=null){
				
				for(int i=0;i<index;i++){
					
					childNodes_.push(children[i]);
					
				}
				
			}
			
			// Add to end:
			appendInnerHTML(text);
			
			if(children==null){
				return;
			}
			
			// Append the remaining nodes:
			for(int i=index;i<childCount;i++){
				childNodes_.push(children[i]);
			}
			
		}
		
		/// <summary>Prepends the given child element to the content of this element, adding it as the first child.</summary>
		/// <param name="child">The child to prepend.</param>
		public void prepend(HtmlElement child){
			// Insert at #0:
			insertChild(0,child);
		}
		
		/// <summary>Prepends the given html text to the content of this element, adding it as the first child.</summary>
		/// <param name="text">The html text to prepend.</param>
		public void prepend(string text){
			// Insert at #0:
			insertInnerHTML(0,text);
		}
		
		/// <summary>Prepends the given html text to the content of this element, adding it as the first child.</summary>
		/// <param name="text">The html text to prepend.</param>
		public void prependInnerHTML(string text){
			// Insert at #0:
			insertInnerHTML(0,text);
		}
		
		/// <summary>Gets or sets the innerHTML of this element.</summary>
		public virtual string innerHTML{
			get{
				if(htmlDocument.AotDocument){
					return "";
				}
				System.Text.StringBuilder result=new System.Text.StringBuilder();
				
				if(childNodes_!=null){
					
					for(int i=0;i<childNodes_.length;i++){
						childNodes_[i].ToString(result);
					}
					
				}
				
				return result.ToString();
			}
			set{
				
				IsRebuildingChildren=true;
				
				if(childNodes_!=null){
					empty();
				}
				
				if(!string.IsNullOrEmpty(value)){
					appendInnerHTML(value);
				}
				
				IsRebuildingChildren=false;
				
			}
		}
		
		/// <summary>Called when the DOM changed.</summary>
		internal override void ChangedDOM(){
			
			// Request a layout:
			htmlDocument.RequestLayout();
			
		}
		
		/// <summary>Called when this element got added to the DOM.</summary>
		internal override void AddedToDOM(){
			
			HtmlDocument doc=htmlDocument;
			
			if(doc!=null){
				
				if(doc.AttributeIndex!=null){
					// Index element if needed:
					AddToAttributeLookups();
				}
				
			}
			
			// Reload its style:
			Style.Computed.RefreshStructure();
			
			// Update its css by telling it the parent changed.
			// This affects inherit, height/width etc.
			Style.Computed.ParentChanged();
			
			if(doc!=null){
				// Request a layout:
				doc.RequestLayout();
			}
			
		}
		
		/// <summary>Called when this element got removed from the DOM.</summary>
		internal override void RemovedFromDOM(){
			
			if(htmlDocument.AttributeIndex!=null){
				
				// Remove this from the DOM attribute cache:
				htmlDocument.RemoveCachedElement(this);
				
			}
			
			// Remove handler:
			OnRemovedFromDOM();
			
			// Let the style know we went offscreen:
			RenderableData renderable=RenderData;
			renderable.WentOffScreen();
			
			// Apply to all virtual elements:
			VirtualElements virts=renderable.Virtuals;
			
			if(virts!=null){
			
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
				
					// Remove it:
					kvp.Value.RemovedFromDOM();
					
				}
				
			}
			
			base.RemovedFromDOM();
			
			// Request a layout:
			htmlDocument.RequestLayout();
			
		}
		
		/// <summary>Called when this element goes offscreen.</summary>
		public void WentOffScreen(){
			
			RenderableData renderable=RenderData;
			renderable.WentOffScreen();
			
			// Apply to all virtual elements:
			VirtualElements virts=renderable.Virtuals;
			
			if(virts!=null){
			
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
				
					// Tell it that it's gone offscreen:
					(kvp.Value as HtmlElement ).WentOffScreen();
					
				}
				
			}
			
			if(childNodes_!=null){
				
				for(int i=0;i<childNodes_.length;i++){
					
					// Get as a HTML node:
					HtmlNode htmlNode=(childNodes_[i] as HtmlNode);
					
					if(htmlNode==null){
						return;
					}
					
					// Call offscreen:
					htmlNode.WentOffScreen();
					
				}
				
			}
			
		}
		
		/// <summary>Appends the given element defined as text.</summary>
		/// <param name="html">The html to append, e.g. "<div id='someNewElement'></div>".</param>
		/// <returns>The newly created element.</returns>
		public void appendChild(string html){
			append(html);
		}
		
		/// <summary>Replaces this element with the given html.</summary>
		public void replaceWith(string html){
			
			// Insert html before this:
			before(html);
			
			// remove this:
			remove();
			
		}
		
		/// <summary>Applies to iframes. The document contained in the iframe itself.</summary>
		public HtmlDocument contentDocument{
			get{
				return firstChild as HtmlDocument;
			}
		}
		
		/// <summary>Scrolls the element by the given values.</summary>
		/// <param name="x">The change in x pixels.</param>
		/// <param name="y">The change in y pixels.</param>
		public void scrollBy(float x,float y){
			if(x==0f && y==0f){
				return;
			}
			
			// Get the scroll value (if there is one):
			Css.Value scroll=Style.Computed.Scroll;
			
			if(scroll==null){
				scrollTo(x,y);
				return;
			}
			
			float top=(scroll[0]==null) ? 0f : scroll[0].GetDecimal(RenderData,null);
			float left=(scroll[1]==null) ? 0f : scroll[1].GetDecimal(RenderData,null);
			
			scrollTo(left+x,top+y);
			
		}
		
		/// <summary>Scrolls the element to the given exact values.</summary>
		/// <param name="x">The x offset in pixels.</param>
		/// <param name="y">The y offset in pixels.</param>
		public void scrollTo(float x,float y){
			
			// Straight through to CSS:
			Style.scrollLeft=x+"px";
			Style.scrollTop=y+"px";
			
		}
		
		/// <summary>Forces a layout to occur if one is required.
		/// You should almost never need to call this directly - it's only needed if you want to read the fully
		/// computed size of an element immediately after having updated it's style.</summary>
		public void RequireLayout(){
			htmlDocument.Renderer.Layout();
		}
		
		/// <summary>The amount of pixels the content of this element is scrolled horizontally.</summary>
		public int scrollLeft{
			get{
				RequireLayout();
				
				// Get the scroll value (if there is one):
				Css.Value scroll=Style.Computed.Scroll;
				
				if(scroll==null || scroll[1]==null){
					return 0;
				}
				
				return scroll[1].GetInteger(RenderData,null);
				
			}
			set{
				scrollTo(value,scrollTop);
			}
		}
		
		/// <summary>The amount of pixels the content of this element is scrolled vertically.</summary>
		public int scrollTop{
			get{
				RequireLayout();
				
				// Get the scroll value (if there is one):
				Css.Value scroll=Style.Computed.Scroll;
				
				if(scroll==null || scroll[0]==null){
					return 0;
				}
				
				return scroll[0].GetInteger(RenderData,null);
				
			}
			set{
				scrollTo(scrollLeft,value);
			}
		}
		
		/// <summary>The height of the content inside this element.</summary>
		public int contentHeight{
			get{
				RequireLayout();
				return (int)Style.Computed.FirstBox.ContentHeight;
			}
		}
		
		/// <summary>The width of the content inside this element.</summary>
		public int contentWidth{
			get{
				RequireLayout();
				return (int)Style.Computed.FirstBox.ContentWidth;
			}
		}
		
		/// <summary>The height of this element.</summary>
		public int pixelHeight{
			get{
				RequireLayout();
				return (int)Style.Computed.FirstBox.Height;
			}
		}
		
		/// <summary>The width of this element.</summary>
		public int pixelWidth{
			get{
				RequireLayout();
				return (int)Style.Computed.FirstBox.Width;
			}
		}
		
		/// <summary>The height of this element without margins or borders.</summary>
		public int scrollHeight{
			get{
				RequireLayout();
				return (int)Style.Computed.FirstBox.InnerHeight;
			}
		}
		
		/// <summary>The width of this element without margins or borders.</summary>
		public int scrollWidth{
			get{
				RequireLayout();
				return (int)Style.Computed.FirstBox.InnerWidth;
			}
		}
		
		/// <summary>Gets or sets the checked state of this radio/checkbox input. Note that 'checked' is a C# keyword, thus the uppercase.
		/// Nitro is not case-sensitive, so element.checked works fine there.</summary>
		public bool Checked{
			get{
				string check=this["checked"];
				
				if(!string.IsNullOrEmpty(check)){
					
					if(check=="0" || check.ToLower()=="false"){
						
						return false;
						
					}
					
					return true;
					
				}
				
				return false;
			}
			set{
				if(value){
					this["checked"]="1";
				}else{
					this["checked"]="";
				}
			}
		}
		
		/// <summary>The attributes of this element (DOM spec compliant mapping for Properties).</summary>
		public Dictionary<string,string> attributes{
			get{
				return Properties;
			}
		}
		
		/// <summary>The gameObject that this element is ultimately parented to.</summary>
		public GameObject rootGameObject{
			get{
				WorldUI wUI=worldUI;
				
				if(wUI==null){
					return UI.GUINode;
				}
				
				return wUI.gameObject;
			}
		}
		
		/// <summary>The world UI this element belongs to.</summary>
		public WorldUI worldUI{
			get{
				return htmlDocument.InWorldUI;
			}
		}
		
		/// <summary>Gets or sets the image from the background of this element.</summary>
		public Texture image{
			get{
				
				BackgroundImage img=RenderData.BGImage;
				
				if(img==null){
					return null;
				}
				
				if(img.Image==null){
					return null;
				}
				
				// Get the picture format:
				PictureFormat picture=img.Image.Contents as PictureFormat;
				
				if(picture==null){
					return null;
				}
				
				return picture.Image;
			}
			set{
				
				BackgroundImage img=RenderData.BGImage;
				
				if(value==null){
					if(img!=null){
						RenderData.BGImage=null;
						htmlDocument.RequestLayout();
					}
				}else{
					if(img==null){
						img=new BackgroundImage(RenderData);
						RenderData.BGImage=img;
					}
					img.SetImage(new ImagePackage(value as Texture2D));
				}
			}
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <param name="timeToAccelAndDecel">The time, in seconds, to take accelerating and decelerating.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime,float timeToAccelAndDecel){
			return animate(css,constantSpeedTime,timeToAccelAndDecel,timeToAccelAndDecel);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime){
			return animate(css,constantSpeedTime,0f,0f);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <param name="timeToAccelerate">The time, in seconds, to take accelerating.</param>
		/// <param name="timeToDecelerate">The time, in seconds, to take decelerating.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime,float timeToAccelerate,float timeToDecelerate){
			return new UIAnimation(this,css,constantSpeedTime,timeToAccelerate,timeToDecelerate);
		}
		
		/// <summary>Gets or sets if this element is focused.</summary>
		public bool focused{
			
			get{
				return (htmlDocument.activeElement==this);
			}
			set{
				if(value==true){
					focus();
				}else{
					blur();
				}
			}
		}
		
		/// <summary>This elements tab index.</summary>
		public int tabIndex{
			get{
				string value=this["tabindex"];
				
				if(value==null){
					return 0;
				}
				
				return int.Parse(value);
			}
			set{
				this["tabindex"]=value.ToString();
			}
		}
		
        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <returns>The style declaration describing the element.</returns>
        public Css.ComputedStyle getComputedStyle(){
			return getComputedStyle(null);
        }
		
        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <param name="pseudo">The optional pseudo selector to use.</param>
        /// <returns>The style declaration describing the element.</returns>
        public Css.ComputedStyle getComputedStyle(string pseudo){
			
			if(string.IsNullOrEmpty(pseudo)){
				return Style.Computed;
			}
			
			// Get the particular pseudo-element:
			pseudo=pseudo.ToLower().Trim();
			
			// Check if it contains :
			int index=pseudo.LastIndexOf(':');
			
			if(index!=-1){
				// Chop from there:
				pseudo=pseudo.Substring(index+1);
			}
			
			IRenderableNode el=Style.Computed.GetVirtualChild(pseudo) as IRenderableNode;
			
			if(el==null){
				return null;
			}
			
			return el.ComputedStyle;
			
		}
		
		/// <summary>Gets the computed style of this element.</summary>
		public Css.ComputedStyle computedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Gets the style of this element.</summary>
		public Css.ElementStyle style{
			get{
				return Style;
			}
		}
		
	}
	
}