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

using PowerUI;


namespace Dom{
	
	public partial class Node{

		/// <summary>The parent as a HtmlElement (convenience method).</summary>
		public HtmlElement htmlParentNode{
			get{
				return parentNode as HtmlElement;
			}
		}
		
		/// <summary>The ownerDocument as a Html document.</summary>
		public HtmlDocument htmlDocument{
			get{
				return document_ as HtmlDocument;
			}
		}
		
	}
	
}