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
		
		/// <summary>Builds a set of globals from an array of data (of the form 'key',value,'key',value..).</summary>
		public static Dictionary<string,object> buildGlobals(object[] data){
			
			if(data==null || data.Length==0){
				return null;
			}
			
			if(data.Length==1){
				
				if(data[0] is Dictionary<string,object>){
					return data[0] as Dictionary<string,object>;
				}
				
			}
			
			// Must be a multiple of 2:
			if((data.Length % 2)!=0){
				throw new Exception("Didn't recognise this as a valid globals set.");
			}
			
			// # of globals:
			int count=data.Length/2;
			
			// Create the set:
			Dictionary<string,object> result=new Dictionary<string,object>(count);
			
			// Alternates between key and value:
			int index=0;
			
			for(int i=0;i<count;i++){
				
				// Get k/v:
				string key=data[index++] as string;
				object value=data[index++] as object;
				
				if(key==null){
					continue;
				}
				
				// Add it:
				result[key]=value;
				
			}
			
			return result;
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
		public Window cycle(string type,string url,Dictionary<string,object> globals){
			return Windows.cycle(type,url,globals);
		}
		
		/// <summary>Closes an open window or opens it if it wasn't already.
		/// globalData alternates between a string (key) and a value (object).
		/// I.e 'key',value,'key2',value..</summary>
		public Window cycle(string type,string url,params object[] globalData){
			return Windows.cycle(type,url,buildGlobals(globalData));
		}
		
		/// <summary>Opens a window of the given type and points it at the given URL.
		/// globalData alternates between a string (key) and a value (object).
		/// I.e 'key',value,'key2',value..</summary>
		public Window open(string type,string url,params object[] globalData){
			return Windows.open(type,url,buildGlobals(globalData));
		}
		
		/// <summary>Creates a new window of the given type and points it at the given URL.
		/// Creates a set of global variables available to the JS in the window.</summary>
		public Window open(string type,string url,Dictionary<string,object> globals){
			return Windows.open(type,url,globals);
		}
		
		/// <summary>Closes a window. Just a convenience version of window.close();</summary>
		public void close(string type,string url){
			
			// Try getting it:
			Window w=get(type,url);
			
			if(w!=null){
				w.close();
			}
			
		}
		
		/// <summary>The available window template types.</summary>
		public static Dictionary<string,Type> windowTypes;
		
		/// <summary>Adds the given type as an available window type.</summary>
		public static void Add(Type type){
			
			if(windowTypes==null){
				// Create the set now:
				windowTypes=new Dictionary<string,Type>();
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
				windowTypes[tag]=type;
				
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