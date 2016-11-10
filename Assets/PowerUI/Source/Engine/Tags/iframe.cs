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

using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles iframes.
	/// Supports the src="" attribute.
	/// </summary>
	
	[Dom.TagName("iframe")]
	public class HtmlIframeElement:HtmlElement{
		
		/// <summary>The src of the page this iframe points to.</summary>
		public string Src;
		/// <summary>True if the tag for this iframe has been loaded.</summary>
		public bool Loaded;
		/// <summary>The document in this iframe.</summary>
		public HtmlDocument ContentDocument;
		
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.FramesetOk=false;
				
				lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.Rawtext);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				Src=this["src"];
				LoadContent();
				return true;
			}
			
			return false;
		}
		
		/// <summary>Loads the content of this iframe now.</summary>
		private void LoadContent(){
			if(!Loaded || string.IsNullOrEmpty(Src)){
				return;
			}
			
			Location parentLocation=null;
			
			if(parentNode!=null){
				parentLocation=document.basepath;
			}
			
			// Load now:
			ContentDocument.location=new Location(Src,parentLocation);
			
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			// Update viewport:
			if(ContentDocument==null){
				return;
			}
			
			ContentDocument.Viewport.Height=box.InnerHeight;
			ContentDocument.Viewport.Width=box.InnerWidth;
			
		}
		
		public float InnerWidth{
			get{
				
				ComputedStyle computed=Style.Computed;
				
				Css.Value value=computed[Css.Properties.Width.GlobalProperty];
				
				// Note: Value is never null.
				
				return value.GetDecimal(RenderData,Css.Properties.Width.GlobalProperty);
				
			}
		}
		
		public float InnerHeight{
			get{
				
				ComputedStyle computed=Style.Computed;
				
				Css.Value value=computed[Css.Properties.Height.GlobalProperty];
				
				// Note: Value is never null.
				
				return value.GetDecimal(RenderData,Css.Properties.Height.GlobalProperty);
				
			}
		}
		
		public override void OnChildrenLoaded(){
			Loaded=true;
			
			// Iframes generate a new document object for isolation purposes:
			ContentDocument=new HtmlDocument(htmlDocument.Renderer,htmlDocument.window);
			
			// Setup initial viewport:
			ContentDocument.Viewport.Width=InnerWidth;
			ContentDocument.Viewport.Height=InnerHeight;
			
			// Setup the iframe ref:
			ContentDocument.window.iframe=this;
			
			// Append the document as a child of the iframe:
			appendChild(ContentDocument);
			
			LoadContent();
			
			// And handle style/ other defaults:
			base.OnTagLoaded();
		}
		
	}
	
}