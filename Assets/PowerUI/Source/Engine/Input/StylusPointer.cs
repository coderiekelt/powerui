using Dom;
using Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A stylus input.
	/// </summary>
	public class StylusPointer : TouchPointer{
		
		/// <summary>The vertical angle. When it's Pi/2, it's standing vertically.</summary>
		public float VerticalAngle;
		/// <summary>The azimuth angle of the stylus.</summary>
		public float AzimuthAngle;
		
		/// <summary>Updates the stylus info.</summary>
		public override void UpdateStylus(float angleX,float angleY){
			
			AzimuthAngle=angleX;
			VerticalAngle=angleY;
			
		}
		
	}
	
}