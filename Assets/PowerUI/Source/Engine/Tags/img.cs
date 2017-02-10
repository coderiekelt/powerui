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


namespace PowerUI{
	
	/// <summary>
	/// Handles an image tag. The src attribute is supported.
	/// </summary>
	
	[Dom.TagName("img,image")]
	public class HtmlImageElement:HtmlElement{
		
		/// <summary>The aspect ratio of this image (height/width).</summary>
		public float AspectRatio;
		/// <summary>The image being loaded for this tag.</summary>
		public ImagePackage Image;
		/// <summary>The base width of the image.</summary>
		public float RawWidth=0f;
		/// <summary>The base height of the image.</summary>
		public float RawHeight=0f;
		/// <summary>The inverse aspect ratio of this image (height/width).</summary>
		public float InverseAspectRatio;
		/// <summary>The SRC set, if there is one.</summary>
		public ContentGroup SrcSet;
		/// <summary>True when the tag has been fully parsed.</summary>
		private bool Loaded;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				// Always self closing:
				lexer.Push(this,false);
				
				// Also blocks framesets though:
				lexer.FramesetOk=false;
				
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
		
		/// <summary>Re-applies the src.</summary>
		public void UpdateSrc(){
			
			string src=null;
			
			// Do we have an srcset?
			if(SrcSet!=null){
				
				// Select best src from the srcset (by density only for now):
				ContentEntry ce=SrcSet.BestByDensity;
				
				if(ce!=null){
					src=ce.Src;
				}
				
			}else{
				
				// Use src:
				src=this["src"];
				
			}
			
			if(src!=null){
				src=src.Trim();
			}
			
			if(!string.IsNullOrEmpty(src)){
				// Build it as a CSS url:
				src="url(\""+src.Replace("\"","\\\"")+"\")";
			}
			
			// Set it now:
			Style.Computed.ChangeTagProperty("background-image",src);
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				string src=this["src"];
				
				if(src==null){
					src="";
				}
				
				if(SrcSet!=null){
					
					// Add an SRC to it:
					SrcSet.AddSrc(src);
					
				}
				
				if(Loaded){
					UpdateSrc();
				}
				
				return true;
			}else if(property=="srcset"){
				
				string srcset=this["srcset"];
				
				if(string.IsNullOrEmpty(srcset)){
					SrcSet=null;
				}else{
					
					// Create the group:
					SrcSet=new ContentGroup();
					
					// Add 'src'
					string srcValue=this["src"];
					
					if(srcValue!=null){
						SrcSet.AddSrc(srcValue);
					}
					
					// Load each value from the set. Split by comma:
					string[] srcs=srcset.Split(',');
					
					for(int i=0;i<srcs.Length;i++){
						
						// Get the row:
						string[] srcEntry=srcs[i].Trim().Split(' ');
						
						// Descriptor (e.g. '2x' or '100w')
						string descriptor=(srcEntry.Length==1) ? null : srcEntry[1];
						
						// Add it:
						SrcSet.Add(srcEntry[0],descriptor);
						
					}
					
				}
				
				if(Loaded){
					UpdateSrc();
				}
				
				return true;
			}else if(property=="sizes"){
				
				// TODO
				
			}
			
			return false;
		}
		
		public override void OnChildrenLoaded(){
			
			Loaded=true;
			UpdateSrc();
		
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			// Replaced:
			box.OrdinaryInline=false;
			
			if(widthUndefined){
				
				if(heightUndefined){
					
					// Both undefined - use the base:
					box.InnerWidth=RawWidth;
					box.InnerHeight=RawHeight;
					
				}else{
					
					// Apply width from the height:
					box.InnerWidth=box.InnerHeight * AspectRatio;
					
				}
				
			}else if(heightUndefined){
				
				// Apply height from the width:
				box.InnerHeight=box.InnerWidth * InverseAspectRatio;
				
			}
			
			// They're always defined by the end of this:
			widthUndefined=false;
			heightUndefined=false;
			
		}
		
		public override void OnLoadEvent(Dom.Event e){
			
			BackgroundImage bgImage=RenderData.BGImage;
			
			if(bgImage==null){
				return;
			}
			
			Image=bgImage.Image;
			
			if(Image==null){
				return;
			}
			
			float width=(float)Image.Width;
			float height=(float)Image.Height;
			
			// Figure out the aspect ratios:
			AspectRatio=width/height;
			InverseAspectRatio=height/width;
			
			// Cache w/h:
			RawWidth=width;
			RawHeight=height;
			
			// Request layout:
			bgImage.RequestLayout();
			
		}
		
	}
	
}