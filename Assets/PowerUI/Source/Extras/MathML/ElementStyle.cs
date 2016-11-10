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
using Dom;
using Blaze;
using UnityEngine;
using Css;

namespace MathML{
	
	/// <summary>
	/// Represents MathElement.style, a likely upcoming API.
	/// It hosts the computed style amongst other things.
	/// </summary>
	
	public class ElementStyle : Style{
		
		
		/// <summary>The computed style of this element.</summary>
		public ComputedStyle Computed;	
		
		
		/// <summary>Creates a new element style for the given element.</summary>
		/// <param name="element">The element that this will be the style for.</param>
		public ElementStyle(MathElement element):base(element){
			Computed=new ComputedStyle(element);
		}
		
		/// <summary>JS API function for getting the computed style.</summary>
		public ComputedStyle getComputedStyle(){
			return Computed;
		}
		
		public override ComputedStyle GetComputed(){
			return Computed;
		}
		
		public override string GetString(string property){
			return Computed.GetString(property);
		}
		
		/// <summary>Sets the text content of this element.</summary>
		public string content{
			set{
				Set("content","\""+value.Replace("\"","\\\"")+"\"");
			}
			get{
				return GetString("content");
			}
		}
		
		/// <summary>How far from the left this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string left{
			set{
				Set("left",value);
			}
			get{
				return GetString("left");
			}
		}
		
		/// <summary>How far from the right this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string right{
			set{
				Set("right",value);
			}
			get{
				return GetString("right");
			}
		}
		
		/// <summary>How far from the top this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string top{
			set{
				Set("top",value);
			}
			get{
				return GetString("top");
			}
		}
		
		/// <summary>How far from the bottom this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string bottom{
			set{
				Set("bottom",value);
			}
			get{
				return GetString("bottom");
			}
		}
		
		/// <summary>The width of the element.</summary>
		public string width{
			set{
				Set("width",value);
			}
			get{
				return GetString("width");
			}
		}
		
		/// <summary>The height of the element.</summary>
		public string height{
			set{
				Set("height",value);
			}
			get{
				return GetString("height");
			}
		}
		
	}
	
}