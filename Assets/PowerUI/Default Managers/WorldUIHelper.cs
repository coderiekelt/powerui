using UnityEngine;
using System.Collections;


namespace PowerUI{
	
	/// <summary>
	/// A helper for creating WorldUI's. This helper is totally optional - see the WorldUI and FlatWorldUI classes.
	/// </summary>
	
	public class WorldUIHelper : PowerUI.Manager { // It's a manager so it can share the same inspector properties
		
		/// <summary>Is input enabled for WorldUI's?</summary>
		[Tooltip("Disable input for extra performance")]
		public bool InputEnabled=true;
		/// <summary>The pixel width of the virtual screen.</summary>
		[Tooltip("Height comes from the aspect ratio of your scale.")]
		public int PixelWidth=800;
		/// <summary>Makes the UI always face the camera.</summary>
		[Tooltip("It'll always face a camera (Camera.main by default)")]
		public bool AlwaysFaceTheCamera;
		/// <summary>Set the WorldUI into PP mode.</summary>
		[Tooltip("The UI will scale itself so it's always pixel perfect")]
		public bool PixelPerfect;
		/// <summary>The expiry for the WorldUI.</summary>
		[Tooltip("The amount of seconds until the WorldUI destroys itself. 0 means no expiry time.")]
		public float Expiry=0f;
		
		
		public override void OnEnable () {
			
			// Dump renderer/filter:
			MeshRenderer mr=gameObject.GetComponent<MeshRenderer>();
			
			// True if we've got the visual guide:
			bool hasGuide=false;
			
			if( mr!=null && mr.material!=null && mr.material.name.StartsWith("worldUIMaterial") ){
				
				// Remove it:
				GameObject.Destroy(mr);
				
				hasGuide=true;
				
				// Remove filter too:
				MeshFilter filter=gameObject.GetComponent<MeshFilter>();
				
				if(filter!=null){
					
					GameObject.Destroy(filter);
					
				}
				
			}
			
			// First, figure out the 'aspect ratio' of the scale:
			Vector3 scale=transform.localScale;
			float yAspect=scale.y / scale.x;
			
			// Calc the number of pixels:
			int height=(int)((float)PixelWidth * yAspect);
			
			// Generate a new UI (the name is optional).
			// The two numbers are the dimensions of our virtual screen:
			WorldUI ui=new WorldUI(gameObject.name,PixelWidth,height);
			
			// Set the scale such that the width "fits".
			// As we're parented to something scaled 
			// we just need to scale it so it's x world units wide (a resolution of width/x):
			ui.SetResolution((float)PixelWidth / 10f);
			
			// Settings:
			ui.PixelPerfect=PixelPerfect;
			ui.AlwaysFaceCamera=AlwaysFaceTheCamera;
			
			if(Expiry!=0f){
				
				ui.SetExpiry(Expiry);
				
			}
			
			// Give it some content using PowerUIManager's Navigate method:
			Navigate(ui.document);
			
			// Parent it to the GO:
			ui.ParentToOrigin(transform);
			
			if(hasGuide){
				// Rotate it 90 degrees about x (to match up with the guide):
				ui.transform.localRotation=Quaternion.AngleAxis(90f,new Vector3(1f,0f,0f));
			}
			
			// Optionally accept input:
			ui.AcceptInput=InputEnabled;
			
		}
		
	}

}