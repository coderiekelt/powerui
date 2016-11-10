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


namespace PowerUI{
	
	/// <summary>
	/// Represents the built in dynamic image format.
	/// </summary>
	
	public class DynamicFormat:ImageFormat{
		
		/// <summary>The dynamic image retrieved.</summary>
		public DynamicTexture DynamicImage;
		/// <summary>An isolated material for this image.</summary>
		private Material IsolatedMaterial;
		
		
		public override string[] GetNames(){
			return new string[]{"dyn"};
		}
		
		public override Material GetImageMaterial(ShaderSet shaders){
			
			if(IsolatedMaterial==null){
				IsolatedMaterial=new Material(shaders.Isolated);
				IsolatedMaterial.SetTexture("_Sprite",DynamicImage.GetTexture());
			}
			
			return IsolatedMaterial;
		}
		
		public override bool Isolate{
			get{
				return true;
			}
		}
		
		public override ImageFormat Instance(){
			return new DynamicFormat();
		}
		
		public override int Height{
			get{
				return DynamicImage.Height;
			}
		}
		
		public override int Width{
			get{
				return DynamicImage.Width;
			}
		}
		
		public override bool Loaded{
			get{
				return (DynamicImage!=null);
			}
		}
		
		public override void ClearX(){
			DynamicImage=null;
		}
		
	}
	
}