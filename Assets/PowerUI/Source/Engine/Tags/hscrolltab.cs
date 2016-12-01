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
	/// Handles the tab (draggable part) of a horizontal scrollbar.
	/// </summary>
	
	[Dom.TagName("hscrolltab")]
	public class HtmlHScrollElement:HtmlScrollTabElement{
		
		/// <summary>The start x location of the tab when the mouse clicked it.</summary>
		public float StartX;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override void DragTab(DragEvent clickEvent){
			
			// Track where the bar started off:
			StartX=RenderData.FirstBox.Position.Left;
			
		}
		
		public override bool UseX(){
			return true;
		}
		
		protected override void SetTabSize(float newSize){
			style.width=newSize+"fpx";
			ScrollBy(0,true,true);
		}
		
		public override float BarSize(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			return ScrollBar.RenderData.FirstBox.InnerWidth-SizeBefore()-SizeAfter();
		}
		
		public override float TabSize(){
			return RenderData.FirstBox.InnerWidth;
		}
		
		public override float StyleSize(){
			// Grab the box:
			LayoutBox box=RenderData.FirstBox;
			
			// Return the horizontal style size. That's:
			return box.Width-box.InnerWidth;
		}
		
		public override float SizeBefore(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			// Grab the first child:
			Node firstChild=ScrollBar.firstChild;
			
			if(firstChild==this){
				// No button before:
				return 0;
			}
			
			return (firstChild as HtmlElement ).style.Computed.FirstBox.Width;
		}
		
		public override float SizeAfter(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			// Grab the last child:
			Node lastChild=ScrollBar.lastChild;
			
			if(lastChild==this){
				// No button after:
				return 0;
			}
			
			return (lastChild as HtmlElement ).style.Computed.FirstBox.Width;
		}
		
		public override float TabPosition(){
			return RenderData.FirstBox.Position.Left;
		}
		
		/*
		public override void OnMouseMoveEvent(MouseEvent mouseEvent){
			float deltaX=mouseEvent.clientX-mouseEvent.trigger.DownDocumentX;
			
			if(deltaX==0){
				return;
			}
			
			ScrollBy(deltaX,false,true);
		}
		*/
		
		public override void ScrollTo(float location,bool scrollTarget){
			
			StartX=0;
			ScrollBy(location,false,scrollTarget);
			
		}
		
		public override void ScrollBy(float deltaX,bool fromCurrent,bool scrollTarget){
			// Scroll it by deltaX from StartX.
			float newLocation=deltaX;
			
			#warning incorrect
			LayoutBox box=RenderData.FirstBox;
			
			if(fromCurrent){
				newLocation+=box.Position.Left;
			}else{
				newLocation+=StartX;
			}
			
			// Get the size of the button before the tab:
			float sizeBefore=SizeBefore();
			
			float barSize=BarSize();
			
			float max=barSize+sizeBefore-box.Width;
			
			if(newLocation<sizeBefore){
				newLocation=sizeBefore;
			}else if(newLocation>max){
				newLocation=max;
			}
			
			if(newLocation==box.Position.Left){
				return;
			}
			
			style.left=newLocation+"fpx";
			
			if(scrollTarget){
				HtmlElement target=ScrollBar.scrollTarget;
				
				if(target!=null){
					float progress=(newLocation-sizeBefore)/barSize;
					
					target.style.Computed.ChangeTagProperty("scroll-left",(progress * target.style.Computed.FirstBox.ContentWidth)+"px");
					
					// Request a redraw:
					target.htmlDocument.RequestLayout();
					
				}
			}
			
		}
		
	}
	
}