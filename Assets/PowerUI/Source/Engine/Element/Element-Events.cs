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
using Nitro;
using PowerUI;


namespace Dom{
	
	/// <summary>
	/// These are the "Global Event Handlers".
	/// </summary>

	public partial class Element{
		
		/// <summary>Called when this element receives a keyup.</summary>
		public KeyboardEventDelegate onkeyup{
			get{
				return GetFirstDelegate<KeyboardEventDelegate>("keyup");
			}
			set{
				addEventListener("keyup",new KeyboardEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a keydown.</summary>
		public KeyboardEventDelegate onkeydown{
			get{
				return GetFirstDelegate<KeyboardEventDelegate>("keydown");
			}
			set{
				addEventListener("keydown",new KeyboardEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a mouseup.</summary>
		public MouseEventDelegate onmouseup{
			get{
				return GetFirstDelegate<MouseEventDelegate>("mouseup");
			}
			set{
				addEventListener("mouseup",new MouseEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a mouseout.</summary>
		public MouseEventDelegate onmouseout{
			get{
				return GetFirstDelegate<MouseEventDelegate>("mouseout");
			}
			set{
				addEventListener("mouseout",new MouseEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a mousedown.</summary>
		public MouseEventDelegate onmousedown{
			get{
				return GetFirstDelegate<MouseEventDelegate>("mousedown");
			}
			set{
				addEventListener("mousedown",new MouseEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a mousemove. Note that it must be focused.</summary>
		public MouseEventDelegate onmousemove{
			get{
				return GetFirstDelegate<MouseEventDelegate>("mousemove");
			}
			set{
				addEventListener("mousemove",new MouseEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a mouseover.</summary>
		public MouseEventDelegate onmouseover{
			get{
				return GetFirstDelegate<MouseEventDelegate>("mouseover");
			}
			set{
				addEventListener("mouseover",new MouseEventListener(value));
			}
		}
		
		/// <summary>Called when a form is reset.</summary>
		public FormEventDelegate onreset{
			get{
				return GetFirstDelegate<FormEventDelegate>("reset");
			}
			set{
				addEventListener("reset",new FormEventListener(value));
			}
		}
		
		/// <summary>Called when a form is submitted.</summary>
		public FormEventDelegate onsubmit{
			get{
				return GetFirstDelegate<FormEventDelegate>("submit");
			}
			set{
				addEventListener("submit",new FormEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a load event (e.g. iframe).</summary>
		public UIEventDelegate onload{
			get{
				return GetFirstDelegate<UIEventDelegate>("load");
			}
			set{
				addEventListener("load",new UIEventListener(value));
			}
		}
		
		/// <summary>Called when this element gets focused.</summary>
		public FocusEventDelegate onfocus{
			get{
				return GetFirstDelegate<FocusEventDelegate>("focus");
			}
			set{
				addEventListener("focus",new FocusEventListener(value));
			}
		}
		
		/// <summary>Called just before this element is focused.</summary>
		public FocusEventDelegate onfocusin{
			get{
				return GetFirstDelegate<FocusEventDelegate>("focusin");
			}
			set{
				addEventListener("focusin",new FocusEventListener(value));
			}
		}
		
		/// <summary>Called just before this element is blurred.</summary>
		public FocusEventDelegate onfocusout{
			get{
				return GetFirstDelegate<FocusEventDelegate>("focusout");
			}
			set{
				addEventListener("focusout",new FocusEventListener(value));
			}
		}
		
		/// <summary>Called when this element is unfocused (blurred).</summary>
		public FocusEventDelegate onblur{
			get{
				return GetFirstDelegate<FocusEventDelegate>("blur");
			}
			set{
				addEventListener("blur",new FocusEventListener(value));
			}
		}
		
		/// <summary>Called when this element receives a full click.</summary>
		public MouseEventDelegate onclick{
			get{
				return GetFirstDelegate<MouseEventDelegate>("click");
			}
			set{
				addEventListener("click",new MouseEventListener(value));
			}
		}
		
		/// <summary>Used by e.g. input, select etc. Called when its value changes.</summary>
		public DomEventDelegate onchange{
			get{
				return GetFirstDelegate<DomEventDelegate>("change");
			}
			set{
				addEventListener("change",new DomEventListener(value));
			}
		}
		
	}
	
}