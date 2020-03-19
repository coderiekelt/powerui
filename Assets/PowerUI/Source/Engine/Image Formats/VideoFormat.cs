//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright � 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

#if !MOBILE && !UNITY_WEBGL && !UNITY_TVOS

using System;
using Css;
using UnityEngine;
using Dom;
using UnityEngine.Video;


namespace PowerUI{
	
	/// <summary>
	/// Represents the video format.
	/// </summary>
	
	public class VideoFormat:ImageFormat{
		
		/// <summary>The video retrieved.</summary>
		public VideoPlayer Video;
		/// <summary>An isolated material for this image.</summary>
		private Material IsolatedMaterial;
		
		
		public override string[] GetNames(){
			return new string[]{"mov","mpg","mpeg","mp4","avi","asf","ogg","ogv"};
		}
		
		public override Material GetImageMaterial(Shader shader){
			
			if(IsolatedMaterial==null){
				IsolatedMaterial=new Material(shader);
				IsolatedMaterial.SetTexture("_MainTex",Video.texture);
			}
			
			return IsolatedMaterial;
			
		}
		
		public override Texture Texture{
			get{
				return Video.texture;
			}
		}
		
		public override bool LoadFromAsset(UnityEngine.Object asset,ImagePackage package){
			
			// Video
			Video=asset as VideoPlayer;
			
			if(Video!=null){
				return true;
			}
			
			return base.LoadFromAsset(asset,package);
		}
		
		public override void GoingOnDisplay(Css.RenderableData context){
			
			// Note that this is only called if Video is set.
			HtmlVideoElement videoElement=context.Node as HtmlVideoElement;
			
			if(videoElement==null){
				return;
			}
			
			if(!Video.isPlaying && videoElement["autoplay"]!=null){
				
				// Play now:
				videoElement.play();
				
				// Clear - don't autoplay again:
				videoElement["autoplay"]=null;
			}
			
		}
		
		public override ImageFormat Instance(){
			return new VideoFormat();
		}
		
		public override bool Isolate{
			get{
				return true;
			}
		}
		
		public override int Height{
			get{
				return (int)Video.height;
			}
		}
		
		public override int Width{
			get{
				return (int)Video.width;
			}
		}
		
		public override bool Loaded{
			get{
				return (Video!=null);
			}
		}
		
	}
	
}

#endif
