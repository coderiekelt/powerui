using Dom;
using Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A mouse as an input. You can use multiple mice if needed - just add more to InputPointer.AllRaw.
	/// (Generally though if there's more than one then they're touch pointers).
	/// </summary>
	public class MousePointer : InputPointer{
		
		public MousePointer(){
			
		}
		
		/// <summary>The type of input pointer.</summary>
		public override string pointerType{
			get{
				return "mouse";
			}
		}
		
		public override bool Relocate(out Vector2 delta){
			
			// Get the current mouse position:
			Vector2 position=UnityEngine.Input.mousePosition;
			
			// MousePosition's Y value is inverted, so flip it:
			position.y=ScreenInfo.ScreenY-1f-position.y;
			
			// Moved?
			if(position.x==ScreenX && position.y==ScreenY){
				
				// Nope!
				delta=Vector2.zero;
				return false;
				
			}
			
			// Delta:
			delta=new Vector2(
				position.x - ScreenX,
				position.y - ScreenY
			);
			
			// Update position:
			ScreenX=position.x;
			ScreenY=position.y;
			
			return true;
			
		}
		
	}
	
}