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
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				string src=this["src"];
				
				if(src==null){
					src="";
				}
				
				ComputedStyle computed=Style.Computed;
				computed.ChangeTagProperty("background-image","url(\""+src.Replace("\"","\\\"")+"\")");
				return true;
			}
			return false;
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
		
		public override void OnLoadEvent(DomEvent e){
			
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