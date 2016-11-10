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
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Automatically resizes the "actual" image to prevent wasting memory.
	/// With this, you can have one set of high-res images for all your devices and they'll just fit.
	/// Requests from Resources only.
	/// </summary>
	
	public class ResizeProtocol:ResourcesProtocol{
		
		public override string[] GetNames(){
			return new string[]{"resize"};
		}
		
		public override void OnGetGraphic(ImagePackage package){
			
			// Already resized?
			ResizedImage resized=ResizedImages.Get(package.location.Path);
			
			if(resized!=null){
				
				// Sure is!
				package.GotGraphic(resized.Image);
				
				return;
				
			}
			
			// Main thread only:
			Callback.MainThread(delegate(){
				
				// Try loading from resources:
				if(package.Contents.LoadFromResources(package.location,package)){
					
					PictureFormat pict=package.Contents as PictureFormat;
					
					if(pict!=null){
						// Resize the image:
						resized=ResizedImages.Add(package.location.Path,pict.Image as Texture2D);
						
						// Apply:
						pict.Image=resized.Image;
					}
					
					// Great, stop there:
					package.Done();
					return;
				}
				
				// Binary otherwise.
				
				// Note: the full file should be called something.bytes for this to work in Unity.
				TextAsset text=Resources.Load(package.location.Path) as TextAsset;
				
				if(text!=null){
					
					// Apply it now:
					package.ReceivedHeaders(text.bytes.Length);
					package.ReceivedData(text.bytes,0,text.bytes.Length);
					return;
					
				}
				
				package.Failed(404);
				
			});
			
		}
		
	}
	
}