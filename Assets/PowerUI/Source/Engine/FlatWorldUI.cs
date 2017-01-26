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
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// Flat WorldUI's render to textures. Normal worldUI's are better for performance but flat worldUI's are more flexible. 
	/// Free/Indie users should use normal WorldUI's instead for performance purposes, however this can still be used.
	/// The difference here is the texture can be e.g. applied to a curved surface.
	/// </summary>
	
	public partial class FlatWorldUI:WorldUI{
		
		/// <summary>Global position offset.</summary>
		public static float GlobalOffset=-150f;
		
		/// <summary>The default layer to use for rendering FlatWorldUI's. If not set the PowerUI layer is used, Change by using yourWorldUI.Layer.</summary>
		private int DefaultLayer=-1;
		
		/// <summary>The raw texture. Note that for Unity Pro users, this is always a RenderTexture (and a Texture2D for free users).</summary>
		public Texture Texture;
		/// <summary>The internal camera being used to render it flat.</summary>
		public Camera SourceCamera;
		/// <summary>The internal camera's gameobject being used.</summary>
		public GameObject CameraObject;
		/// <summary>A handler which ensures the camera remains the right size.</summary>
		private FlatWorldUIHandler Handler;
		/// <summary>Flat world UI's get stacked up on top of each other in virtual space.
		/// This is true if it will update the offset of one UI to another when SetDimensions is called.</summary>
		private bool RequiresOffset;
		
		/// <summary>Creates a new Flat World UI with 100x100 pixels of space and a name of "new World UI".
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		public FlatWorldUI():this("new World UI",0,0){}
		
		/// <summary>Creates a new Flat World UI with 100x100 pixels of space and the given name.
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		/// <param name="name">The name for the UI's gameobject.</param>
		public FlatWorldUI(string name):this(name,0,0){}
		
		/// <summary>Creates a new Flat World UI with the given pixels of space and a name of "new World UI".
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		/// <param name="widthPX">The width in pixels of this UI.</param>
		/// <param name="heightPX">The height in pixels of this UI.</param>
		public FlatWorldUI(int widthPX,int heightPX):this("new World UI",widthPX,heightPX){}
		
		/// <summary>Creates a new Flat World UI with the given pixels of space and a given name.
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		/// <param name="name">The name for the UI's gameobject.</param>
		/// <param name="widthPX">The width in pixels of this UI.</param>
		/// <param name="heightPX">The height in pixels of this UI.</param>
		public FlatWorldUI(string name,int widthPX,int heightPX):base(name,widthPX,heightPX){
			
			// It's a flat UI:
			Flat=true;
			
			// Create camera gameobject:
			CameraObject=new GameObject();
			
			CameraObject.name=name+"-#camera";
			
			// Parent the camera to the root:
			CameraObject.transform.parent=gameObject.transform;
			CameraObject.transform.localPosition=Vector3.zero;
			
			// Add a camera:
			SourceCamera=CameraObject.AddComponent<Camera>();
			
			// Put it right at the back:
			SourceCamera.depth=-9999;
			
			// Set the clear flags:
			SourceCamera.clearFlags=CameraClearFlags.Color;
			SourceCamera.backgroundColor=new Color(1f,1f,1f,0f);
			
			// Make it forward rendered (it deals with transparency):
			SourceCamera.renderingPath=RenderingPath.Forward;
			
			// Setup the cameras distance:
			SetCameraDistance(UI.GetCameraDistance());
			
			// Call the camera creation method:
			UI.CameraGotCreated(SourceCamera);
			
			// Make it orthographic:
			SourceCamera.orthographic=true;
			
			// Set the orthographic size:
			SetOrthographicSize();
		
			// Start our handler:
			Handler=CameraObject.AddComponent<FlatWorldUIHandler>();
			
			// And the location too:
			Handler.Location=new Rect(0,0,widthPX,heightPX);
			
			// Apply aspect:
			Handler.Aspect=(float)widthPX / (float)heightPX;
			
			// Hook up the camera:
			Handler.Camera=SourceCamera;
			
			// Next it's time for the texture itself! We're going to try using a RenderTexture first.
			
			RenderTexture renderTexture=null;
			
			#if UNITY_5_5_OR_NEWER
			
			// Create a render texture:
			renderTexture=new RenderTexture(widthPX,heightPX,16,RenderTextureFormat.ARGB32);
			
			// Apply it to the texture:
			Texture=(Texture)renderTexture;
			
			// Hook it up:
			SourceCamera.targetTexture=renderTexture;
			
			#else
			
			if(SystemInfo.supportsRenderTextures){
				
				// Create a render texture:
				renderTexture=new RenderTexture(widthPX,heightPX,16,RenderTextureFormat.ARGB32);
				
				// Apply it to the texture:
				Texture=(Texture)renderTexture;
				
				// Hook it up:
				SourceCamera.targetTexture=renderTexture;
				
			}else{
				// No RT support. Time to use our workaround instead!
				
				// Create the texture:
				Texture2D texture=new Texture2D(widthPX,heightPX);
				
				// Hook it up now:
				Handler.Output=texture;
				
				// Apply it:
				Texture=(Texture)texture;
				
			}
			
			#endif
			
			// Change the layer of the gameobject and also the camera.
			
			// Set the culling mask:
			if(DefaultLayer==-1){
			
				Layer=UI.Layer;
				
			}else{
				
				Layer=DefaultLayer;
				
			}
			
			gameObject.transform.position=new Vector3(0f,GlobalOffset,0f);
			
			if(heightPX==0){
				RequiresOffset=true;
				GlobalOffset-=(float)heightPX * WorldPerPixel.y;
			}
		}
		
		/// <summary>Alias for Texture.</summary>
		public Texture texture{
			get{
				return Texture;
			}
		}
		
		/// <summary>Resolves a hit to a point on this WorldUI.
		/// Note that x and y are 'relative' in the -0.5 to +0.5 range.</summary>
		public override void ResolvePoint(RaycastHit hit,out float x,out float y){
			// Get the point in UV space as the collider could be anything:
			Vector2 point=hit.textureCoord;
			
			// Great - this time the point is from 0-1 in x and y.
			x=point.x-0.5f;
			y=point.y-0.5f;
		}
		
		/// <summary>Applies Texture to the sharedMaterial on the gameobject.</summary>
		public void ApplyTo(GameObject gameObject){
			
			MeshRenderer renderer=gameObject.GetComponent<MeshRenderer>();
			Material material;
			
			if(renderer==null){
				
				// Try skinned:
				SkinnedMeshRenderer sme=gameObject.GetComponent<SkinnedMeshRenderer>();
				
				if(sme==null){
					throw new Exception("That gameObject doesn't have a mesh renderer or a skinned mesh renderer.");
				}
				
				// Get the material:
				material=sme.sharedMaterial;
				
			}else{
				
				// Get the material:
				material=renderer.sharedMaterial;
				
			}
			
			// Apply it:
			material.mainTexture=Texture;
			
		}
		
		/// <summary>Applies Texture to material.mainTexture.</summary>
		public void ApplyTo(Material material){
			
			// Apply it:
			material.mainTexture=Texture;
			
		}
		
		/// <summary>Creates a material with the given shader and applies texture to its mainTexture property.</summary>
		public Material CreateMaterial(Shader shader){
			
			Material material=new Material(shader);
			
			material.mainTexture=Texture;
			
			return material;
			
		}
		
		/// <summary>Creates a Standard Shader material and applies texture to its mainTexture property.</summary>
		public Material CreateMaterial(){
			
			Material material=new Material(Shader.Find("Standard"));
			
			material.mainTexture=Texture;
			
			return material;
			
		}
		
		public override void RenderWithCamera(int layer){
			base.RenderWithCamera(layer);
			SourceCamera.cullingMask=(1<<layer);
		}
		
		/// <summary>Sets the distance of the camera, essentially defining the amount of z-index available.</summary>
		/// <param name="distance">The distance of the camera from the origin along the z axis in world units.</param>
		public void SetCameraDistance(float distance){
			SourceCamera.farClipPlane=distance*2f;
			CameraObject.transform.localPosition=new Vector3(0f,0f,-distance);
		}
		
		public override void SetOrigin(float x,float y){
			x=0.5f;
			y=0.5f;
			base.SetOrigin(x,y);
		}
		
		public override void SetResolution(float pp){
		}
		
		public override void SetResolution(int x,int y){
		}
		
		public override void SetDimensions(int x,int y){
			
			if(RequiresOffset){
				
				RequiresOffset=false;
				GlobalOffset-=(float)y * WorldPerPixel.y;
				
			}
			
			int currentHeight=pixelHeight;
			int currentWidth=pixelWidth;
			
			// Set the base dimensions:
			base.SetDimensions(x,y);
			
			if(Handler!=null){
				// Update handler location too:
				Handler.Location=new Rect(0,0,x,y);
				
				// Apply aspect:
				Handler.Aspect=(float)x / (float)y;
			}
			
			if(Texture!=null){
				
				if(Texture.GetType()==typeof(RenderTexture)){
					
					// Update the texture size:
					if(y!=currentHeight){
						Texture.height=y;
					}
					
					if(x!=currentWidth){
						Texture.width=x;
					}
					
				}else{
					
					Debug.LogWarning("Resizing a Flat world UI without RT support is inefficient.");
					
				}
				
			}
			
			// Set the orthographic size:
			SetOrthographicSize();
			
		}
		
		private void SetOrthographicSize(){
			
			if(SourceCamera==null){
				return;
			}
			
			// Grab the world per pixel size:
			SourceCamera.orthographicSize=pixelHeight / 2f * WorldPerPixel.y;
			
		}
		
	}
	
}