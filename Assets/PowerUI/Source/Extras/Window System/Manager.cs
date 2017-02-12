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
	/// A window system manager. There's one per document (available as document.sparkWindows). Handles e.g. popping open a window.
	/// </summary>
	
	public class Manager{
		
		/// <summary>The document that is being managed.</summary>
		public HtmlDocument Document;
		/// <summary>The group of open windows.</summary>
		public WindowGroup Windows;
		
		
		public Manager(HtmlDocument doc){
			Document=doc;
			Windows=new WindowGroup(this);
		}
		
		/// <summary>Gets a window by ID. Null if out of range.</summary>
		public Window get(int id){
			return this[id];
		}
		
		/// <summary>Gets a window by ID. Null if out of range.</summary>
		public Window this[int id]{
			get{
				
				if(id<0 || id>=Windows.Windows.Count){
					return null;
				}
				
				// Get it:
				return Windows.Windows[id];
			}
		}
		
		/// <summary>Gets the window of the given type pointing at the given URL.</summary>
		public Window get(string type,string url){
			return Windows.get(type,url);
		}
		
		/// <summary>Closes an open window or opens it if it wasn't already.</summary>
		public Window cycle(string type,string url){
			return Windows.cycle(type,url,null);
		}
		
		/// <summary>Closes an open window or opens it if it wasn't already.</summary>
		public Window cycle(string type,string url,Dictionary<string,object> globals){
			return Windows.cycle(type,url,globals);
		}
		
		/// <summary>Creates a new window of the given type and points it at the given URL.</summary>
		public Window open(string type,string url){
			return Windows.open(type,url,null);
		}
		
		/// <summary>Creates a new window of the given type and points it at the given URL.
		/// Creates a set of global variables available to the JS in the window.</summary>
		public Window open(string type,string url,Dictionary<string,object> globals){
			return Windows.open(type,url,globals);
		}
		
		/// <summary>The available window template types.</summary>
		public static Dictionary<string,Type> WindowTypes;
		
		/// <summary>Adds the given type as an available window type.</summary>
		public static void Add(Type type){
			
			if(WindowTypes==null){
				// Create the set now:
				WindowTypes=new Dictionary<string,Type>();
			}
			
			// Get the name attribute from it (don't inherit):
			TagName tagName=Attribute.GetCustomAttribute(type,typeof(TagName),false) as TagName;
			
			if(tagName==null){
				// Nope!
				return;
			}
			
			string tag=tagName.Tags;
			
			if(!string.IsNullOrEmpty(tag)){
				
				// Add now:
				WindowTypes[tag]=type;
				
			}
			
		}
		
	}
	
}

namespace PowerUI{

	public partial class HtmlDocument{
		
		/// <summary>Instance of a window manager. May be null.</summary>
		private Windows.Manager windows_;
		
		/// <summary>The document.windows API. Read only.</summary>
		public Windows.Manager sparkWindows{
			get{
				
				if(windows_==null){
					windows_=new Windows.Manager(this);
				}
				
				return windows_;
			}
		}
		
	}
	
}