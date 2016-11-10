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

namespace PowerUI{
	
	/// <summary>
	/// Represents a canvas which lets you draw 2D shapes and polygons on the UI.
	/// </summary>
	
	[Dom.TagName("canvas")]
	public class HtmlCanvasElement:HtmlElement{
		
		/// <summary>The 2D canvas context. Use getContext("2D") or context to obtain.</summary>
		private CanvasContext2D Context2D;
		
		
		/// <summary>Gets a rendering context for this canvas.</summary>
		public override CanvasContext getContext(string contextName){
			// Lowercase it:
			contextName=contextName.ToLower();
			
			// Is it the 2D context?
			if(contextName=="2d"){
				return context2D;
			}
			
			return null;
		}
		
		/// <summary>The 2D canvas context.</summary>
		public CanvasContext2D context2D{
			get{
				if(Context2D==null){
					Context2D=new CanvasContext2D(this);
				}
				
				return Context2D;
			}
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			if(Context2D!=null){
				Context2D.Resized(box);
			}
			
		}
		
	}
	
	
	public partial class HtmlElement{
		
		/// <summary>Gets a rendering context for this canvas (if it is a canvas element!).</summary>
		/// <param name="text">The context type e.g. "2D".</param>
		public virtual CanvasContext getContext(string text){
			return null;
		}
		
	}
	
}