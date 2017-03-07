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
using UnityEngine;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Used to 'flatten' an element.
	/// </summary>

	public partial class HtmlElement{
		
		/// <summary>Obtains the flat image when CSS -spark-filter is in use.</summary>
		public Texture flatImage{
			get{
				
				// Obtain the raster data from the render data:
				RasterDisplayableProperty rdp=RenderData.RasterProperty;
				
				if(rdp==null){
					return null;
				}
				
				// It's the output property (which might've passed through filters):
				return rdp.Output;
			}
		}
		
		/// <summary>Flattens this element and returns the live flat image.</summary>
		public Texture flatten(){
			
			Texture img=flatImage;
			
			if(img!=null){
				// Already flat!
				return img;
			}
			
			// Just flatten it (ie no full Loonim effects here):
			style.Computed.ChangeTagProperty("-spark-filter","flatten");
			
			// Get the flat image:
			return flatImage;
		}
		
	}
	
}