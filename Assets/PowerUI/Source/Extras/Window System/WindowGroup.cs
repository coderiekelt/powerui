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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using PowerUI;
using Nitro;
using Dom;


namespace Windows{
	
	/// <summary>
	/// A group of windows.
	/// </summary>
	public class WindowGroup:EventTarget{
		
		/// <summary>The window manager.</summary>
		public Manager Manager;
		/// <summary>The parent group, if any.</summary>
		public WindowGroup Parent;
		/// <summary>An element to parent child windows to.</summary>
		private HtmlElement WindowHostElement_;
		/// <summary>All active windows, sorted by depth.</summary>
		public List<Window> Windows=new List<Window>();
		
		
		/// <summary>Creates a window group.</summary>
		public WindowGroup(Manager manager){
			Manager=manager;
		}
		
		public WindowGroup(){}
		
		/// <summary>The doc that hosts child windows.</summary>
		public HtmlDocument WindowHostDocument{
			get{
				
				Window w=this as Window;
				
				if(w==null){
					
					// Use the manager:
					return Manager.Document;
					
				}
				
				// Use the window's content doc:
				if(w.contentDocument==null){
					
					// Go up a level:
					return Parent.WindowHostDocument;
					
				}
				
				return w.contentDocument;
				
			}
		}
		
		/// <summary>An element to parent child windows to.</summary>
		public HtmlElement WindowHostElement{
			get{
				
				if(WindowHostElement_==null){
					
					// Get the host doc:
					HtmlDocument hostDoc=WindowHostDocument;
					
					// Create a 100% * 100% fixed div inside it:
					HtmlElement e=hostDoc.createElement("div") as HtmlElement;
					e.className="spark-window-host";
					e.style.width="100%";
					e.style.height="100%";
					e.style.position="fixed";
					hostDoc.appendChild(e);
					WindowHostElement_=e;
					
				}
				
				return WindowHostElement_;
			}
		}
		
		/// <summary>Gets a window of the given type and pointing at the given URL.</summary>
		/// <returns>Null if not found.</returns>
		public Window get(string type,string url){
			
			// For each one..
			for(int i=Windows.Count-1;i>=0;i--){
				
				Window w=Windows[i];
				
				// Match?
				if(w.Type==type && (url==null || w.Location==url)){
					return w;
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Closes an open window or opens it if it wasn't already.</summary>
		public Window cycle(string type,string url,Dictionary<string,object> globals){
			
			// Try getting it:
			Window w=get(type,url);
			
			if(w==null){
				
				// Open it:
				w=open(type,url,globals);
				return w;
				
			}else if(w.HidBy!=null){
				
				// It's actually hidden - unhide it (and hide the visible one).
				
				// Remove it from the linked list of hidden windows:
				if(w.HidWindow!=null){
					w.HidWindow.HidBy=w.HidBy;
				}
				
				w.HidBy.HidWindow=w.HidWindow;
				
				// Make it visible:
				w.Visibility(true,null);
				
				// Next, hide the visible one:
				Window visible=w.GetVisibleWindow();
				visible.Visibility(false,w);
				
				return w;
				
			}
			
			// Close it:
			w.close();
			
			return null;
			
		}
		
		/// <summary>Opens a window.</summary>
		public Window open(string typeName,string url,Dictionary<string,object> globals){
			
			if(Manager.WindowTypes==null){
				Manager.Setup(Assemblies.Current);
			}
			
			Type type;
			if(!Manager.WindowTypes.TryGetValue(typeName,out type)){
				return null;
			}
			
			// instance it now:
			Window w=Activator.CreateInstance(type) as Window;
			
			if(w==null){
				return null;
			}
			
			// Get stacking behaviour:
			StackMode stacking=w.StackMode;
			
			object stackModeObj;
			if(globals!=null && globals.TryGetValue("-spark-stack-mode",out stackModeObj)){
				
				string stackMode=stackModeObj.ToString().Trim().ToLower();
				
				if(stackMode=="hide"){
					stacking=StackMode.Hide;
				}else if(stackMode=="close"){
					stacking=StackMode.Close;
				}else{
					stacking=StackMode.Over;
				}
				
			}
			
			if(stacking==StackMode.Hide){
				
				// Hides any window of the same type.
				Window same=get(typeName,null);
				
				if(same!=null){
					
					// Make sure it's actually the visible one:
					same=same.GetVisibleWindow();
					
					same.Visibility(false,w);
					w.HidWindow=same;
				}
				
			}else if(stacking==StackMode.Close){
				
				// Close all windows of the same type.
				
				// For each one..
				for(int i=Windows.Count-1;i>=0;i--){
					
					Window same=Windows[i];
					
					// Match?
					if(same.Type==typeName){
						same.close();
					}
					
				}
				
			}
			
			// Apply type:
			w.Type=typeName;
			
			// Add now:
			SetupWindow(w,url,globals);
			
			return w;
			
		}
		
		/// <summary>Removes the given window.</summary>
		internal void Remove(Window w){
			
			if(Windows[w.Index]!=w){
				return;
			}
			
			// Move everything else over:
			for(int i=w.Index+1;i<Windows.Count;i++){
				Windows[i].Index--;
			}
			
			// Remove at its index now:
			Windows.RemoveAt(w.Index);
			
		}
		
		/// <summary>Inserts the given window into the list of open windows and begins loading it.</summary>
		private void SetupWindow(Window w,string url,Dictionary<string,object> globals){
			
			// Get and apply depth:
			int newWindowDepth=w.Depth;
			w.ActiveDepth=newWindowDepth;
			w.Manager=Manager;
			w.Parent=this;
			
			// Add to the end:
			Windows.Add(w);
			w.Index=Windows.Count-1;
			
			// Shuffle it forward if it's a lower depth (but don't shuffle forward for ==):
			
			// For each one..
			for(int i=Windows.Count-1;i>0;i--){
				
				Window current=Windows[i-1];
				int depth=current.ActiveDepth;
				
				if(newWindowDepth<depth){
					
					// Shuffle it forward:
					current.Index=i;
					Windows[i]=current;
					Windows[i-1]=w;
					w.Index=i-1;
					
				}else{
					break;
				}
				
			}
			
			// Next, load its content:
			w.Load(url,globals);
			
		}
		
	}
	
}