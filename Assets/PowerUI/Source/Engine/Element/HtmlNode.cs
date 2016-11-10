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
using Css;
using Dom;
using Nitro;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// An interface which all HTML nodes subscribe to.
	/// </summary>
	
	public interface HtmlNode{
		
		#region Internal layout
		
		/// <summary>Called when an element is no longer on the screen.</summary>
		void WentOffScreen();
		
		#endregion
		
	}
	
	/// <summary>
	/// The HTML namespace attribute as used by all HTML nodes.
	/// </summary>
	public class HtmlNamespace : XmlNamespace{
		
		public HtmlNamespace()
			:base("http://www.w3.org/1999/xhtml","xhtml",typeof(PowerUI.HtmlDocument),"svg:svg,mml:math")
		{
			
		}
		
	}
	
}
