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

using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles the resizer tab (appears when two scrollbars are visible, but can also be manually added).
	/// </summary>
	
	[Dom.TagName("resizer")]
	public class HtmlResizerElement:HtmlElement{
		
		/// <summary>Originates from the resize CSS property.</summary>
		private bool AllowX;
		/// <summary>Originates from the resize CSS property.</summary>
		private bool AllowY;
		/// <summary>The starting mouse values.</summary>
		public float PositionX;
		/// <summary>The starting mouse values.</summary>
		public float PositionY;
		/// <summary>The element being resized when this is dragged.</summary>
		public HtmlElement ToResize;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>The distance the input pointer must move in order to start dragging this element.</summary>
		public override float DragStartDistance{
			get{
				// Resizers start dragging immediately:
				return 1f;
			}
		}
		
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(e.type=="dragstart"){
				
				if(ToResize==null){
					ToResize=parentElement as HtmlElement;
				}
				
				// Track where the mouse started off:
				UIEvent ue=(e as UIEvent);
				
				PositionX=ue.trigger.DownDocumentX;
				PositionY=ue.trigger.DownDocumentY;
				
				// Get the CSS resize property:
				if(ToResize!=null){
					
					// Obtain its values:
					Css.Properties.Resize.Compute(ToResize.ComputedStyle,out AllowX,out AllowY);
					
				}
				
			}
			
			// Handle locally:
			return base.HandleLocalEvent(e,bubblePhase);
			
		}
		
		/// <summary>Called when the thumb is being dragged.</summary>
		public override void OnDrag(PowerUI.DragEvent mouseEvent){
			
			// Get the amount of pixels the pointer moved by:
			float deltaX=AllowX ? mouseEvent.clientX-PositionX : 0f;
			float deltaY=AllowY ? mouseEvent.clientY-PositionY : 0f;
			
			if(deltaX==0f && deltaY==0f){
				return;
			}
			
			// Update position:
			PositionX=mouseEvent.clientX;
			PositionY=mouseEvent.clientY;
			
			// Resize now!
			ComputedStyle cs=ToResize.Style.Computed;
			
			if(deltaX!=0f){
				
				// Width is..
				Css.Value width=cs[Css.Properties.Width.GlobalProperty];
				
				// Update it:
				deltaX+=width.GetDecimal(ToResize.RenderData,Css.Properties.Width.GlobalProperty);
				
				// Write it back out:
				cs.ChangeProperty(Css.Properties.Width.GlobalProperty,new Css.Units.DecimalUnit(deltaX));
				
			}
			
			if(deltaY!=0f){
				
				// Height is..
				Css.Value height=cs[Css.Properties.Height.GlobalProperty];
				
				// Update it:
				deltaY+=height.GetDecimal(ToResize.RenderData,Css.Properties.Height.GlobalProperty);
				
				// Write it back out:
				cs.ChangeProperty(Css.Properties.Height.GlobalProperty,new Css.Units.DecimalUnit(deltaY));
				
			}
			
		}
		
		public override void OnChildrenLoaded(){
			
			// We're always draggable:
			this["draggable"]="1";
			
		}
		
	}
	
}