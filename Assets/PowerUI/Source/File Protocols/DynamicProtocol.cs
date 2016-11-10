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
	/// This protocol, dynamic:// is for dynamic textures.
	/// They draw directly to the texture atlas and achieve high performance.
	/// Used by e.g. Curved Healthbars.
	/// </summary>
	
	public class DynamicProtocol:FileProtocol{
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
		public override string[] GetNames(){
			
			return new string[]{"dynamic","dyn"};
			
		}
		
		/// <summary>Always use dyn cotent type.</summary>
		public override string ContentType(Location path){
			return "dyn";
		}
		
		public override void OnGetGraphic(ImagePackage package){
			
			// The package contents are always DynamicFormat (because of the ContentType above):
			DynamicFormat contents=package.Contents as DynamicFormat;
			
			// Get the texture:
			DynamicTexture dynamic=DynamicTexture.Get(package.location.absoluteNoHash);
			
			// Apply:
			contents.DynamicImage=dynamic;
			
			// Ready:
			package.Done();
			
		}
		
	}
	
}