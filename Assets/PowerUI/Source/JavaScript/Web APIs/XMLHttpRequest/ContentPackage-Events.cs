//--------------------------------------
//          Kulestar Unity HTTP
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;
using Nitro;
using Dom;


namespace PowerUI{
	
	public partial class ContentPackage{
		
		/// <summary>Called when the ready state changes.</summary>
		public UIEventDelegate onreadystatechange{
			get{
				return GetFirstDelegate<UIEventDelegate>("readystatechange");
			}
			set{
				addEventListener("readystatechange",new UIEventListener(value));
			}
		}
		
		/// <summary>Called when it's done loading.</summary>
		public UIEventDelegate onload{
			get{
				return GetFirstDelegate<UIEventDelegate>("load");
			}
			set{
				addEventListener("load",new UIEventListener(value));
			}
		}
		
		/// <summary>Called when the request times out.</summary>
		public UIEventDelegate ontimeout{
			get{
				return GetFirstDelegate<UIEventDelegate>("timeout");
			}
			set{
				addEventListener("timeout",new UIEventListener(value));
			}
		}
		
		/// <summary>Called when the request errors.</summary>
		public UIEventDelegate onerror{
			get{
				return GetFirstDelegate<UIEventDelegate>("error");
			}
			set{
				addEventListener("error",new UIEventListener(value));
			}
		}
		
		/// <summary>Called when the request is aborted.</summary>
		public UIEventDelegate onabort{
			get{
				return GetFirstDelegate<UIEventDelegate>("abort");
			}
			set{
				addEventListener("abort",new UIEventListener(value));
			}
		}
		
	}
	
}