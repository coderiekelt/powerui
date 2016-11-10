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
using Blaze;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// A block of two triangles in a dynamic mesh.
	/// The triangles are positioned to create a flat 2D rectangle.
	/// The colour and position can be adjusted to fit content onto the block.
	/// </summary>
	
	public class MeshBlock{
		
		private static UVBlock BlankUV=new UVBlock(2f,2f,2f,2f);
		
		/// <summary>The index of this block in the buffer.</summary>
		public int BlockIndex;
		
		/// <summary>The index of the first vertex in the buffer.</summary>
		public int VertexIndex{
			get{
				return BlockIndex * 4;
			}
		}
		
		/// <summary>The first triangle index.</summary>
		public int TriangleIndex{
			get{
				return BlockIndex * 6;
			}
		}
		
		/// <summary>The buffer that this block is from.</summary>
		public BlockBuffer Buffer;
		
		/// <summary>The colour of the whole block. Applied to vertex colours.</summary>
		public Color Colour;
		/// <summary>The UV coordinate block for a letter off the atlas.</summary>
		public UVBlock TextUV;
		/// <summary>The UV coordinate for an image off the atlas.</summary>
		public UVBlock ImageUV;
		
		/// <summary>The vertex in the top left corner.</summary>
		public Vector3 VertexTopLeft;
		/// <summary>The vertex in the top right corner.</summary>
		public Vector3 VertexTopRight;
		/// <summary>The vertex in the bottom left corner.</summary>
		public Vector3 VertexBottomLeft;
		/// <summary>The vertex in the bottom right corner.</summary>
		public Vector3 VertexBottomRight;
		
		/*
		public bool Overlaps(MeshBlock block){
			
			// Check if any of blocks 4 corners are within this square.
			
			Vector3[] vertBuffer=block.ParentMesh.Vertices.Buffer;
			Vector3[] myBuffer=ParentMesh.Vertices.Buffer;
			
			// Apply inverse transform to blocks corners:
			for(int i=0;i<4;i++){
				
				// Get the point:
				Vector3 point=vertBuffer[block.VertexIndex+i];
				Vector3 boxPoint=myBuffer[VertexIndex+2];
				
				// Simple box test:
				if(point.x<boxPoint.x || point.x>myBuffer[VertexIndex+3].x || point.y<boxPoint.y || point.y>myBuffer[VertexIndex].y){
					continue;
				}
				
				return true;
				
			}
			
			return false;
		}
		*/
		
		/// <summary>Sets the vertex colours of this block.</summary>
		/// <param name="colour">The colour to set them to.</param>
		public void SetColour(Color colour){
			Colour=colour;
		}
		
		/// <summary>Sets the vertices of this box to that specified by the given block
		/// but clipped to fit within a boundary.</summary>
		/// <param name="boundary">The clipping boundary. The vertices will be clipped to within this.</param>
		/// <param name="block">The position of the vertices.</param>
		/// <param name="zIndex">The depth of the vertices.</param>
		public void SetClipped(BoxRegion boundary,BoxRegion block,Renderman renderer,float zIndex){
			// Clipping with no image/ affect on UVs:
			block.ClipBy(boundary);
			// And just apply the result:
			ApplyVertices(block,renderer,zIndex);
		}
		
		/// <summary>Sets the vertices of this box to that specified by the given block
		/// but clipped to fit within a boundary. At the same time, an image is applied
		/// to the block and its UV coordinates are also clipped.</summary>
		/// <param name="boundary">The clipping boundary. The vertices will be clipped to within this.</param>
		/// <param name="block">The position of the vertices.</param>
		/// <param name="renderer">The renderer that will render this block.</param>
		/// <param name="zIndex">The depth of the vertices.</param>
		/// <param name="imgLocation">The location of the image on the meshes atlas.</param>
		public UVBlock SetClipped(BoxRegion boundary,BoxRegion block,Renderman renderer,float zIndex,AtlasLocation imgLocation,UVBlock uvBlock){
			
			// Image defines how big we want the image to be in pixels on the screen.
			// So firstly we need to find the ratio of how scaled our image actually is:
			float originalHeight=block.Height;
			float scaleX=imgLocation.Width/block.Width;
			float scaleY=imgLocation.Height/originalHeight;
			
			// We'll need to clip block and make sure the image block is clipped too:
			float blockX=block.X;
			float blockY=block.Y;
			
			if(block.ClipByChecked(boundary)){
				
				// It actually got clipped - time to do some UV clipping too.
				
				// Apply the verts:
				ApplyVertices(block,renderer,zIndex);
				
				block.X-=blockX;
				block.Y-=blockY;
				block.MaxX-=blockX;
				block.MaxY-=blockY;
				
				// Flip the gaps (the clipped and now 'missing' sections) - UV's are inverted relative to the vertices.
				
				// Bottom gap is just block.Y:
				float bottomGap=block.Y;
				
				// Top gap is the original height - the new maximum; write it to the bottom gap:
				block.Y=originalHeight-block.MaxY;
				
				// Update the top gap:
				block.MaxY=originalHeight-bottomGap;
				
				// Image was in terms of real screen pixels, so now we need to scale it to being in 'actual image' pixels.
				// From there, the region 
				block.X*=scaleX;
				block.MaxX*=scaleX;
				
				block.Y*=scaleY;
				block.MaxY*=scaleY;
				
				if(uvBlock==null || uvBlock.Shared){
					
					// Create the UV block:
					uvBlock=new UVBlock();
					
				}
				
				// Get the new max/min values:
				uvBlock.MinX=imgLocation.GetU(block.X+0.2f);
				uvBlock.MaxX=imgLocation.GetU(block.MaxX-0.2f);
				uvBlock.MaxY=imgLocation.GetV(block.MaxY-0.2f);
				uvBlock.MinY=imgLocation.GetV(block.Y+0.2f);
				
			}else{
				
				// Apply the verts:
				ApplyVertices(block,renderer,zIndex);
				
				// Globally share the UV!
				uvBlock=imgLocation;
				
			}
			
			return uvBlock;
			
		}
		
		/// <summary>Applies the SDF outline "location", essentially the thickness of an outline, to this block.</summary>
		public void ApplyOutline(float location){
			
			if(Buffer.UV3==null){
				// UV3 is now required:
				Buffer.RequireUV3();
			}
			
			// Apply the values to the tangents:
			Vector2[] buffer=Buffer.UV3;
			
			Vector2 tangent=new Vector2(location,location);
			
			for(int i=0;i<4;i++){
				buffer[VertexIndex+i]=tangent;
			}
			
		}
		
		/// <summary>This locates the vertices of this block in world space to the position defined by the given box.</summary>
		/// <param name="block">The position of the vertices in screen coordinates.</param>
		/// <param name="renderer">The renderer used when rendering this block.</param>
		/// <param name="zIndex">The depth of the vertices.</param>
		private void ApplyVertices(BoxRegion block,Renderman renderer,float zIndex){
		
			// Compute the min/max pixels:
			Vector3 min=renderer.PixelToWorldUnit(block.X,block.Y,zIndex);
			Vector3 max=renderer.PixelToWorldUnit(block.MaxX,block.MaxY,zIndex);
			
			// Get the 4 corners:
			VertexTopLeft=min;
			VertexBottomRight=max;
			VertexTopRight=new Vector3(max.x,min.y,min.z); 
			VertexBottomLeft=new Vector3(min.x,max.y,min.z);
			
		}
		
		/// <summary>Sets the UV and image on this block to that of the solid colour pixel.</summary>
		public void SetSolidColourUV(){
			
			// Set the UVs - solid colour is always at y>1:
			ImageUV=null;
			TextUV=null;
			
		}
		
		/// <summary>Advances to the following block.
		public void Next(){
			
			BlockIndex++;
			
			if(BlockIndex==MeshDataBufferPool.BlockCount){
				
				// Advance a buffer:
				Buffer=Buffer.Next;
				BlockIndex=0;
				
			}
			
		}
		
		/// <summary>Writes out the given colour right now.</summary>
		public void PaintColour(Color colour){
			
			int vertexIndex=VertexIndex;
			
			// Apply the colour:
			Color[] buffer=Buffer.Colours;
			
			#if LINEAR
				colour=colour.linear;
			#endif
			
			for(int i=0;i<4;i++){
				buffer[vertexIndex+i]=colour;
			}
			
		}
		
		/// <summary>Writes out the data to the meshes buffers.</summary>
		public void Done(Transformation transform){
			// Apply the vertices:
			Vector3[] verts=Buffer.Vertices;
			Vector3[] normals=Buffer.Normals;
			int vertexIndex=VertexIndex;
			int triangleIndex=TriangleIndex;
			int[] triangles=Buffer.Triangles;
			
			// Write the triangles:
			int triOffset=Buffer.Offset + vertexIndex;
			
			// First triangle - Top left corner:
			triangles[triangleIndex++]=triOffset;
			
			// Top right corner:
			triangles[triangleIndex++]=triOffset+1;
			
			// Bottom left corner:
			triangles[triangleIndex++]=triOffset+2;
			
			// Second triangle - Top right corner:
			triangles[triangleIndex++]=triOffset+1;
			
			// Bottom right corner:
			triangles[triangleIndex++]=triOffset+3;
			
			// Bottom left corner:
			triangles[triangleIndex++]=triOffset+2;
			
			// Vertices next!
			
			if(transform!=null){
				
				// Top Left:
				verts[vertexIndex]=transform.Apply(VertexTopLeft);
				
				// Top Right:
				verts[vertexIndex+1]=transform.Apply(VertexTopRight);
				
				// Bottom Left:
				verts[vertexIndex+2]=transform.Apply(VertexBottomLeft);
				
				// Bottom Right:
				verts[vertexIndex+3]=transform.Apply(VertexBottomRight);
				
				if(normals!=null){
					
					// Top Left:
					normals[vertexIndex]=transform.Apply(new Vector3(0f,0f,-1f));
					
					// Top Right:
					normals[vertexIndex+1]=transform.Apply(new Vector3(0f,0f,-1f));
					
					// Bottom Left:
					normals[vertexIndex+2]=transform.Apply(new Vector3(0f,0f,-1f));
					
					// Bottom Right:
					normals[vertexIndex+3]=transform.Apply(new Vector3(0f,0f,-1f));
					
				}
				
			}else{
				
				// Top Left:
				verts[vertexIndex]=VertexTopLeft;
				
				// Top Right:
				verts[vertexIndex+1]=VertexTopRight;
				
				// Bottom Left:
				verts[vertexIndex+2]=VertexBottomLeft;
				
				// Bottom Right:
				verts[vertexIndex+3]=VertexBottomRight;
				
				if(normals!=null){
					
					// Top Left:
					normals[vertexIndex]=new Vector3(0f,0f,-1f);
					
					// Top Right:
					normals[vertexIndex+1]=new Vector3(0f,0f,-1f);
					
					// Bottom Left:
					normals[vertexIndex+2]=new Vector3(0f,0f,-1f);
					
					// Bottom Right:
					normals[vertexIndex+3]=new Vector3(0f,0f,-1f);
					
				}
				
			}
			
			// The UVs:
			if(ImageUV!=null){
				ImageUV.Write(Buffer.UV1,vertexIndex);
			}else{
				BlankUV.Write(Buffer.UV1,vertexIndex);
			}
			
			if(TextUV!=null){
				TextUV.Write(Buffer.UV2,vertexIndex);
			}else{
				BlankUV.Write(Buffer.UV2,vertexIndex);
			}
			
			// Apply the colour:
			Color[] buffer=Buffer.Colours;
			
			#if LINEAR
				Color colour=Colour.linear;
			#else
				Color colour=Colour;
			#endif
			
			for(int i=0;i<4;i++){
				buffer[vertexIndex+i]=colour;
			}
			
		}
		
	}
	
}