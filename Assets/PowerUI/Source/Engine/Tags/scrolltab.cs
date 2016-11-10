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

namespace PowerUI{
	
	/// <summary>
	/// Handles the tab (draggable part) of a horizontal or vertical scrollbar.
	/// </summary>
	
	public class HtmlScrollTabElement:HtmlElement{
		
		/// <summary>The scrollbar this tab belongs to.</summary>
		public HtmlInputElement ScrollBar;
		
		
		public HtmlScrollTabElement(){
			// We're always draggable:
			this["draggable"]="1";
		}
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>The distance the input pointer must move in order to start dragging this element.</summary>
		public override float DragStartDistance{
			get{
				// Scrollbars start dragging immediately:
				return 1f;
			}
		}
		
		/// <summary>Called when this element started getting dragged.</summary>
		public override void OnDragStart(DragEvent e){
			
			// Cache the scrollbar:
			GetScrollBar();
			
			// And tell the tab it got dragged:
			DragTab(e);
			
		}
		
		/// <summary>The size (either width or height) of the element before the scroll tab.</summary>
		public virtual float SizeBefore(){
			return 0;
		}
		
		/// <summary>The size (either width or height) of the element after the scroll tab.</summary>
		public virtual float SizeAfter(){
			return 0;
		}
		
		/// <summary>The total border/padding size on this axis of the tab itself.</summary>
		public virtual float StyleSize(){
			return 0;
		}
		
		/// <summary>Called when the element has been scrolled.</summary>
		/// <param name="progress">A value from 0->1 which denotes how much the content has been scrolled by.</param>
		public void ElementScrolled(float progress){
			// Get the position the tab should be at (Compensating for the arrow button):
			float scrollPoint=(progress*BarSize())+SizeBefore();
			
			#warning incorrect
			// Make it relative to the current location:
			if(UseX()){
				scrollPoint-=style.Computed.FirstBox.Position.Left;
			}else{
				scrollPoint-=style.Computed.FirstBox.Position.Top;
			}
			
			// Scroll the tab only:
			ScrollBy(scrollPoint,true,false);
		}
		
		/// <summary>Sets up the <see cref="PowerUI.ScrollTabTag.ScrollBar"/> property.</summary>
		public void GetScrollBar(){
			if(ScrollBar==null){
				// Get the scroll bar:
				ScrollBar=parentElement as HtmlInputElement;
			}
		}
		
		/// <summary>Checks if this is a horizontal scrollbar.</summary>
		///	<returns>True if this is a horizontal scrollbar which uses the x axis; false otherwise.</returns>
		public virtual bool UseX(){
			return false;
		}
		
		/// <summary>Gets the length of the bar in pixels.</summary>
		/// <returns>The pixel length of the scrollbar.</returns>
		public virtual float BarSize(){
			return 0;
		}
		
		/// <summary>The current position of the tab. Note that width of elements before the tab like buttons are included in this.</summary>
		public virtual float TabPosition(){
			return 0;
		}
		
		/// <summary>Gets this tabs progress along the scrollbar, taking into account the size of the tab itself.</summary>
		public float TabProgress(){
			// Get the size of the track, accounting for the tab itself:
			float trackSize=(BarSize()-TabSize());
			
			// Get the tab position:
			float position=(TabPosition()-SizeBefore());
			
			return position / trackSize;
		}
		
		/// <summary>Scrolls this scrollbar by the given number of pixels.
		/// Note that this may fail if the scrollbar cannot scroll any further.</summary>
		/// <param name="pixels">The number of pixels to scroll this bar by.</param>
		public void ScrollBy(float pixels){
			ScrollBy(pixels,true,true);
		}
		
		/// <summary>Scrolls this scrollbar by the given number of pixels, optionally relative to a fixed point on the bar.
		/// Note that this may fail if the scrollbar cannot scroll any further.</summary>
		/// <param name="pixels">The number of pixels to scroll this bar by.</param>
		/// <param name="fromCurrent">True if pixels is relative to where the tab currently is. False if pixels is relative
		/// to where the bar was when the mouse was clicked. See e.g. <see cref="PowerUI.VScrollTabTag.StartY"/>.</param>
		/// <param name="scrollTarget">True if the target should also be scrolled.</param>
		public virtual void ScrollBy(float pixels,bool fromCurrent,bool scrollTarget){}
		
		/// <summary>Called when the tab is dragged.</summary>
		/// <param name="e">The drag event.</param>
		public virtual void DragTab(DragEvent e){}
		
		/// <summary>Makes the tab a percentage size relative to the length of the bar.</summary>
		/// <param name="percentSize">A value from 0->1 that represents how visible the content
		/// is and as a result how long the tab is.</param>
		public void ApplyTabSize(float percentSize){
			
			// How wide is the border/padding of the tab?
			float styleSize=StyleSize();
			
			// How big should the new tab be?
			float newTabSize=(percentSize*BarSize())-styleSize;
			
			if(newTabSize==TabSize()){
				// It didn't change.
				return;
			}
			
			// Apply the new tab size:
			SetTabSize(newTabSize);
		}
		
		/// <summary>Scrolls this tab to the specific percentage along the bar. 0-1. Optionally scrolls the target element.</summary>
		
		public void ScrollToPoint(float percent,bool scrollTarget){
			ScrollTo(percent * BarSize(),scrollTarget);
		}
		
		/// <summary>Scrolls this tab to the specific position along the bar. Optionally scrolls the target element.</summary>
		public virtual void ScrollTo(float location,bool scrollTarget){
		}
		
		/// <summary>Sets the tab to be the given size in pixels.</summary>
		/// <param name="newSize">The size in pixels of the tab.</param>
		protected virtual void SetTabSize(float newSize){}
		
		/// <summary>Gets the tabs size in pixels.</summary>
		/// <returns>The size of the tab in pixels.</returns>
		public virtual float TabSize(){
			return 0;
		}
		
	}
	
}