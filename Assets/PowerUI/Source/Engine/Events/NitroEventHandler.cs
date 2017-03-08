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
using Dom;

namespace PowerUI{
	
	/// Handler for Nitro events.
	public class NitroEventListener : EventListener{
		
		public Nitro.DynamicMethod<Nitro.Void> Listener;
		
		public NitroEventListener(Nitro.DynamicMethod<Nitro.Void> listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(Event e){
			Listener.Run(e);
		}
		
	}
	
}

namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		// All event-specific addEventListener overloads (except for SVG).
		// This avoids needing to manually create e.g. a EventListener<KeyboardEvent> object.
		
		public void addEventListener(string name,Nitro.DynamicMethod<Nitro.Void> listener){
			addEventListener(name,new PowerUI.NitroEventListener(listener));
		}
		
		public void addEventListener(string name,Nitro.DynamicMethod<Nitro.Void> listener,bool capture){
			addEventListener(name,new PowerUI.NitroEventListener(listener),capture);
		}
		
	}
	
}