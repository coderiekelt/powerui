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
using UnityEngine;
using Dom;
using Jint.Runtime.Interop;


namespace PowerUI{
	
	/// <summary>
	/// The default script handler for text/javascript-x.
	/// </summary>
	
	public class JavaScriptEngine : PowerUI.ScriptEngine{
		
		/// <summary>The meta types that your engine will handle. E.g. "text/javascript".</summary>
		public override string[] GetTypes(){
			return new string[]{"text/javascript"};
		}
		
		/// <summary>An instance of the ScriptEngine on this page.</summary>
		public Jint.Engine Engine;
		
		public JavaScriptEngine(){}
		
		public JavaScriptEngine(bool safeHost,HtmlDocument doc,object window){
			
			Engine = new Jint.Engine(cfg => cfg.AllowClr());
			
			Engine.SetValue("document", doc)
				.SetValue("window", window)
				.SetValue("Promise", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Promise)))
				.SetValue("XMLHttpRequest", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.XMLHttpRequest)))
				.SetValue("Module", TypeReference.CreateTypeReference(Engine, typeof(WebAssembly.Module)))
				.SetValue("console", new JavaScript.console());
			
			Document=doc;
			
		}
		
		/// <summary>Gets or sets script variable values.</summary>
		/// <param name="index">The name of the variable.</param>
		/// <returns>The variable value.</returns>
		public override object this[string global]{
			get{
				if(Engine==null){
					return null;
				}
				
				return Engine.GetValue(global);
			}
			set{
				if(Engine==null){
					return;
				}
				
				Engine.SetValue(global,value);
			}
		}
		
		public override PowerUI.ScriptEngine Instance(Document document){
			
			HtmlDocument doc=document as HtmlDocument;
			bool safeHost=true;
			
			if(doc!=null){
				
				Location location=doc.location;
				Window window=doc.window;
				
				// Iframe security check - can code from this domain run at all?
				// We have the Nitro runtime so it could run unwanted code.
				if(window.parent!=null && location!=null && !location.fullAccess){
					
					// It's an iframe to some unsafe location.
					safeHost = false;
					
				}
				
			}
			
			return new JavaScriptEngine(safeHost,doc,doc.window);
		}
		
		public override object Compile(string source, object scope){
			
			try{
				// Trigger an event to say the engine is about to start:
				Dom.Event e=new Dom.Event("scriptenginestart");
				htmlDocument.dispatchEvent(e);
				
				// Run it now:
				return Engine.Execute(source);
				
			}catch(Exception e){
				
				string scriptLocation=htmlDocument.ScriptLocation;
				
				if(string.IsNullOrEmpty(scriptLocation)){
					// Use document.basepath instead:
					scriptLocation=Document.basepath.ToString();
				}
				
				if(!string.IsNullOrEmpty(scriptLocation)){
					scriptLocation=" (At "+scriptLocation+")";
				}
				
				Dom.Log.Add("JavaScript compile error "+scriptLocation+": "+e);
				return Jint.Native.Undefined.Instance;
			}
			
		}
		
	}
	
}