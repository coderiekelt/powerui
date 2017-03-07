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
	
	/// <summary>A delegate used just before a window loads.</summary.
	public delegate void WindowDelegate(Window w);
	
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
					hostDoc.html.appendChild(e);
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
		
		/// <summary>Closes a window. Just a convenience version of window.close();</summary>
		public void close(string type,string url){
			
			// Try getting it:
			Window w=get(type,url);
			
			if(w!=null){
				w.close();
			}
			
		}
		
		/// <summary>Closes an open window or opens it if it wasn't already 
		/// optionally with globals. Of the form 'key',value,'key2',value..</summary>
		public Window cycle(string type,string url,params object[] globalData){
			return cycle(type,url,Manager.buildGlobals(globalData));
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
		
		/// <summary>Loads a window of the given type.</summary>
		public Promise load(string typeName){
			return load(typeName,null,(Dictionary<string,object>)null);
		}
		
		/// <summary>Opens a window optionally with globals. Of the form 'key',value,'key2',value..
		/// returning a promise which runs when the windows 'load' event occurs.</summary>
		public Promise load(string typeName,string url,params object[] globalData){
			return load(typeName,url,Manager.buildGlobals(globalData));
		}
		
		/// <summary>Opens a window, returning a promise which runs when the windows 'load' event occurs.</summary>
		public Promise load(string typeName,string url,Dictionary<string,object> globals){
			
			// Add an event listener just before Load is invoked:
			Promise p=new Promise();
			
			open(
				typeName,
				url,
				delegate(Window w){
					
					// Add an event cb:
					if(w==null){
						
						// Rejected:
						p.reject("Window '"+typeName+"' is missing.");
						
					}else{
						
						w.addEventListener("load",delegate(UIEvent e){
							
							// Ok!
							p.resolve(w);
							
						});
						
					}
				},
				globals
			);
			
			return p;
		}
		
		/// <summary>Opens a window optionally with globals. Of the form 'key',value,'key2',value..</summary>
		public Window open(string typeName,string url,params object[] globalData){
			return open(typeName,url,null,Manager.buildGlobals(globalData));
		}
		
		/// <summary>Opens a window.</summary>
		public Window open(string typeName,string url,Dictionary<string,object> globals){
			return open(typeName,url,null,globals);
		}
		
		/// <summary>Opens a window.</summary>
		public Window open(string typeName,string url,WindowDelegate preload,Dictionary<string,object> globals){
			
			if(Manager.windowTypes==null){
				
				// Load the windows now!
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(Windows.Window),
					delegate(Type t){
						// Add it as an available window:
						Manager.Add(t);
					}
				);
				
			}
			
			Type type;
			if(!Manager.windowTypes.TryGetValue(typeName,out type)){
				
				UnityEngine.Debug.Log("Warning: Requested to open a window called '"+typeName+"' but it doesn't exist.");
				
				if(preload!=null){
					// Invoke the load method:
					preload(null);
				}
				
				return null;
			}
			
			// Get existing:
			Window same=get(typeName,null);
			
			// Get stacking behaviour:
			StackMode stacking=StackMode.Close;
			
			if(same!=null){
				stacking=same.StackMode;
			}
			
			object stackModeObj;
			if(globals!=null && globals.TryGetValue("-spark-stack-mode",out stackModeObj)){
				
				string stackMode=stackModeObj.ToString().Trim().ToLower();
				
				if(stackMode=="hide"){
					stacking=StackMode.Hide;
				}else if(stackMode=="close"){
					stacking=StackMode.Close;
				}else if(stackMode=="hijack"){
					stacking=StackMode.Hijack;
				}else{
					stacking=StackMode.Over;
				}
				
			}
			
			if(stacking==StackMode.Hijack){
				
				// Hijack an existing window! Just load straight into it but clear its event handlers:
				same.RunLoad=true;
				same.ClearEvents();
				
				if(preload!=null){
					preload(same);
				}
				
				same.Load(url,globals);
				same.TryLoadEvent(globals);
				
				return same;
			}
			
			// instance it now:
			Window w=Activator.CreateInstance(type) as Window;
			
			if(w==null){
				return null;
			}
			
			if(stacking==StackMode.Hide){
				
				// Hides any window of the same type.
				
				if(same!=null){
					
					// Make sure it's actually the visible one:
					same=same.GetVisibleWindow();
					
					same.Visibility(false,w);
					w.HidWindow=same;
				}
				
			}else if(stacking==StackMode.Close && same!=null){
				
				// Close all windows of the same type.
				
				// For each one..
				for(int i=Windows.Count-1;i>=0;i--){
					
					same=Windows[i];
					
					// Match?
					if(same.Type==typeName){
						same.close();
					}
					
				}
				
			}
			
			// Apply type:
			w.Type=typeName;
			
			if(preload!=null){
				preload(w);
			}
			
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
			w.TryLoadEvent(globals);
			
		}
		
	}
	
}