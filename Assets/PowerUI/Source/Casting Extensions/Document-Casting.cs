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
using PowerUI.Http;
using System.Collections;
using System.Collections.Generic;
using PowerUI;


namespace PowerUI{
	
	/// <summary>
	/// A collection of convenience functions for obtaining HtmlElements.
	/// </summary>
	
	public partial class HtmlDocument{
		
		/// <summary>Casts getElementByTagName to a HtmlElement for you (exists because of SVG and MathML).</summary>
		public HtmlElement getByTagName(string tag){
			
			return getElementByTagName(tag) as HtmlElement;
			
		}
		
		/// <summary>Casts getElementById to a HtmlElement for you (exists because of SVG and MathML).</summary>
		public HtmlElement getById(string id){
			
			return getElementById(id) as HtmlElement;
			
		}
		
		/// <summary>Casts getElementByAttribute to a HtmlElement for you (exists because of SVG and MathML).</summary>
		public HtmlElement getByAttribute(string property,string value){
			
			return getElementByAttribute(property,value) as HtmlElement;
			
		}
		
	}
	
}