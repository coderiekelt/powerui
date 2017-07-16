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
using JavaScript;


namespace PowerUI{
	
	/// <summary>
	/// The default script handler for text/javascript-x.
	/// </summary>
	
	[JSProperties(Hidden=true)]
	public class JavaScriptEngine : PowerUI.ScriptEngine{
		
		/// <summary>An instance of the ScriptEngine on this page.</summary>
		public JavaScript.ScriptEngine Engine;
		
		/// <summary>The meta types that your engine will handle. E.g. "text/javascript".</summary>
		public override string[] GetTypes(){
			return new string[]{"text/javascript-x"};
		}
		
		public JavaScriptEngine(){}
		
		public JavaScriptEngine(bool safeHost,HtmlDocument doc,object window){
			
			// Setup the engine:
			Engine = new JavaScript.ScriptEngine(doc.SecurityDomain);
			Engine.FullAccess = safeHost;
			Engine.ImportGlobals(window);
			JavaScript.ScriptEngine.EnableILAnalysis=true;
			Document=doc;
			
			UnityEngine.Debug.Log("Nitrassic is not fully linked with PowerUI yet - many PowerUI DOM API's are currently unavailable (but they'll be with you shortly!)");
			
		}
		
		/// <summary>Gets or sets script variable values.</summary>
		/// <param name="index">The name of the variable.</param>
		/// <returns>The variable value.</returns>
		public override object this[string global]{
			get{
				if(Engine==null){
					return null;
				}
				
				return Engine.GetGlobal(global);
			}
			set{
				if(Engine==null){
					return;
				}
				
				Engine.SetGlobal(global,value);
			}
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="context">The context to use for the 'this' value.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public override object RunLiteral(string name,object context,object[] args,bool optional){
			if(string.IsNullOrEmpty(name)||Engine==null){
				return null;
			}
			
			// Args incl. our context:
			object[] argsWithThis = new object[args==null ? 1 : args.Length + 1];
			
			argsWithThis[0] = context;
			
			if(args!=null){
				for(int i=0;i<args.Length;i++){
					argsWithThis[i+1]=args[i];
				}
			}
			
			return Engine.CallGlobalFunction(name,optional,argsWithThis);
			
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
		
		protected override void Compile(string source){
			
			try{
				
				// Get the location:
				string location=null;
				
				if(Document.location!=null){
					location = Document.location.absoluteNoHash;
				}
				
				// Get the cache seed:
				string cacheSeed = location == null ? null : JavaScript.Cache.GetSeed(source,location);
				
				// Compile it:
				CompiledCode cc = Engine.Compile(cacheSeed,source);
				
				// Trigger an event to say the engine is about to start:
				Dom.Event e=new Dom.Event("scriptenginestart");
				htmlDocument.dispatchEvent(e);
				
				// Run it now:
				cc.Execute();
				
			}catch(Exception e){
				
				string scriptLocation=htmlDocument.ScriptLocation;
				
				if(string.IsNullOrEmpty(scriptLocation)){
					// Use document.basepath instead:
					scriptLocation=Document.basepath.ToString();
				}
				
				if(!string.IsNullOrEmpty(scriptLocation)){
					scriptLocation=" (At "+scriptLocation+")";
				}
				
				Dom.Log.Add("JavaScript compile error"+scriptLocation+": "+e);
			}
			
		}
		
	}
	
}