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
using PowerUI;
using System.Collections;
using System.Collections.Generic;


namespace Windows{
	
	/// <summary>
	/// An instance of an open window.
	/// </summary>
	
	public class Window : WindowGroup{
		
		/// <summary>Index in managers array.</summary>
		private int Index_; 
		/// <summary>Index in managers array.</summary>
		internal int Index{
			get{
				return Index_;
			}
			set{
				Index_=value;
				
				// Update the attribute:
				if(element!=null){
					element["-spark-window-id"]=value.ToString();
				}
				
			}
		}
		/// <summary>The window type. E.g. "floating".</summary>
		internal string Type;
		/// <summary>This windows actual depth.</summary>
		internal int ActiveDepth;
		/// <summary>The location of this window.</summary>
		internal string Location;
		/// <summary>The root element which contains this window. All templates must have one root only (watch out for comments and text!).</summary>
		internal HtmlElement element;
		/// <summary>The iframe. May be null.</summary>
		internal HtmlElement frame;
		/// <summary>If this window is in the 'hide other' stacking mode, this is the window it hid.</summary>
		internal Window HidWindow;
		/// <summary>If this window is hidden, the one that hid it.</summary>
		internal Window HidBy;
		/// <summary>The document to load content into. May be null.</summary>
		internal HtmlDocument contentDocument;
		
		
		/// <summary>Closes the window and its kids.</summary>
		public void close(){
			close(true);
		}
		
		/// <summary>Closes the window and its kids, optionally avoiding removing the actual element.</summary>
		private void close(bool removeFrame){
			
			// Close all kids:
			foreach(Window w in Windows){
				w.close(false);
			}
			
			if(removeFrame){
				
				// Run the close events:
				trigger("close");
				trigger("animatehide");
				
				if(HidWindow!=null){
					// Display hid window again:
					HidWindow.Visibility(true,null);
					HidWindow=null;
				}
				
				// Clear the doc:
				if(contentDocument!=null){
					contentDocument.clear();
				}
				
				// Destroy the element:
				element.parentNode.removeChild(element);
				
				// Remove from parent:
				Parent.Remove(this);
				
				element=null;
				
			}
			
		}
		
		/// <summary>The window at the back and same depth as this one.</summary>
		public Window backSameDepth{
			get{
				Window current=previousSameDepth;
				
				while(current!=null){
					Window next=current.previousSameDepth;
					
					if(next==null){
						return current;
					}
					
					current=next;
				}
				
				return this;
			}
		}
		
		/// <summary>The window before this one (further back) of the same type.</summary>
		public Window previousSameDepth{
			get{
				
				if(Index<=0){
					return null;
				}
				
				Window w=Parent.Windows[Index-1];
				
				if(w.ActiveDepth!=ActiveDepth){
					return null;
				}
				
				return w;
			}
		}
		
		/// <summary>The window at the front and same depth as this one.</summary>
		public Window frontSameDepth{
			get{
				Window current=nextSameDepth;
				
				while(current!=null){
					Window next=current.nextSameDepth;
					
					if(next==null){
						return current;
					}
					
					current=next;
				}
				
				return this;
			}
		}
		
		/// <summary>The window after this one (further forward) of the same type.</summary>
		public Window nextSameDepth{
			get{
				
				if(Index>=Parent.Windows.Count){
					return null;
				}
				
				Window w=Parent.Windows[Index+1];
				
				if(w.ActiveDepth!=ActiveDepth){
					return null;
				}
				
				return w;
			}
		}
		
		/// <summary>Sends this window to the back.</summary>
		public void sendBackward(){
			
			// Insert element before the one at the back:
			Window back=previousSameDepth;
			
			if(back!=this){
				element.parentNode.insertBefore(element,back.element);
			}
			
		}
		
		/// <summary>Sends this window to the back.</summary>
		public void sendToBack(){
			
			// Insert element before the one at the back:
			Window back=backSameDepth;
			
			if(back!=this){
				element.parentNode.insertBefore(element,back.element);
			}
			
		}
		
		/// <summary>Moves this window forward one place.</summary>
		public void bringForward(){
			
			// Insert element after the one in front:
			Window front=nextSameDepth;
			
			if(front!=this){
				element.parentNode.insertAfter(element,front.element);
			}
			
		}
		
		/// <summary>Brings this window to the front.</summary>
		public void bringToFront(){
			
			// Insert element after the one at the front:
			Window front=frontSameDepth;
			
			if(front!=this){
				element.parentNode.insertAfter(element,front.element);
			}
			
		}
		
		/// <summary>Hides/ shows the window (without actually closing it).</summary>
		public void Visibility(bool visible,Window hidBy){
			
			if(element==null){
				return;
			}
			
			HidBy=hidBy;
			element.style.display=visible?"block":"none";
			
		}
		
		/// <summary>True if these windows stack.</summary>
		public virtual StackMode StackMode{
			get{
				return StackMode.Over;
			}
		}
		
		/// <summary>The depth that this type of window lives at.</summary>
		public virtual int Depth{
			get{
				return 0;
			}
		}
		
		/// <summary>An element with the '-spark-title' attribute.</summary>
		public HtmlElement TitleElement{
			get{
				return element.getElementByAttribute("-spark-title",null) as HtmlElement;
			}
		}
		
		/// <summary>Navigates the window to the given URL. Should only be used once; 
		/// close and open another window (or use links inside the iframe).</summary>
		private void Goto(string url,Dictionary<string,object> globals){
			
			// Set location:
			Location=url;
			
			if(contentDocument==null){
				
				// Doesn't navigate.
				return;
				
			}
			
			// Use an event which runs between the clear and load:
			contentDocument.AfterClearBeforeSet=delegate(Dom.DomEvent acs){
				
				// Hook up title change:
				contentDocument.ontitlechange+=delegate(Dom.DomEvent e){
					
					// Set the 'title' element, if we have one:
					HtmlElement t=TitleElement;
					
					if(t!=null){
						t.innerHTML=contentDocument.title;
					}
					
				};
				
				// Apply globals to the script engine:
				if(globals!=null){
					
					foreach(KeyValuePair<string,object> kvp in globals){
						
						// Set it:
						contentDocument.setJsVariable(kvp.Key,kvp.Value);
						
					}
					
				}
				
			};
			
			// Run the open events:
			trigger("open");
			trigger("animateshow");
			
			// Navigate CD:
			contentDocument.location.href=url;
			
		}
		
		/// <summary>Triggers a 'window{name}' event in the root document.</summary>
		public void trigger(string name){
			
			// Create the event:
			Dom.DomEvent e=Manager.Document.createEvent("window"+name);
			e.SetTrusted();
			
			// Trigger it now:
			Manager.Document.dispatchEvent(e);
			
		}
		
		/// <summary>When a window hides another, it may result in a linked list of hidden windows.
		/// This essentially finds the front of the linked list.</summary>
		public Window GetVisibleWindow(){
			
			Window current=HidBy;
			
			while(current!=null){
				Window next=current.HidBy;
				
				if(next==null){
					return current;
				}
				
				current=next;
			}
			
			return this;
			
		}
		
		/// <summary>Gets a global of the given name.</summary>
		public T Get<T>(string name,Dictionary<string,object> globals){
			
			object value;
			if(globals!=null && globals.TryGetValue(name.Trim().ToLower(),out value)){
				
				return (T)value;
				
			}
			
			// Not found.
			return default(T);
			
		}
		
		/// <summary>Adds style if it was required.</summary>
		protected void AddStyle(){
			
			string stylePath="resources://"+Type+"-style.html";
			HtmlDocument hostDoc=Parent.WindowHostDocument;
			
			if(hostDoc.GetStyle(stylePath)==null){
				
				// Create a link tag:
				HtmlElement he=hostDoc.createElement("link") as HtmlElement;
				
				// Setup (causes it to load now):
				he["type"]="text/css";
				he["href"]=stylePath;
				
				// Append it (not actually required):
				// element.appendChild(he);
				
			}
			
		}
		
		/// <summary>Writes the windows HTML now. Collects element and optionally an iframe.</summary>
		public void SetHtml(string html){
			
			// Either use before() or direct insert:
			int domIndex=0;
			
			if(Index!=(Parent.Windows.Count-1)){
				
				// Insert at the child index of the window after this one:
				Window afterThis=Parent.Windows[Index+1];
				domIndex=afterThis.element.childIndex;
				
			}
			
			// Ok! Get the host:
			HtmlElement windowHost=Parent.WindowHostElement;
			
			// Insert now!
			windowHost.insertInnerHTML(domIndex,html);
			
			// Grab the child at 'index':
			element=windowHost.childNodes[domIndex] as HtmlElement;
			
			if(element==null){
				// Text or comment template error.
				throw new Exception("The window template for '"+Type+"' starts with e.g. text/ a comment.");
			}
			
			// Search for an iframe (optional):
			frame=element.getElementByTagName("iframe") as HtmlElement;
			
			if(frame!=null){
				contentDocument=frame.contentDocument;
			}
			
		}
		
		/// <summary>Loads the contents of this window now.</summary>
		public virtual void Load(string url,Dictionary<string,object> globals){
			
			// Get the template:
			DataPackage package=new DataPackage("resources://"+Type+"-template.html",null);
			
			package.onload=delegate(UIEvent e){
				
				// Write the HTML:
				SetHtml(package.responseText);
				
				// Append style if it was required:
				AddStyle();
				
				// Update the attribute:
				element["-spark-window-id"]=Index.ToString();
				
				// Load now:
				Goto(url,globals);
				
			};
			
			package.onerror=delegate(UIEvent e){
				
				// Error!
				Dom.Log.Add("Window template for '"+Type+"' was not found. Tried "+package.location);
				
			};
			
			// Send now:
			package.send();
			
		}
		
	}
	
}