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
using PowerUI;
using Blaze;


namespace Css{
	
	/// <summary>
	/// Helps with the process of rastering elements. Used by the CSS filter property.
	/// </summary>
	
	public partial class RasterDisplayableProperty:DisplayableProperty{
		
		/// <summary>The raster output.
		/// Usually originates from either the FlatWorldUI renderer or via a filter.</summary>
		public Texture Output;
		/// <summary>A material displaying the output.</summary>
		private Material Material;
		/// <summary>A flatWorldUI which helps with the process of rendering the element.</summary>
		public FlatWorldUI Renderer;
		/// <summary>An AtlasLocation object used to describe where the image is.</summary>
		private AtlasLocation LocatedAt;
		/// <summary>A filter to apply.</summary>
		public Loonim.SurfaceTexture Filter;
		/// <summary>The draw info which draws the filter.</summary>
		public Loonim.DrawInfo FilterDrawInfo;
		
		
		/// <summary>Creates a new solid background colour property for the given element.</summary>
		/// <param name="data">The renderable object to give a bg colour to.</param>
		public RasterDisplayableProperty(RenderableData data):base(data){
		}
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				// Must be the very first thing.
				return 1;
			}
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			// Does the given renderer belong to the worldUI?
			if(Renderer!=null && renderer==Renderer.Renderer){
				
				// Yes! We're actually drawing the element. Do nothing.
				return;
				
			}
			
			MeshBlock block=GetFirstBlock(renderer);
			
			if(block==null){
				// This can happen if an animation is requesting that a now offscreen element gets painted only.
				return;
			}
			
			block.PaintColour(renderer.ColorOverlay);
			
		}
		
		/// <summary>Sets a filter to apply. This is what rasterising elements is all for!</summary>
		public void SetFilter(Loonim.SurfaceTexture tex){
			
			Filter=tex;
			
			if(tex==null){
				
				if(FilterDrawInfo!=null){
					
					// Tidy it up:
					FilterDrawInfo.Clear();
					
					FilterDrawInfo=null;
				}
				
				if(Renderer!=null){
					
					// Update it now:
					Output=Renderer.Texture;
					
					if(Material!=null){
						
						// Hook up the output:
						Material.SetTexture("_Sprite",Output);
						
					}
					
				}
				
			}else{
				
				// Create the draw info (for GPU mode):
				FilterDrawInfo=new Loonim.DrawInfo();
				
				if(Renderer!=null){
					
					// Update it now:
					Filter.Set("source1",Renderer.Texture);
					
					// Note that the next draw will update Output for us.
					
				}
				
			}
			
		}
		
		internal override void NowOffScreen(){
			
			if(Renderer!=null){
				
				FlatWorldUI fwui=Renderer;
				Renderer=null;
				
				// Make sure we're on the main thread:
				Callback.MainThread(delegate(){
					
					// Destroy it:
					fwui.Destroy();
					
				});
				
			}
			
		}
		
		/// <summary>Destroys this RDP.</summary>
		public void Destroy(){
			
			// Destroy it:
			if(Renderer!=null){
				Renderer.Destroy();
				Renderer=null;
			}
			
		}
		
		/// <summary>Updates the FlatWorldUI so it builds the mesh for this element.</summary>
		private void UpdateRenderer(LayoutBox box,float width,float height){
			
			// - Set w/h to width and height:
			int w=(int)width;
			int h=(int)height;
			
			if(Renderer.SetDimensions(w,h)){
				
				// Output texture changed.
				
				if(Filter==null){
					
					// Update output:
					Output=Renderer.Texture;
					
					if(Material!=null){
						
						// Hook up the output:
						Material.SetTexture("_Sprite",Output);
						
					}
					
				}else{
					
					Filter.Set("source1",Renderer.Texture);
					// Output will be updated shortly.
					
					// Always mark as changed:
					Filter.Changed=true;
					
				}
				
			}
			
			// Redraw:
			if(Filter!=null){
				
				if(FilterDrawInfo!=null){
					FilterDrawInfo.SetSize(w,h);
				}
				
				// Draw now:
				Output=Filter.Draw(FilterDrawInfo);
				
				if(Material!=null){
					
					// Hook up the output:
					Material.SetTexture("_Sprite",Output);
					
				}
				
			}
			
			// Temporarily set the positioning of box such that it's at the origin:
			float _x=box.X;
			float _y=box.Y;
			float _pX=box.ParentOffsetLeft;
			float _pY=box.ParentOffsetTop;
			int _pos=box.PositionMode;
			
			// Clear:
			box.X=-box.Border.Left;
			box.Y=-box.Border.Top;
			box.ParentOffsetTop=box.X;
			box.ParentOffsetLeft=box.Y;
			box.PositionMode=PositionMode.Fixed;
			
			// Put the RenderData in the render only queue of *Renderer* and ask it to layout now:
			RenderableData _next=RenderData.Next;
			UpdateMode _mode=RenderData.NextUpdateMode;
			
			// Clear:
			RenderData.Next=null;
			RenderData.NextUpdateMode=UpdateMode.Render;
			
			// Queue:
			Renderer.Renderer.StylesToUpdate=RenderData;
			
			// Draw now!
			Renderer.Renderer.Update();
			
			// Restore (box):
			box.X=_x;
			box.Y=_y;
			box.ParentOffsetTop=_pX;
			box.ParentOffsetLeft=_pY;
			box.PositionMode=_pos;
			
			// Restore (queue):
			RenderData.Next=_next;
			RenderData.NextUpdateMode=_mode;
			
		}
		
		/// <summary>A unique identifier.</summary>
		private static int RasterID=1;
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			// Dimensions:
			float width=box.Width;
			float height=box.Height;
			
			if(Renderer==null){
				
				// Create the FWUI now:
				Renderer=new FlatWorldUI("#Internal-PowerUI-Raster-"+RasterID,(int)width,(int)height);
				RasterID++;
				
				if(Filter!=null){
					// Set source:
					Filter.Set("source1",Renderer.Texture);
				}
				
				// Grab the output texture:
				Output=Renderer.Texture;
				
			}
			
			// Does the given renderer belong to the worldUI?
			if(renderer==Renderer.Renderer){
				
				// Yes! We're actually drawing the element.
				
				return;
				
			}
			
			// Next we'll draw the rastered image.
			// It's essentially just the output from the renderer.
			
			// Get the top left inner corner (inside margin and border):
			float top=box.Y;
			float left=box.X;
			
			// Update the FlatWorldUI next:
			UpdateRenderer(box,width,height);
			
			// Always isolated:
			Isolate();
			
			// Make sure the renderer stalls and doesn't draw anything else of this element or its kids.
			renderer.StallStatus=2;
			
			// Setup boundary:
			BoxRegion boundary=new BoxRegion(left,top,width,height);
			
			if(!boundary.Overlaps(renderer.ClippingBoundary)){
				
				if(Visible){
					SetVisibility(false);
				}
				
				return;
			}else if(!Visible){
				
				// ImageLocation will allocate here if it's needed.
				SetVisibility(true);
				
			}
			
			// Texture time - get its location on that atlas:
			if(LocatedAt==null){
				LocatedAt=new AtlasLocation(width,height);
			}else{
				
				// Dimensions changed?
				int w=(int)width;
				int h=(int)height;
			
				if(LocatedAt.Width!=w || LocatedAt.Height!=h){
					
					// Update it:
					LocatedAt.UpdateFixed(width,height);
					
				}
				
			}
			
			boundary.ClipBy(renderer.ClippingBoundary);
			
			// Ensure we have a batch:
			renderer.SetupBatch(this,null,null);
			
			if(Material==null){
				
				// Create the material now using the isolated shader:
				Material=new Material(renderer.CurrentShaderSet.Isolated);
				
				// Hook up the output:
				Material.SetTexture("_Sprite",Output);
				
			}
			
			// Allocate the block:
			MeshBlock block=Add(renderer);
			
			// Set current material:
			SetBatchMaterial(renderer,Material);
			
			// Set the (overlay) colour:
			block.SetColour(renderer.ColorOverlay);
			block.TextUV=null;
			
			// Z-index (same as a background-image):
			float zIndex = (RenderData.computedStyle.ZIndex-0.003f);
			
			BoxRegion screenRegion=new BoxRegion();
			screenRegion.Set(left,top,width,height);
			
			// Setup the block:
			block.ImageUV=block.SetClipped(boundary,screenRegion,renderer,zIndex,LocatedAt,block.ImageUV);
			
			// Flush it:
			block.Done(renderer.Transform);
			
		}
		
	}
	
	public partial class RenderableData{
		
		/// <summary>Gets or sets a RasterDisplayableProperty.</summary>
		public RasterDisplayableProperty RasterProperty{
			get{
				return GetProperty(typeof(RasterDisplayableProperty)) as RasterDisplayableProperty;
			}
			set{
				AddOrReplaceProperty(value,typeof(RasterDisplayableProperty));
			}
		}
		
	}
	
}