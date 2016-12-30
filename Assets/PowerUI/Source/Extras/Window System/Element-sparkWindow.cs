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
using Windows;


namespace PowerUI{
	
	/// <summary>
	/// A specialised window type for displaying OptionLists.
	/// Context menus derive from this.
	/// </summary>
	
	public partial class HtmlElement{
		
		/// <summary>Gets the spark window that holds this element.</summary>
		public Windows.Window sparkWindow{
			get{
				
				// Go up the dom looking for the first element with a '-spark-window-id' attribute:
				string windowID=this["-spark-window-id"];
				
				if(windowID==null){
					
					Dom.Node parent=parentNode;
					
					// If the parent is a document, we may need to skip over it (if we're inside an iframe).
					while(parent is Dom.Document){
						
						// Skip:
						parent=parent.parentNode;
						
					}
					
					// Get it as a HTML element:
					HtmlElement parentHtml=parent as HtmlElement;
					
					if(parentHtml==null){
						return null;
					}
					
					return parentHtml.sparkWindow;
					
				}
				
				// Got a window!
				int id;
				if(int.TryParse(windowID,out id)){
					
					// Get it by ID:
					return htmlDocument.sparkWindows[id];
					
				}
				
				return null;
			}
		}
		
	}
	
}