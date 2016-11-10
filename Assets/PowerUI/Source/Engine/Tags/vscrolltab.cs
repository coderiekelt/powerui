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
	/// Handles the tab (draggable part) of a vertical scrollbar.
	/// </summary>
	
	[Dom.TagName("vscrolltab")]
	public class HtmlVScrollElement:HtmlScrollTabElement{
		
		/// <summary>The start y location of the tab when the mouse clicked it.</summary>
		public float StartY;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override void DragTab(DragEvent clickEvent){
			
			// Track where the bar started off:
			StartY=RenderData.FirstBox.Position.Top;
			
		}
		
		protected override void SetTabSize(float newSize){
			style.height=newSize+"fpx";
			ScrollBy(0,true,true);
		}
		
		public override float BarSize(){
			GetScrollBar();
			
			if(ScrollBar==null){
				return 0;
			}
			
			return ScrollBar.RenderData.FirstBox.InnerHeight-SizeBefore()-SizeAfter();
		}
		
		public override float TabSize(){
			return RenderData.FirstBox.InnerHeight;
		}
		
		public override float StyleSize(){
			// Grab the box:
			LayoutBox box=RenderData.FirstBox;
			
			// Return the vertical style size. That's:
			return box.Height-box.InnerHeight;
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
			
			return (firstChild as HtmlElement ).style.Computed.FirstBox.Height;
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
			
			return (lastChild as HtmlElement ).style.Computed.FirstBox.Height;
		}
		
		public override float TabPosition(){
			return RenderData.FirstBox.Position.Top;
		}
		
		public override void OnMouseMoveEvent(MouseEvent mouseEvent){
			float deltaY=mouseEvent.clientY-mouseEvent.trigger.DownDocumentY;
			
			if(deltaY==0){
				return;
			}
			
			ScrollBy(deltaY,false,true);
		}
		
		public override void ScrollTo(float location,bool scrollTarget){
			
			StartY=0;
			ScrollBy(location,false,scrollTarget);
			
		}
		
		public override void ScrollBy(float deltaY,bool fromCurrent,bool scrollTarget){
			// Scroll it by deltaY from StartY.
			float newLocation=deltaY;
			
			LayoutBox box=RenderData.FirstBox;
			
			if(fromCurrent){
				newLocation+=box.Position.Top;
			}else{
				newLocation+=StartY;
			}
			
			// Get the size of the button before the tab:
			float sizeBefore=SizeBefore();
			
			float barSize=BarSize();
			
			float max=barSize+sizeBefore-box.Height;
			
			if(newLocation<sizeBefore){
				newLocation=sizeBefore;
			}else if(newLocation>max){
				newLocation=max;
			}
			
			if(newLocation==box.Position.Top){
				return;
			}
			
			Style.top=newLocation+"fpx";
			
			if(scrollTarget){
				
				if(ScrollBar.DivertOutput){
					
					float tabSize=box.Height;
					
					float progress=(newLocation-sizeBefore)/(barSize-tabSize);
					
					ScrollBar.OnScrolled(progress);
					
				}else{
				
					HtmlElement target=ScrollBar.scrollTarget;
					
					if(target!=null){
						float progress=(newLocation-sizeBefore)/barSize;
						
						target.style.Computed.ChangeTagProperty("scroll-top",(progress * target.style.Computed.FirstBox.ContentHeight)+"px");
						
						// And request a redraw:
						target.htmlDocument.RequestLayout();
					}
				
				}
				
			}
			
		}
		
	}
	
}