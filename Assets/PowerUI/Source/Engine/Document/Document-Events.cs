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
using Nitro;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a HTML Document. UI.document is the main UI document.
	/// Use UI.document.innerHTML to set its content.
	/// </summary>

	public partial class HtmlDocument{
		
		/// <summary>Called when the title of this document changes.</summary>
		public DomEventDelegate ontitlechange{
			get{
				return GetFirstDelegate<DomEventDelegate>("titlechange");
			}
			set{
				addEventListener("titlechange",new DomEventListener(value));
			}
		}
		
		/// <summary>Called when the tooltip for this document changes.</summary>
		public DomEventDelegate ontooltipchange{
			get{
				return GetFirstDelegate<DomEventDelegate>("tooltipchange");
			}
			set{
				addEventListener("tooltipchange",new DomEventListener(value));
			}
		}
		
		/// <summary>Called when the document resizes.</summary>
		public DomEventDelegate onresize{
			get{
				return GetFirstDelegate<DomEventDelegate>("resize");
			}
			set{
				addEventListener("resize",new DomEventListener(value));
			}
		}
		
		/// <summary>Called when a key goes up.</summary>
		public KeyboardEventDelegate onkeyup{
			get{
				return GetFirstDelegate<KeyboardEventDelegate>("keyup");
			}
			set{
				addEventListener("keyup",new KeyboardEventListener(value));
			}
		}
		
		/// <summary>Called when a key goes down.</summary>
		public KeyboardEventDelegate onkeydown{
			get{
				return GetFirstDelegate<KeyboardEventDelegate>("keydown");
			}
			set{
				addEventListener("keydown",new KeyboardEventListener(value));
			}
		}
		
		/// <summary>Called when the mouse moves.</summary>
		public MouseEventDelegate onmousemove{
			get{
				return GetFirstDelegate<MouseEventDelegate>("mousemove");
			}
			set{
				addEventListener("mousemove",new MouseEventListener(value));
			}
		}
		
		/// <summary>Called when the document is about to be unloaded.</summary>
		public BeforeUnloadEventDelegate onbeforeunload{
			get{
				return GetFirstDelegate<BeforeUnloadEventDelegate>("beforeunload");
			}
			set{
				addEventListener("beforeunload",new BeforeUnloadEventListener(value));
			}
		}
		
	}
	
}